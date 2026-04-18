using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResourceryPlatformWorkflow.Workflow.Transcriptions;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[ApiExplorerSettings(IgnoreApi = false)]
[ApiController]
[Route("api/workflow/transcription")]
public class TranscriptionController(
    ITranscriptionAppService transcriptionAppService,
    ILogger<TranscriptionController> logger,
    IConfiguration configuration)
    : WorkflowController,
        ITranscriptionAppService
{
private readonly ITranscriptionAppService _transcriptionAppService = transcriptionAppService ?? throw new ArgumentNullException(nameof(transcriptionAppService));
    private readonly ILogger<TranscriptionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    // Update this in code to control where files are written.
    // Example: @"D:\RecordedVideos"
    private const string SaveRootPath = @"C:\RecordedVideos";
    private const string DefaultWipoBaseUrl = "http://s2t.ecowas.int:8088/S2T/API/ECOWAS";
    private const string DefaultWipoUsername = "ecowasapis2t";
    private const string DefaultWipoPassword = "ecowasapipwd";
    private const string DefaultOrganizationCode = "ECOWAS";
    private const int MaxWipoLogPayloadLength = 800;

    private string WipoBaseUrl =>
        (_configuration["Transcription:Wipo:BaseUrl"] ?? DefaultWipoBaseUrl).TrimEnd('/');

    private string WipoUsername =>
        _configuration["Transcription:Wipo:Username"] ?? DefaultWipoUsername;

    private string WipoPassword =>
        _configuration["Transcription:Wipo:Password"] ?? DefaultWipoPassword;

    private string OrganizationCode =>
        _configuration["Transcription:Wipo:OrganizationCode"] ?? DefaultOrganizationCode;

    [HttpGet("{id}")]
    public Task<TranscriptionDto> GetAsync(Guid id)
    {
        return _transcriptionAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<System.Collections.Generic.List<TranscriptionDto>> GetListAsync()
    {
        return _transcriptionAppService.GetListAsync();
    }

    [HttpPost]
    public Task<TranscriptionDto> CreateAsync(CreateUpdateTranscriptionDto input)
    {
        return _transcriptionAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<TranscriptionDto> UpdateAsync(Guid id, CreateUpdateTranscriptionDto input)
    {
        return _transcriptionAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _transcriptionAppService.DeleteAsync(id);
    }

    [HttpGet("by-source-reference")]
    public Task<TranscriptionDto> GetBySourceReferenceIdAsync([FromQuery] string sourceReferenceId)
    {
        return _transcriptionAppService.GetBySourceReferenceIdAsync(sourceReferenceId);
    }

    [HttpPost("save-recording")]
    [RequestSizeLimit(500_000_000)] // 500 MB
    public async Task<IActionResult> SaveRecordingAsync([FromForm] SaveRecordingInput input)
    {
        if (input.File == null || input.File.Length == 0)
        {
            return BadRequest(new { message = "No file was uploaded." });
        }

        Directory.CreateDirectory(SaveRootPath);

        var safeDirectory = ResolveSafeSubDirectory(input.DirectoryHint);
        var targetDirectory = Path.Combine(SaveRootPath, safeDirectory);
        Directory.CreateDirectory(targetDirectory);

        var extension = NormalizeExtension(input.Format, input.File.FileName);
        var safeName = $"recording-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}.{extension}";
        var fullPath = Path.Combine(targetDirectory, safeName);

        await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await input.File.CopyToAsync(stream);

        return Ok(new
        {
            message = "Recording saved.",
            path = fullPath,
            fileName = safeName,
            size = input.File.Length
        });
    }

    [HttpPost("save-info")]
    public async Task<IActionResult> SaveInfoAsync([FromBody] SaveTranscriptionInfoInput input)
    {
        if (input == null)
        {
            return BadRequest(new { message = "Invalid request body." });
        }

        if (string.IsNullOrWhiteSpace(input.Title))
        {
            return BadRequest(new { message = "Title is required." });
        }

        var eventDate = input.EventDate ?? input.DateOfTranscription ?? DateTime.UtcNow;
        var inputSource = ParseInputSource(input.TranscriptionMode);

        var dto = new CreateUpdateTranscriptionDto
        {
            Title = input.Title.Trim(),
            Description = input.Description,
            IsPublic = input.IsPublic,
            DateOfTranscription = eventDate,
            EventDate = eventDate,
            MediaFile = input.MediaFile ?? string.Empty,
            Language = string.IsNullOrWhiteSpace(input.Language) ? "en" : input.Language,
            InputeFormat = string.IsNullOrWhiteSpace(input.InputFormat) ? "webm" : input.InputFormat,
            Status = string.IsNullOrWhiteSpace(input.Status) ? "Draft" : input.Status,
            InputSource = inputSource,
            ThumbNailImage = input.ThumbNailImage,
            SourceReferenceId = input.SourceReferenceId,
        };

        TranscriptionDto transcription;
        if (input.TranscriptionId.HasValue)
        {
            transcription = await _transcriptionAppService.UpdateAsync(input.TranscriptionId.Value, dto);
        }
        else
        {
            transcription = await _transcriptionAppService.CreateAsync(dto);
        }

        return Ok(new
        {
            message = "Transcription information saved.",
            transcriptionId = transcription.Id,
            sourceReferenceId = transcription.SourceReferenceId,
            status = transcription.Status,
        });
    }

    [HttpPost("submit-to-wipo")]
    [RequestSizeLimit(500_000_000)]
    public async Task<IActionResult> SubmitToWipoAsync([FromForm] SubmitToWipoInput input)
    {
        if (input.File == null || input.File.Length == 0)
        {
            return BadRequest(new { message = "No file was uploaded." });
        }

        var sourceReferenceId = string.IsNullOrWhiteSpace(input.SourceReferenceId)
            ? Guid.NewGuid().ToString()
            : input.SourceReferenceId;
        var language = string.IsNullOrWhiteSpace(input.Language) ? "en" : input.Language;
        var inputFormat = string.IsNullOrWhiteSpace(input.InputFormat)
            ? NormalizeExtension(null, input.File.FileName)
            : NormalizeExtension(input.InputFormat, input.File.FileName);

        var uploadQuery =
            $"organization_code={Uri.EscapeDataString(OrganizationCode)}" +
            $"&source_reference_id={Uri.EscapeDataString(sourceReferenceId)}" +
            $"&language={Uri.EscapeDataString(language)}" +
            $"&input_format={Uri.EscapeDataString(inputFormat)}";
        var endpoint = $"{WipoBaseUrl}/MediaUpload?{uploadQuery}";

        using var httpClient = CreateWipoClient();

        _logger.LogInformation(
            "WIPO upload request prepared. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}, Language={Language}, InputFormat={InputFormat}, FileName={FileName}, FileSize={FileSize}, MimeType={MimeType}",
            endpoint,
            sourceReferenceId,
            language,
            inputFormat,
            input.File.FileName,
            input.File.Length,
            input.File.ContentType
        );

        async Task<HttpResponseMessage> SendUploadAsync(string filePartName)
        {
            using var stream = input.File.OpenReadStream();
            using var form = new MultipartFormDataContent();
            using var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrWhiteSpace(input.File.ContentType) ? "application/octet-stream" : input.File.ContentType
            );

            // Per WIPO docs, upload metadata is passed in query string.
            // Multipart body should carry the media file as field name "data".
            form.Add(fileContent, filePartName, input.File.FileName);

            _logger.LogInformation(
                "WIPO upload multipart built with keys=[{FilePartName}]",
                filePartName
            );

            return await httpClient.PostAsync(endpoint, form);
        }

        // WIPO docs use multipart field name "data".
        // Keep a fallback to "file" for compatibility with older integrations.
        var usedFilePartName = "data";
        HttpResponseMessage response;
        string payload;

        try
        {
            response = await SendUploadAsync(usedFilePartName);
            payload = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "WIPO upload connection failed. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}, FilePartName={FilePartName}",
                endpoint,
                sourceReferenceId,
                usedFilePartName
            );
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "Unable to reach WIPO transcription service.",
                endpoint,
                sourceReferenceId,
                filePartName = usedFilePartName,
                details = ex.Message
            });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(
                ex,
                "WIPO upload timed out. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}, FilePartName={FilePartName}",
                endpoint,
                sourceReferenceId,
                usedFilePartName
            );
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "WIPO transcription service timed out.",
                endpoint,
                sourceReferenceId,
                filePartName = usedFilePartName,
                details = ex.Message
            });
        }
        _logger.LogInformation(
            "WIPO upload response. StatusCode={StatusCode}, FilePartName={FilePartName}, BodySnippet={BodySnippet}",
            (int)response.StatusCode,
            usedFilePartName,
            TruncateForLog(payload)
        );

        if (!response.IsSuccessStatusCode)
        {
            usedFilePartName = "file";
            try
            {
                response = await SendUploadAsync(usedFilePartName);
                payload = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "WIPO upload retry connection failed. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}, FilePartName={FilePartName}",
                    endpoint,
                    sourceReferenceId,
                    usedFilePartName
                );
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    message = "Unable to reach WIPO transcription service after retry.",
                    endpoint,
                    sourceReferenceId,
                    filePartName = usedFilePartName,
                    details = ex.Message
                });
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(
                    ex,
                    "WIPO upload retry timed out. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}, FilePartName={FilePartName}",
                    endpoint,
                    sourceReferenceId,
                    usedFilePartName
                );
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    message = "WIPO transcription service timed out after retry.",
                    endpoint,
                    sourceReferenceId,
                    filePartName = usedFilePartName,
                    details = ex.Message
                });
            }
            _logger.LogWarning(
                "WIPO upload retry response. StatusCode={StatusCode}, FilePartName={FilePartName}, BodySnippet={BodySnippet}",
                (int)response.StatusCode,
                usedFilePartName,
                TruncateForLog(payload)
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                message = "WIPO upload failed",
                sourceReferenceId,
                transcriptionId = input.TranscriptionId,
                filePartName = usedFilePartName,
                details = payload
            });
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
            _logger.LogWarning(ex, "WIPO upload response could not be parsed as JSON. Persisting submission without result links.");
        }

        var dto = new CreateUpdateTranscriptionDto
        {
            Title = string.IsNullOrWhiteSpace(input.Title) ? "Untitled Transcription" : input.Title.Trim(),
            Description = input.Description,
            IsPublic = input.IsPublic,
            DateOfTranscription = input.DateOfTranscription ?? DateTime.UtcNow,
            EventDate = input.EventDate,
            MediaFile = input.File.FileName,
            Language = language,
            InputeFormat = inputFormat,
            Status = responseStatus,
            InputSource = input.InputSource,
            ThumbNailImage = input.ThumbNailImage,
            SourceReferenceId = sourceReferenceId,
            LinkJson = responseLinkJson,
            LinkSrt = responseLinkSrt,
            LinkHtml = responseLinkHtml,
            LinkTxt = responseLinkTxt,
            LinkDocx = responseLinkDocx,
            LinkVerbatimDocx = responseLinkVerbatimDocx
        };

        TranscriptionDto transcription;
        if (input.TranscriptionId.HasValue)
        {
            transcription = await _transcriptionAppService.UpdateAsync(input.TranscriptionId.Value, dto);
        }
        else
        {
            transcription = await _transcriptionAppService.CreateAsync(dto);
        }

        return Ok(new
        {
            message = "Submitted to WIPO",
            transcriptionId = transcription.Id,
            sourceReferenceId,
            language,
            inputFormat,
            wipoResponse = payload
        });
    }

    [HttpGet("transcription-status")]
    public async Task<IActionResult> GetTranscriptionStatusAsync([FromQuery] string sourceReferenceId, [FromQuery] string language = "en")
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceId))
        {
            return BadRequest(new { message = "sourceReferenceId is required." });
        }

        var query =
            $"organization_code={Uri.EscapeDataString(OrganizationCode)}" +
            $"&language={Uri.EscapeDataString(string.IsNullOrWhiteSpace(language) ? "en" : language)}" +
            $"&source_reference_id={Uri.EscapeDataString(sourceReferenceId)}";

        var endpoint = $"{WipoBaseUrl}/TranscriptionResults?{query}";

        using var httpClient = CreateWipoClient();
        _logger.LogInformation(
            "WIPO status request prepared. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}, Language={Language}",
            endpoint,
            sourceReferenceId,
            language
        );
        HttpResponseMessage response;
        string payload;

        try
        {
            response = await httpClient.GetAsync(endpoint);
            payload = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "WIPO status connection failed. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}",
                endpoint,
                sourceReferenceId
            );
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "Unable to reach WIPO transcription service.",
                endpoint,
                sourceReferenceId,
                details = ex.Message
            });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(
                ex,
                "WIPO status request timed out. Endpoint={Endpoint}, SourceReferenceId={SourceReferenceId}",
                endpoint,
                sourceReferenceId
            );
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "WIPO transcription service timed out.",
                endpoint,
                sourceReferenceId,
                details = ex.Message
            });
        }
        _logger.LogInformation(
            "WIPO status response. StatusCode={StatusCode}, BodySnippet={BodySnippet}",
            (int)response.StatusCode,
            TruncateForLog(payload)
        );

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                message = "WIPO status query failed",
                sourceReferenceId,
                details = payload
            });
        }
        var transcription = await _transcriptionAppService.GetBySourceReferenceIdAsync(sourceReferenceId);
        if (transcription != null)
        {
            var status = transcription.Status;
            using var json = JsonDocument.Parse(payload);
            var first = json.RootElement.ValueKind == JsonValueKind.Array
                ? json.RootElement.EnumerateArray().FirstOrDefault()
                : json.RootElement;

            if (first.ValueKind != JsonValueKind.Undefined &&
                first.TryGetProperty("status", out var statusProp))
            {
                status = statusProp.GetString() ?? status;
            }

            var linkJson = GetTranscriptResultLink(first, "link_json");
            var linkSrt = GetTranscriptResultLink(first, "link_srt");
            var linkHtml = GetTranscriptResultLink(first, "link_html");
            var linkTxt = GetTranscriptResultLink(first, "link_txt");
            var linkDocx = GetTranscriptResultLink(first, "link_docx");
            var linkVerbatimDocx = GetTranscriptResultLink(first, "link_verbatimdocx");

            var update = new CreateUpdateTranscriptionDto
            {
                Title = transcription.Title,
                Description = transcription.Description,
                IsPublic = transcription.IsPublic,
                DateOfTranscription = transcription.DateOfTranscription,
                EventDate = transcription.EventDate,
                MediaFile = transcription.MediaFile,
                Language = transcription.Language,
                InputeFormat = transcription.InputeFormat,
                Status = status,
                InputSource = transcription.InputSource,
                ThumbNailImage = transcription.ThumbNailImage,
                SourceReferenceId = transcription.SourceReferenceId,
                LinkJson = linkJson,
                LinkSrt = linkSrt,
                LinkHtml = linkHtml,
                LinkTxt = linkTxt,
                LinkDocx = linkDocx,
                LinkVerbatimDocx = linkVerbatimDocx
            };

            await _transcriptionAppService.UpdateAsync(transcription.Id, update);
        }

        return Content(payload, "application/json");
    }

    [HttpGet("download-result")]
    public async Task<IActionResult> DownloadResultAsync(
        [FromQuery] string sourceReferenceId,
        [FromQuery] string resultKey,
        [FromQuery] string language = "en")
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceId))
        {
            return BadRequest(new { message = "sourceReferenceId is required." });
        }

        if (string.IsNullOrWhiteSpace(resultKey))
        {
            return BadRequest(new { message = "resultKey is required." });
        }

        var transcription = await _transcriptionAppService.GetBySourceReferenceIdAsync(sourceReferenceId);
        if (transcription == null)
        {
            return NotFound(new { message = "Transcription not found.", sourceReferenceId });
        }

        var remoteUrl = ResolveResultUrl(transcription, resultKey);
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            return NotFound(new { message = "Requested result link is not available.", sourceReferenceId, resultKey });
        }

        if (!Uri.TryCreate(remoteUrl, UriKind.Absolute, out var remoteUri) ||
            (remoteUri.Scheme != Uri.UriSchemeHttp && remoteUri.Scheme != Uri.UriSchemeHttps))
        {
            return BadRequest(new { message = "Invalid result URL.", sourceReferenceId, resultKey });
        }

        using var httpClient = CreateWipoClient();
        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync(remoteUri);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "Result file download failed. SourceReferenceId={SourceReferenceId}, ResultKey={ResultKey}, Url={Url}, Language={Language}",
                sourceReferenceId,
                resultKey,
                remoteUrl,
                language
            );
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "Unable to download transcription result from upstream service.",
                sourceReferenceId,
                resultKey,
                details = ex.Message
            });
        }

        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, new
            {
                message = "Upstream file download failed.",
                sourceReferenceId,
                resultKey,
                details = TruncateForLog(details)
            });
        }

        var contentType = response.Content.Headers.ContentType?.MediaType;
        if (string.IsNullOrWhiteSpace(contentType))
        {
            contentType = "application/octet-stream";
        }

        var fileName = TryResolveDownloadFileName(response, remoteUri, resultKey);
        var fileBytes = await response.Content.ReadAsByteArrayAsync();

        return File(fileBytes, contentType, fileName);
    }

    private HttpClient CreateWipoClient()
    {
        var client = new HttpClient();
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

    private static string ResolveResultUrl(TranscriptionDto transcription, string resultKey)
    {
        var normalized = NormalizeResultKey(resultKey);
        return normalized switch
        {
            "linkjson" or "json" => transcription.LinkJson,
            "linksrt" or "srt" => transcription.LinkSrt,
            "linkhtml" or "html" or "linthtml" => transcription.LinkHtml,
            "linktxt" or "txt" => transcription.LinkTxt,
            "linkdocx" or "docx" => transcription.LinkDocx,
            "linkverbatimdocx" or "verbatimdocx" => transcription.LinkVerbatimDocx,
            _ => string.Empty
        };
    }

    private static string NormalizeResultKey(string resultKey)
    {
        if (string.IsNullOrWhiteSpace(resultKey))
        {
            return string.Empty;
        }

        var trimmed = resultKey.Trim().ToLowerInvariant();
        var chars = trimmed.Where(char.IsLetterOrDigit).ToArray();
        return new string(chars);
    }

    private static string TryResolveDownloadFileName(HttpResponseMessage response, Uri remoteUri, string resultKey)
    {
        var fromHeader = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName;
        if (!string.IsNullOrWhiteSpace(fromHeader))
        {
            return fromHeader.Trim('"');
        }

        var fromPath = Path.GetFileName(remoteUri.LocalPath);
        if (!string.IsNullOrWhiteSpace(fromPath))
        {
            return fromPath;
        }

        return $"transcription-{resultKey}.bin";
    }

    private static string ResolveSafeSubDirectory(string directoryHint)
    {
        if (string.IsNullOrWhiteSpace(directoryHint))
        {
            return string.Empty;
        }

        // Keep only the last segment to avoid path traversal and absolute paths.
        var normalized = directoryHint.Replace('\\', '/').Trim('/');
        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return string.Empty;
        }

        var leaf = segments[^1];
        foreach (var invalid in Path.GetInvalidFileNameChars())
        {
            leaf = leaf.Replace(invalid, '_');
        }

        return leaf;
    }

    private static string NormalizeExtension(string format, string originalFileName)
    {
        if (!string.IsNullOrWhiteSpace(format))
        {
            var f = format.Trim().Trim('.').ToLowerInvariant();
            if (f is "mp4" or "mp3" or "webm" or "ogg" or "mov")
            {
                return f;
            }
        }

        var ext = Path.GetExtension(originalFileName).TrimStart('.').ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(ext))
        {
            return ext;
        }

        return "webm";
    }

    private static InputSource ParseInputSource(string transcriptionMode)
    {
        return string.Equals(transcriptionMode, "record", StringComparison.OrdinalIgnoreCase)
            ? InputSource.Recording
            : InputSource.Upload;
    }

    public class SaveRecordingInput
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }

        [FromForm(Name = "directoryHint")]
        public string DirectoryHint { get; set; }

        [FromForm(Name = "format")]
        public string Format { get; set; }
    }

    public class SubmitToWipoInput
    {
        [FromForm(Name = "transcriptionId")]
        public Guid? TranscriptionId { get; set; }

        [FromForm(Name = "title")]
        public string Title { get; set; }

        [FromForm(Name = "description")]
        public string Description { get; set; }

        [FromForm(Name = "isPublic")]
        public bool IsPublic { get; set; }

        [FromForm(Name = "dateOfTranscription")]
        public DateTime? DateOfTranscription { get; set; }

        [FromForm(Name = "eventDate")]
        public DateTime? EventDate { get; set; }

        [FromForm(Name = "thumbNailImage")]
        public string ThumbNailImage { get; set; }

        [FromForm(Name = "inputSource")]
        public InputSource InputSource { get; set; } = InputSource.Upload;

        [FromForm(Name = "file")]
        public IFormFile File { get; set; }

        [FromForm(Name = "sourceReferenceId")]
        public string SourceReferenceId { get; set; }

        [FromForm(Name = "language")]
        public string Language { get; set; }

        [FromForm(Name = "inputFormat")]
        public string InputFormat { get; set; }
    }

    public class SaveTranscriptionInfoInput
    {
        public Guid? TranscriptionId { get; set; }
        public string SourceReferenceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateOfTranscription { get; set; }
        public DateTime? EventDate { get; set; }
        public string Language { get; set; }
        public string TranscriptionMode { get; set; }
        public string DocumentSetUrl { get; set; }
        public string ThumbNailImage { get; set; }
        public string MediaFile { get; set; }
        public string InputFormat { get; set; }
        public string Status { get; set; }
        public bool IsPublic { get; set; }
    }
}
