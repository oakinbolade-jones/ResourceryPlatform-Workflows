using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class AzureLanguageService : ISummarizationService
{
    private readonly HttpClient _httpClient;
    private readonly AzureLanguageServiceOptions _options;
    private readonly ILogger<AzureLanguageService> _logger;

    public AzureLanguageService(
        HttpClient httpClient,
        IOptions<AzureLanguageServiceOptions> options,
        ILogger<AzureLanguageService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> SummarizeAsync(
        string text,
        string? language,
        int sentenceCount,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text is required.", nameof(text));
        }

        if (string.IsNullOrWhiteSpace(_options.Key))
        {
            throw new InvalidOperationException(
                "Azure__LanguageService__Key, Azure__LanguageService__Key1, or AZURELANGUAGESERVICE_KEY is not configured."
            );
        }

        sentenceCount = sentenceCount > 0
            ? sentenceCount
            : Math.Max(1, _options.DefaultSentenceCount);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "language/analyze-text/jobs?api-version=2023-04-01"
        );
        request.Headers.Add("Ocp-Apim-Subscription-Key", _options.Key);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = JsonContent.Create(new
        {
            displayName = "Workflow transcript summarization",
            analysisInput = new
            {
                documents = new[]
                {
                    new
                    {
                        id = "1",
                        language = string.IsNullOrWhiteSpace(language) ? "en" : language,
                        text
                    }
                }
            },
            tasks = new[]
            {
                new
                {
                    kind = "ExtractiveSummarization",
                    taskName = "summary",
                    parameters = new { sentenceCount }
                }
            }
        });

        try
        {
            using var createResponse = await _httpClient.SendAsync(request, cancellationToken);
            createResponse.EnsureSuccessStatusCode();

            var jobResult = await PollJobAsync(createResponse, cancellationToken);
            var sentences = jobResult.Tasks?.Items?
                .SelectMany(item => item.Results?.Documents ?? Array.Empty<SummarizationDocument>())
                .SelectMany(document => document.Sentences ?? Array.Empty<SummarySentence>())
                .OrderBy(sentence => sentence.Offset)
                .Select(sentence => sentence.Text)
                .Where(summarySentence => !string.IsNullOrWhiteSpace(summarySentence));

            return string.Join(" ", sentences ?? Enumerable.Empty<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Azure Language summarization failed for language {Language}", language);
            throw;
        }
    }

    private async Task<SummarizationJobResult> PollJobAsync(
        HttpResponseMessage createResponse,
        CancellationToken cancellationToken)
    {
        var location = createResponse.Headers.Location;
        if (location == null)
        {
            if (!createResponse.Headers.TryGetValues("Operation-Location", out var operationLocations))
            {
                throw new InvalidOperationException("Azure Language did not return an operation location.");
            }

            var operationLocation = operationLocations.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(operationLocation))
            {
                throw new InvalidOperationException("Azure Language did not return an operation location.");
            }

            location = new Uri(operationLocation, UriKind.RelativeOrAbsolute);
        }

        var operationUri = location;
        var timeout = TimeSpan.FromSeconds(Math.Max(10, _options.PollingTimeoutSeconds));
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(timeout);

        while (!timeoutSource.IsCancellationRequested)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, operationUri);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _options.Key);
            using var response = await _httpClient.SendAsync(request, timeoutSource.Token);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SummarizationJobResult>(
                cancellationToken: timeoutSource.Token
            ) ?? throw new InvalidOperationException("Azure Language returned an empty job result.");

            if (string.Equals(result.Status, "succeeded", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            if (string.Equals(result.Status, "failed", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(result.Status, "cancelled", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Azure Language summarization job {result.Status}.");
            }

            await Task.Delay(1000, timeoutSource.Token);
        }

        throw new TimeoutException("Azure Language summarization timed out.");
    }

    private sealed class SummarizationJobResult
    {
        public string Status { get; set; } = string.Empty;
        public SummarizationTasks? Tasks { get; set; }
    }

    private sealed class SummarizationTasks
    {
        public SummarizationTaskItem[]? Items { get; set; }
    }

    private sealed class SummarizationTaskItem
    {
        public SummarizationTaskResults? Results { get; set; }
    }

    private sealed class SummarizationTaskResults
    {
        public SummarizationDocument[]? Documents { get; set; }
    }

    private sealed class SummarizationDocument
    {
        public SummarySentence[]? Sentences { get; set; }
    }

    private sealed class SummarySentence
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("offset")]
        public int Offset { get; set; }
    }
}
