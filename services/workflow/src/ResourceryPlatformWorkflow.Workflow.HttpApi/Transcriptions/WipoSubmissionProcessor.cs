using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public class WipoSubmissionProcessor(
    IConfiguration configuration,
    IDistributedCache distributedCache,
    ILogger<WipoSubmissionProcessor> logger
) : ITransientDependency
{
    private const string DefaultWipoBaseUrl = "http://172.18.25.4:8088/S2T/API/ECOWAS";
    private const string DefaultWipoUsername = "ecowasapis2t";
    private const string DefaultWipoPassword = "ecowasapipwd";
    private const string DefaultOrganizationCode = "ECOWAS";
    private const string PendingTranscriptionCacheKeyPrefix = "workflow:transcription:pending:";
    private const int MaxWipoLogPayloadLength = 800;

    private static readonly DistributedCacheEntryOptions PendingTranscriptionCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2),
        SlidingExpiration = TimeSpan.FromHours(6)
    };

    private readonly IConfiguration _configuration = configuration;
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly ILogger<WipoSubmissionProcessor> _logger = logger;

    private string WipoBaseUrl =>
        (_configuration["Transcription:Wipo:BaseUrl"] ?? DefaultWipoBaseUrl).TrimEnd('/');

    private string WipoUsername =>
        _configuration["Transcription:Wipo:Username"] ?? DefaultWipoUsername;

    private string WipoPassword =>
        _configuration["Transcription:Wipo:Password"] ?? DefaultWipoPassword;

    private string OrganizationCode =>
        _configuration["Transcription:Wipo:OrganizationCode"] ?? DefaultOrganizationCode;

    public async Task ProcessAsync(SubmitToWipoBackgroundJobArgs args)
    {
        if (args == null || string.IsNullOrWhiteSpace(args.SourceReferenceId))
        {
            return;
        }

        var sourceReferenceId = args.SourceReferenceId.Trim();
        var staged = await GetPendingTranscriptionAsync(sourceReferenceId)
            ?? new TranscriptionController.PendingTranscriptionCacheItem { SourceReferenceId = sourceReferenceId };

        staged.Status = "Submitting";
        staged.LastUpdatedUtc = DateTime.UtcNow;
        await SetPendingTranscriptionAsync(staged);

        try
        {
            if (string.IsNullOrWhiteSpace(args.FilePath) || !File.Exists(args.FilePath))
            {
                staged.Status = "SubmissionFailed";
                staged.WipoUploadResponseRaw = "Queued file could not be found on server.";
                staged.LastUpdatedUtc = DateTime.UtcNow;
                await SetPendingTranscriptionAsync(staged);
                return;
            }

            var uploadQuery =
                $"organization_code={Uri.EscapeDataString(OrganizationCode)}" +
                $"&source_reference_id={Uri.EscapeDataString(sourceReferenceId)}" +
                $"&language={Uri.EscapeDataString(args.Language ?? "en")}" +
                $"&input_format={Uri.EscapeDataString(args.InputFormat ?? "webm")}";

            var endpoint = $"{WipoBaseUrl}/MediaUpload?{uploadQuery}";

            using var httpClient = CreateWipoClient();

            _logger.LogInformation(
                "Queued WIPO upload request prepared. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}, Language={Language}, InputFormat={InputFormat}, FileName={FileName}",
                endpoint,
                sourceReferenceId,
                args.Language,
                args.InputFormat,
                args.FileName
            );

            async Task<HttpResponseMessage> SendUploadAsync(string filePartName)
            {
                await using var stream = new FileStream(args.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var form = new MultipartFormDataContent();
                using var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(args.FileContentType) ? "application/octet-stream" : args.FileContentType
                );

                form.Add(fileContent, filePartName, args.FileName ?? Path.GetFileName(args.FilePath));
                return await httpClient.PostAsync(endpoint, form);
            }

            var usedFilePartName = "data";
            var response = await SendUploadAsync(usedFilePartName);
            var payload = await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                "Queued WIPO upload response. StatusCode={StatusCode}, FilePartName={FilePartName}, BodySnippet={BodySnippet}",
                (int)response.StatusCode,
                usedFilePartName,
                TruncateForLog(payload)
            );

            if (!response.IsSuccessStatusCode)
            {
                usedFilePartName = "file";
                response = await SendUploadAsync(usedFilePartName);
                payload = await response.Content.ReadAsStringAsync();

                _logger.LogWarning(
                    "Queued WIPO upload retry response. StatusCode={StatusCode}, FilePartName={FilePartName}, BodySnippet={BodySnippet}",
                    (int)response.StatusCode,
                    usedFilePartName,
                    TruncateForLog(payload)
                );
            }

            if (!response.IsSuccessStatusCode)
            {
                staged.Status = "SubmissionFailed";
                staged.WipoUploadResponseRaw = payload;
                staged.LastUpdatedUtc = DateTime.UtcNow;
                await SetPendingTranscriptionAsync(staged);
                return;
            }

            var responseStatus = "Submitted";
            var responseLinkJson = string.Empty;
            var responseLinkSrt = string.Empty;
            var responseLinkHtml = string.Empty;
            var responseLinkTxt = string.Empty;
            var responseLinkDocx = string.Empty;
            var responseLinkVerbatimDocx = string.Empty;

            try
            {
                using var json = JsonDocument.Parse(payload);
                var first = json.RootElement.ValueKind == JsonValueKind.Array
                    ? json.RootElement.EnumerateArray().FirstOrDefault()
                    : json.RootElement;

                var parsedStatus = GetJsonStringProperty(first, "status");
                responseStatus = string.IsNullOrWhiteSpace(parsedStatus) ? responseStatus : parsedStatus;
                responseLinkJson = GetTranscriptResultLink(first, "link_json");
                responseLinkSrt = GetTranscriptResultLink(first, "link_srt");
                responseLinkHtml = GetTranscriptResultLink(first, "link_html");
                responseLinkTxt = GetTranscriptResultLink(first, "link_txt");
                responseLinkDocx = GetTranscriptResultLink(first, "link_docx");
                responseLinkVerbatimDocx = GetTranscriptResultLink(first, "link_verbatimdocx");
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Queued WIPO upload response could not be parsed as JSON.");
            }

            staged.Status = responseStatus;
            staged.LinkJson = responseLinkJson;
            staged.LinkSrt = responseLinkSrt;
            staged.LinkHtml = responseLinkHtml;
            staged.LinkToVideo = DeriveLinkToVideo(staged.LinkToVideo, responseLinkJson, responseLinkHtml);
            staged.LinkTxt = responseLinkTxt;
            staged.LinkDocx = responseLinkDocx;
            staged.LinkVerbatimDocx = responseLinkVerbatimDocx;
            staged.WipoUploadResponseRaw = payload;
            staged.LastUpdatedUtc = DateTime.UtcNow;
            await SetPendingTranscriptionAsync(staged);
        }
        catch (Exception ex)
        {
            staged.Status = "SubmissionFailed";
            staged.WipoUploadResponseRaw = ex.Message;
            staged.LastUpdatedUtc = DateTime.UtcNow;
            await SetPendingTranscriptionAsync(staged);

            _logger.LogError(
                ex,
                "Queued WIPO upload failed. SourceReferenceId={SourceReferenceId}",
                sourceReferenceId
            );

            throw;
        }
        finally
        {
            TryDeleteStagedFile(args.FilePath);
        }
    }

    private HttpClient CreateWipoClient()
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(60)
        };

        var raw = $"{WipoUsername}:{WipoPassword}";
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
        return client;
    }

    private static string TruncateForLog(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var compact = value.Replace("\r", " ").Replace("\n", " ");
        return compact.Length <= MaxWipoLogPayloadLength
            ? compact
            : compact[..MaxWipoLogPayloadLength] + "...";
    }

    private static string GetJsonStringProperty(JsonElement element, string propertyName)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        if (!element.TryGetProperty(propertyName, out var propertyValue))
        {
            return string.Empty;
        }

        return propertyValue.ValueKind == JsonValueKind.String
            ? propertyValue.GetString() ?? string.Empty
            : string.Empty;
    }

    private static string GetTranscriptResultLink(JsonElement element, string propertyName)
    {
        var direct = GetJsonStringProperty(element, propertyName);
        if (!string.IsNullOrWhiteSpace(direct))
        {
            return direct;
        }

        if (element.ValueKind == JsonValueKind.Object &&
            element.TryGetProperty("transcript_results", out var transcriptResults) &&
            transcriptResults.ValueKind == JsonValueKind.Object)
        {
            return GetJsonStringProperty(transcriptResults, propertyName);
        }

        return string.Empty;
    }

    private static string DeriveLinkToVideo(string explicitLinkToVideo, string linkJson, string linkHtml)
    {
        if (!string.IsNullOrWhiteSpace(explicitLinkToVideo))
        {
            return explicitLinkToVideo.Trim();
        }

        var candidate = string.IsNullOrWhiteSpace(linkJson) ? linkHtml : linkJson;
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return string.Empty;
        }

        var normalized = candidate.Trim();
        normalized = Regex.Replace(
            normalized,
            @"_(?<lang>[a-z]{2})_(?<format>[a-z0-9]+)_[a-z]{2}\.(json|html)$",
            "_${lang}.${format}",
            RegexOptions.IgnoreCase
        );

        return normalized;
    }

    private async Task<TranscriptionController.PendingTranscriptionCacheItem> GetPendingTranscriptionAsync(string sourceReferenceId)
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceId))
        {
            return null;
        }

        var cacheKey = GetPendingTranscriptionCacheKey(sourceReferenceId);
        var json = await _distributedCache.GetStringAsync(cacheKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<TranscriptionController.PendingTranscriptionCacheItem>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private async Task SetPendingTranscriptionAsync(TranscriptionController.PendingTranscriptionCacheItem item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.SourceReferenceId))
        {
            return;
        }

        var cacheKey = GetPendingTranscriptionCacheKey(item.SourceReferenceId);
        var json = JsonSerializer.Serialize(item);
        await _distributedCache.SetStringAsync(cacheKey, json, PendingTranscriptionCacheOptions);
    }

    private static string GetPendingTranscriptionCacheKey(string sourceReferenceId)
    {
        return PendingTranscriptionCacheKeyPrefix + sourceReferenceId.Trim();
    }

    private void TryDeleteStagedFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to delete staged queued file. Path={Path}", filePath);
        }
    }
}
