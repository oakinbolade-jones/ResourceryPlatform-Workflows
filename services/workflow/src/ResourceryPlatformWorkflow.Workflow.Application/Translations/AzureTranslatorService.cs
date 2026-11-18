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
using ResourceryPlatformWorkflow.Workflow.Translations;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class AzureTranslatorService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly AzureTranslatorOptions _options;
    private readonly ILogger<AzureTranslatorService> _logger;

    public AzureTranslatorService(
        HttpClient httpClient,
        IOptions<AzureTranslatorOptions> options,
        ILogger<AzureTranslatorService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> TranslateAsync(
        string text,
        string targetLanguage,
        string? sourceLanguage = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text is required.");

        if (string.IsNullOrWhiteSpace(targetLanguage))
            throw new ArgumentException("Target language is required.");

        if (string.IsNullOrWhiteSpace(_options.Key))
        {
            throw new InvalidOperationException(
                "Azure__Translator__Key or AZURETRANSLATOR_KEY is not configured."
            );
        }

        var route =
            $"translate?api-version=3.0&to={Uri.EscapeDataString(targetLanguage)}";

        if (!string.IsNullOrWhiteSpace(sourceLanguage))
        {
            route += $"&from={Uri.EscapeDataString(sourceLanguage)}";
        }

        var requestBody = new[]
        {
            new { Text = text }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post, route);

        request.Headers.Add(
            "Ocp-Apim-Subscription-Key",
            _options.Key);

        if (!string.IsNullOrWhiteSpace(_options.Region))
        {
            request.Headers.Add(
                "Ocp-Apim-Subscription-Region",
                _options.Region);
        }

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = JsonContent.Create(requestBody);

        try
        {
            using var response = await _httpClient.SendAsync(
                request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<AzureTranslationResponse[]>(
                    cancellationToken: cancellationToken);

            var translatedText = result?
                .FirstOrDefault()?
                .Translations?
                .FirstOrDefault()?
                .Text;

            return translatedText ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Azure translation failed for language {Language}",
                targetLanguage);

            throw;
        }
    }

    private class AzureTranslationResponse
    {
        [JsonPropertyName("translations")]
        public AzureTranslationItem[]? Translations { get; set; }
    }

    private class AzureTranslationItem
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
