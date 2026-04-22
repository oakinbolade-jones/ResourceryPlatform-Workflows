using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
    IConfiguration configuration,
    IDistributedCache distributedCache)
    : WorkflowController,
        ITranscriptionAppService
{
private readonly ITranscriptionAppService _transcriptionAppService = transcriptionAppService ?? throw new ArgumentNullException(nameof(transcriptionAppService));
    private readonly ILogger<TranscriptionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

    // Update this in code to control where files are written.
    // Example: @"D:\RecordedVideos"
    private const string SaveRootPath = @"C:\RecordedVideos";
    private const string DefaultWipoBaseUrl = "http://s2t.ecowas.int:8088/S2T/API/ECOWAS";
    private const string DefaultWipoUsername = "ecowasapis2t";
    private const string DefaultWipoPassword = "ecowasapipwd";
    private const string DefaultOrganizationCode = "ECOWAS";
    private const int MaxWipoLogPayloadLength = 800;
    private const string PendingTranscriptionCacheKeyPrefix = "workflow:transcription:pending:";
    private static readonly DistributedCacheEntryOptions PendingTranscriptionCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2),
        SlidingExpiration = TimeSpan.FromHours(6)
    };

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
        input.LinkToVideo = DeriveLinkToVideo(input.LinkToVideo, input.LinkJson, input.LinkHtml);
        return _transcriptionAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<TranscriptionDto> UpdateAsync(Guid id, UpdateTranscriptionDto input)
    {
        input.LinkToVideo = DeriveLinkToVideo(input.LinkToVideo, input.LinkJson, input.LinkHtml);
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

    [HttpPost("save-transcript")]
    public async Task<TranscriptionDto> SaveTranscriptAsync([FromQuery] string sourceReferenceId, [FromBody] SaveTranscriptInput input)
    {
        var resolvedSourceReferenceId = string.IsNullOrWhiteSpace(sourceReferenceId)
            ? input?.SourceReferenceId
            : sourceReferenceId;

        Check.NotNullOrWhiteSpace(resolvedSourceReferenceId, nameof(sourceReferenceId));
        resolvedSourceReferenceId = resolvedSourceReferenceId.Trim();

        var transcript = input?.Transcript ?? string.Empty;

        try
        {
            return await _transcriptionAppService.SaveTranscriptAsync(resolvedSourceReferenceId, transcript);
        }
        catch (BusinessException ex) when (ex.Code == WorkflowErrorCodes.Transcriptions.TranscriptionNotFound)
        {
            var staged = await GetPendingTranscriptionAsync(resolvedSourceReferenceId)
                ?? new PendingTranscriptionCacheItem { SourceReferenceId = resolvedSourceReferenceId };

            staged.Transcript = transcript;
            staged.LastUpdatedUtc = DateTime.UtcNow;
            await SetPendingTranscriptionAsync(staged);

            _logger.LogInformation(
                "Transcript staged in cache because no transcription record exists yet. SourceReferenceId={SourceReferenceId}",
                resolvedSourceReferenceId
            );

            return new TranscriptionDto
            {
                Id = staged.TranscriptionId ?? Guid.Empty,
                SourceReferenceId = resolvedSourceReferenceId,
                DocumentData = staged.DocumentData,
                DocumentExtension = staged.DocumentExtension ?? string.Empty,
                Transcript = transcript,
                Status = string.IsNullOrWhiteSpace(staged.Status) ? "Draft" : staged.Status,
                Title = staged.Title ?? string.Empty,
                Description = staged.Description ?? string.Empty,
                IsPublic = staged.IsPublic,
                PublishedToWebCast = staged.PublishedToWebCast,
                DateOfTranscription = staged.DateOfTranscription ?? DateTime.UtcNow,
                EventDate = staged.EventDate,
                MediaFile = staged.MediaFile,
                Language = staged.Language ?? "en",
                InputeFormat = staged.InputFormat ?? "webm",
                InputSource = staged.InputSource,
                LinkJson = staged.LinkJson ?? string.Empty,
                LinkSrt = staged.LinkSrt ?? string.Empty,
                LinkHtml = staged.LinkHtml ?? string.Empty,
                LinkToVideo = DeriveLinkToVideo(staged.LinkToVideo, staged.LinkJson, staged.LinkHtml),
                LinkTxt = staged.LinkTxt ?? string.Empty,
                LinkDocx = staged.LinkDocx ?? string.Empty,
                LinkVerbatimDocx = staged.LinkVerbatimDocx ?? string.Empty,
            };
        }
    }

    // Explicit interface implementation — routes through the app service.
    Task<TranscriptionDto> ITranscriptionAppService.SaveTranscriptAsync(string sourceReferenceId, string transcript)
    {
        return _transcriptionAppService.SaveTranscriptAsync(sourceReferenceId, transcript);
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

        var sourceReferenceId = string.IsNullOrWhiteSpace(input.SourceReferenceId)
            ? Guid.NewGuid().ToString()
            : input.SourceReferenceId.Trim();
        var eventDate = input.EventDate ?? input.DateOfTranscription ?? DateTime.UtcNow;
        var inputSource = ParseInputSource(input.TranscriptionMode);
        var staged = await GetPendingTranscriptionAsync(sourceReferenceId) ?? new PendingTranscriptionCacheItem
        {
            SourceReferenceId = sourceReferenceId
        };

        staged.TranscriptionId = input.TranscriptionId ?? staged.TranscriptionId;
        staged.Title = input.Title.Trim();
        staged.Description = input.Description;
        staged.IsPublic = input.IsPublic;
        staged.PublishedToWebCast = input.PublishedToWebCast;
        staged.DateOfTranscription = eventDate;
        staged.EventDate = input.EventDate ?? eventDate;
        staged.MediaFile = input.MediaFile ?? staged.MediaFile ?? string.Empty;
        staged.Transcript = input.Transcript ?? staged.Transcript;
        staged.LinkJson = string.IsNullOrWhiteSpace(input.LinkJson) ? staged.LinkJson : input.LinkJson;
        staged.LinkHtml = string.IsNullOrWhiteSpace(input.LinkHtml) ? staged.LinkHtml : input.LinkHtml;
        staged.LinkToVideo = DeriveLinkToVideo(input.LinkToVideo, staged.LinkJson, staged.LinkHtml);
        staged.Language = string.IsNullOrWhiteSpace(input.Language) ? "en" : input.Language;
        staged.InputFormat = string.IsNullOrWhiteSpace(input.InputFormat) ? "webm" : input.InputFormat;
        staged.Status = string.IsNullOrWhiteSpace(input.Status) ? "Draft" : input.Status;
        staged.InputSource = inputSource;
        staged.LastUpdatedUtc = DateTime.UtcNow;

        await SetPendingTranscriptionAsync(staged);

        return Ok(new
        {
            message = "Transcription information staged.",
            transcriptionId = staged.TranscriptionId,
            sourceReferenceId = staged.SourceReferenceId,
            status = staged.Status,
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
        var uploadedDocumentData = await ReadFileBytesAsync(input.File);
        var uploadedDocumentExtension = NormalizeDocumentExtension(input.File.FileName);

        var uploadQuery =
            $"organization_code={Uri.EscapeDataString(OrganizationCode)}" +
            $"&source_reference_id={Uri.EscapeDataString(sourceReferenceId)}" +
            $"&language={Uri.EscapeDataString(language)}" +
            $"&input_format={Uri.EscapeDataString(inputFormat)}";
        var endpoint = $"{WipoBaseUrl}/MediaUpload?{uploadQuery}";

        var staged = await GetPendingTranscriptionAsync(sourceReferenceId) ?? new PendingTranscriptionCacheItem
        {
            SourceReferenceId = sourceReferenceId
        };

        staged.TranscriptionId = input.TranscriptionId ?? staged.TranscriptionId;
        staged.Title = string.IsNullOrWhiteSpace(input.Title) ? (staged.Title ?? "Untitled Transcription") : input.Title.Trim();
        staged.Description = input.Description ?? staged.Description;
        staged.IsPublic = input.IsPublic;
        staged.PublishedToWebCast = input.PublishedToWebCast;
        staged.DateOfTranscription = input.DateOfTranscription ?? input.EventDate ?? staged.DateOfTranscription ?? DateTime.UtcNow;
        staged.EventDate = input.EventDate ?? staged.EventDate;
        staged.MediaFile = input.File.FileName;
        staged.DocumentData = uploadedDocumentData;
        staged.DocumentExtension = uploadedDocumentExtension;
        staged.Transcript = input.Transcript ?? staged.Transcript;
        staged.Language = language;
        staged.InputFormat = inputFormat;
        staged.InputSource = input.InputSource;
        staged.Status = "Submitting";
        staged.LastUpdatedUtc = DateTime.UtcNow;

        await SetPendingTranscriptionAsync(staged);

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
            staged.Status = "SubmissionFailed";
            staged.LastUpdatedUtc = DateTime.UtcNow;
            await SetPendingTranscriptionAsync(staged);
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
            staged.Status = "SubmissionFailed";
            staged.LastUpdatedUtc = DateTime.UtcNow;
            await SetPendingTranscriptionAsync(staged);
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
                staged.Status = "SubmissionFailed";
                staged.LastUpdatedUtc = DateTime.UtcNow;
                await SetPendingTranscriptionAsync(staged);
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
                staged.Status = "SubmissionFailed";
                staged.LastUpdatedUtc = DateTime.UtcNow;
                await SetPendingTranscriptionAsync(staged);
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
            staged.Status = "SubmissionFailed";
            staged.WipoUploadResponseRaw = payload;
            staged.LastUpdatedUtc = DateTime.UtcNow;
            await SetPendingTranscriptionAsync(staged);
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
        var responseLinkToVideo = string.Empty;

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
            responseLinkToVideo = DeriveLinkToVideo(null, responseLinkJson, responseLinkHtml);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "WIPO upload response could not be parsed as JSON. Persisting submission without result links.");
        }

        staged.Status = responseStatus;
        staged.LinkJson = responseLinkJson;
        staged.LinkSrt = responseLinkSrt;
        staged.LinkHtml = responseLinkHtml;
        staged.LinkToVideo = responseLinkToVideo;
        staged.LinkTxt = responseLinkTxt;
        staged.LinkDocx = responseLinkDocx;
        staged.LinkVerbatimDocx = responseLinkVerbatimDocx;
        staged.WipoUploadResponseRaw = payload;
        staged.LastUpdatedUtc = DateTime.UtcNow;
        await SetPendingTranscriptionAsync(staged);

        return Ok(new
        {
            message = "Submitted to WIPO",
            transcriptionId = staged.TranscriptionId,
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

        await ProcessWipoStatusPayloadAsync(sourceReferenceId, payload);

        return Content(payload, "application/json");
    }

    [HttpPost("wipo-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> WipoCallbackAsync([FromBody] JsonElement payload)
    {
        if (payload.ValueKind is not JsonValueKind.Array and not JsonValueKind.Object)
        {
            return BadRequest(new { message = "Invalid callback payload." });
        }

        var first = payload.ValueKind == JsonValueKind.Array
            ? payload.EnumerateArray().FirstOrDefault()
            : payload;

        var sourceReferenceId = GetJsonStringProperty(first, "source_reference_id");
        if (string.IsNullOrWhiteSpace(sourceReferenceId))
        {
            sourceReferenceId = GetJsonStringProperty(first, "sourceReferenceId");
        }

        if (string.IsNullOrWhiteSpace(sourceReferenceId))
        {
            return BadRequest(new { message = "sourceReferenceId is required in callback payload." });
        }

        var payloadText = payload.GetRawText();
        await ProcessWipoStatusPayloadAsync(sourceReferenceId, payloadText);

        return Ok(new
        {
            message = "Callback processed.",
            sourceReferenceId
        });
    }

    private async Task ProcessWipoStatusPayloadAsync(string sourceReferenceId, string payload)
    {
        sourceReferenceId = sourceReferenceId?.Trim();

        var parsedStatus = string.Empty;
        var linkJson = string.Empty;
        var linkSrt = string.Empty;
        var linkHtml = string.Empty;
        var linkTxt = string.Empty;
        var linkDocx = string.Empty;
        var linkVerbatimDocx = string.Empty;

        using (var json = JsonDocument.Parse(payload))
        {
            var first = json.RootElement.ValueKind == JsonValueKind.Array
                ? json.RootElement.EnumerateArray().FirstOrDefault()
                : json.RootElement;

            parsedStatus = GetJsonStringProperty(first, "status");
            linkJson = GetTranscriptResultLink(first, "link_json");
            linkSrt = GetTranscriptResultLink(first, "link_srt");
            linkHtml = GetTranscriptResultLink(first, "link_html");
            linkTxt = GetTranscriptResultLink(first, "link_txt");
            linkDocx = GetTranscriptResultLink(first, "link_docx");
            linkVerbatimDocx = GetTranscriptResultLink(first, "link_verbatimdocx");
        }

        var transcription = await _transcriptionAppService.GetBySourceReferenceIdAsync(sourceReferenceId);
        var staged = await GetPendingTranscriptionAsync(sourceReferenceId);

        // When the transcription is complete and a JSON result link is available, fetch its content
        // to populate the Transcript field in the database.
        var fetchedTranscriptJson = string.Empty;
        if (IsCompletedStatus(parsedStatus) && !string.IsNullOrWhiteSpace(linkJson))
        {
            fetchedTranscriptJson = await FetchLinkJsonContentAsync(linkJson);
        }

        if (transcription != null)
        {
            var resolvedStatus = !string.IsNullOrWhiteSpace(parsedStatus)
                ? parsedStatus
                : transcription.Status;

            if (!IsCompletedStatus(resolvedStatus))
            {
                // Strict mode: do not persist in-progress/failed statuses to DB.
                // Keep staged cache (if any) updated for later completion write-through.
                if (staged != null)
                {
                    staged.Status = string.IsNullOrWhiteSpace(parsedStatus) ? staged.Status : parsedStatus;
                    staged.LinkJson = string.IsNullOrWhiteSpace(linkJson) ? staged.LinkJson : linkJson;
                    staged.LinkSrt = string.IsNullOrWhiteSpace(linkSrt) ? staged.LinkSrt : linkSrt;
                    staged.LinkHtml = string.IsNullOrWhiteSpace(linkHtml) ? staged.LinkHtml : linkHtml;
                    staged.LinkToVideo = DeriveLinkToVideo(staged.LinkToVideo, staged.LinkJson, staged.LinkHtml);
                    staged.LinkTxt = string.IsNullOrWhiteSpace(linkTxt) ? staged.LinkTxt : linkTxt;
                    staged.LinkDocx = string.IsNullOrWhiteSpace(linkDocx) ? staged.LinkDocx : linkDocx;
                    staged.LinkVerbatimDocx = string.IsNullOrWhiteSpace(linkVerbatimDocx) ? staged.LinkVerbatimDocx : linkVerbatimDocx;
                    staged.WipoStatusResponseRaw = payload;
                    staged.LastUpdatedUtc = DateTime.UtcNow;
                    await SetPendingTranscriptionAsync(staged);
                }

                return;
            }

            // Use fetched JSON transcript if available; otherwise keep existing value.
            var resolvedTranscript = !string.IsNullOrWhiteSpace(fetchedTranscriptJson)
                ? fetchedTranscriptJson
                : transcription.Transcript;

            var update = new UpdateTranscriptionDto
            {
                Title = transcription.Title,
                Description = transcription.Description,
                IsPublic = transcription.IsPublic,
                PublishedToWebCast = transcription.PublishedToWebCast,
                DateOfTranscription = transcription.DateOfTranscription,
                EventDate = transcription.EventDate,
                MediaFile = transcription.MediaFile,
                Language = transcription.Language,
                InputeFormat = transcription.InputeFormat,
                Status = resolvedStatus,
                InputSource = transcription.InputSource,
                SourceReferenceId = transcription.SourceReferenceId,
                DocumentData = staged?.DocumentData?.Length > 0 ? staged.DocumentData : transcription.DocumentData,
                DocumentExtension = !string.IsNullOrWhiteSpace(staged?.DocumentExtension)
                    ? staged.DocumentExtension
                    : transcription.DocumentExtension,
                LinkJson = linkJson,
                LinkSrt = linkSrt,
                LinkHtml = linkHtml,
                LinkToVideo = DeriveLinkToVideo(transcription.LinkToVideo, linkJson, linkHtml),
                LinkTxt = linkTxt,
                LinkDocx = linkDocx,
                LinkVerbatimDocx = linkVerbatimDocx
            };

            await _transcriptionAppService.UpdateAsync(transcription.Id, update);
            if (!string.IsNullOrWhiteSpace(fetchedTranscriptJson))
            {
                await _transcriptionAppService.SaveTranscriptAsync(transcription.SourceReferenceId, fetchedTranscriptJson);
            }
            await RemovePendingTranscriptionAsync(sourceReferenceId);
        }
        else
        {
            if (staged == null && IsCompletedStatus(parsedStatus))
            {
                staged = new PendingTranscriptionCacheItem
                {
                    SourceReferenceId = sourceReferenceId,
                    Status = parsedStatus,
                    Language = "en",
                    InputFormat = "webm",
                    LastUpdatedUtc = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "No existing transcription or staged cache was found. Creating staged item from completed WIPO payload. SourceReferenceId={SourceReferenceId}",
                    sourceReferenceId
                );
            }

            if (staged == null)
            {
                return;
            }

            staged.Status = string.IsNullOrWhiteSpace(parsedStatus) ? staged.Status : parsedStatus;
            staged.LinkJson = string.IsNullOrWhiteSpace(linkJson) ? staged.LinkJson : linkJson;
            staged.LinkSrt = string.IsNullOrWhiteSpace(linkSrt) ? staged.LinkSrt : linkSrt;
            staged.LinkHtml = string.IsNullOrWhiteSpace(linkHtml) ? staged.LinkHtml : linkHtml;
            staged.LinkToVideo = DeriveLinkToVideo(staged.LinkToVideo, staged.LinkJson, staged.LinkHtml);
            staged.LinkTxt = string.IsNullOrWhiteSpace(linkTxt) ? staged.LinkTxt : linkTxt;
            staged.LinkDocx = string.IsNullOrWhiteSpace(linkDocx) ? staged.LinkDocx : linkDocx;
            staged.LinkVerbatimDocx = string.IsNullOrWhiteSpace(linkVerbatimDocx) ? staged.LinkVerbatimDocx : linkVerbatimDocx;
            // Persist fetched transcript JSON into the staged cache if available.
            if (!string.IsNullOrWhiteSpace(fetchedTranscriptJson))
            {
                staged.Transcript = fetchedTranscriptJson;
            }
            staged.WipoStatusResponseRaw = payload;
            staged.LastUpdatedUtc = DateTime.UtcNow;

            if (IsCompletedStatus(staged.Status))
            {
                var createOrUpdateDto = new CreateUpdateTranscriptionDto
                {
                    Title = string.IsNullOrWhiteSpace(staged.Title) ? "Untitled Transcription" : staged.Title,
                    Description = staged.Description,
                    IsPublic = staged.IsPublic,
                    PublishedToWebCast = staged.PublishedToWebCast,
                    DateOfTranscription = staged.DateOfTranscription ?? DateTime.UtcNow,
                    EventDate = staged.EventDate,
                    MediaFile = staged.MediaFile ?? string.Empty,
                    Language = string.IsNullOrWhiteSpace(staged.Language) ? "en" : staged.Language,
                    InputeFormat = string.IsNullOrWhiteSpace(staged.InputFormat) ? "webm" : staged.InputFormat,
                    Status = staged.Status,
                    InputSource = staged.InputSource,
                    SourceReferenceId = staged.SourceReferenceId,
                    DocumentData = staged.DocumentData,
                    DocumentExtension = staged.DocumentExtension,
                    LinkJson = staged.LinkJson,
                    LinkSrt = staged.LinkSrt,
                    LinkHtml = staged.LinkHtml,
                    LinkToVideo = DeriveLinkToVideo(staged.LinkToVideo, staged.LinkJson, staged.LinkHtml),
                    LinkTxt = staged.LinkTxt,
                    LinkDocx = staged.LinkDocx,
                    LinkVerbatimDocx = staged.LinkVerbatimDocx
                };

                try
                {
                    if (staged.TranscriptionId.HasValue)
                    {
                        var updateDto = new UpdateTranscriptionDto
                        {
                            Title = createOrUpdateDto.Title,
                            Description = createOrUpdateDto.Description,
                            IsPublic = createOrUpdateDto.IsPublic,
                            PublishedToWebCast = createOrUpdateDto.PublishedToWebCast,
                            DateOfTranscription = createOrUpdateDto.DateOfTranscription,
                            EventDate = createOrUpdateDto.EventDate,
                            MediaFile = createOrUpdateDto.MediaFile,
                            Language = createOrUpdateDto.Language,
                            InputeFormat = createOrUpdateDto.InputeFormat,
                            Status = createOrUpdateDto.Status,
                            InputSource = createOrUpdateDto.InputSource,
                            SourceReferenceId = createOrUpdateDto.SourceReferenceId,
                            DocumentData = createOrUpdateDto.DocumentData,
                            DocumentExtension = createOrUpdateDto.DocumentExtension,
                            LinkJson = createOrUpdateDto.LinkJson,
                            LinkSrt = createOrUpdateDto.LinkSrt,
                            LinkHtml = createOrUpdateDto.LinkHtml,
                            LinkToVideo = createOrUpdateDto.LinkToVideo,
                            LinkTxt = createOrUpdateDto.LinkTxt,
                            LinkDocx = createOrUpdateDto.LinkDocx,
                            LinkVerbatimDocx = createOrUpdateDto.LinkVerbatimDocx
                        };

                        await _transcriptionAppService.UpdateAsync(staged.TranscriptionId.Value, updateDto);

                        if (!string.IsNullOrWhiteSpace(staged.Transcript))
                        {
                            await _transcriptionAppService.SaveTranscriptAsync(staged.SourceReferenceId, staged.Transcript);
                        }
                    }
                    else
                    {
                        var created = await _transcriptionAppService.CreateAsync(createOrUpdateDto);
                        staged.TranscriptionId = created.Id;

                        if (!string.IsNullOrWhiteSpace(staged.Transcript))
                        {
                            await _transcriptionAppService.SaveTranscriptAsync(staged.SourceReferenceId, staged.Transcript);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to persist completed transcription payload. SourceReferenceId={SourceReferenceId}",
                        sourceReferenceId
                    );
                    throw;
                }

                await RemovePendingTranscriptionAsync(sourceReferenceId);
            }
            else
            {
                await SetPendingTranscriptionAsync(staged);
            }
        }
    }

    private async Task<string> FetchLinkJsonContentAsync(string linkJsonUrl)
    {
        if (!Uri.TryCreate(linkJsonUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            _logger.LogWarning(
                "LinkJson URL is invalid or not an absolute HTTP/HTTPS URL. Url={Url}",
                linkJsonUrl
            );
            return string.Empty;
        }

        _logger.LogInformation("Fetching transcript JSON from link_json. Url={Url}", linkJsonUrl);

        try
        {
            using var httpClient = CreateWipoClient();
            var response = await httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to fetch transcript JSON from link_json. Url={Url}, StatusCode={StatusCode}",
                    linkJsonUrl,
                    (int)response.StatusCode
                );
                return string.Empty;
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(
                "Transcript JSON fetched successfully from link_json. Url={Url}, Length={Length}",
                linkJsonUrl,
                content.Length
            );
            return content;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(
                ex,
                "Error fetching transcript JSON from link_json. Url={Url}",
                linkJsonUrl
            );
            return string.Empty;
        }
    }

    [HttpPost("sync-transcript-from-link-json")]
    public async Task<IActionResult> SyncTranscriptFromLinkJsonAsync([FromQuery] string sourceReferenceId)
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceId))
        {
            return BadRequest(new { message = L["Transcription:SourceReferenceIdRequired"] });
        }

        sourceReferenceId = sourceReferenceId.Trim();

        var transcription = await _transcriptionAppService.GetBySourceReferenceIdAsync(sourceReferenceId);
        if (transcription == null)
        {
            return NotFound(new { message = L["Transcription:NotFound"], sourceReferenceId });
        }

        if (string.IsNullOrWhiteSpace(transcription.LinkJson))
        {
            return BadRequest(new { message = L["Transcription:LinkJsonUnavailable"], sourceReferenceId });
        }

        var fetchedTranscriptJson = await FetchLinkJsonContentAsync(transcription.LinkJson);
        if (string.IsNullOrWhiteSpace(fetchedTranscriptJson))
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = L["Transcription:LinkJsonFetchFailed"],
                sourceReferenceId,
                linkJson = transcription.LinkJson
            });
        }

        await _transcriptionAppService.SaveTranscriptAsync(sourceReferenceId, fetchedTranscriptJson);

        return Ok(new
        {
            message = L["Transcription:SyncFromLinkJsonSuccess"],
            sourceReferenceId,
            transcriptionId = transcription.Id,
            transcriptLength = fetchedTranscriptJson.Length,
            linkJson = transcription.LinkJson
        });
    }

    [HttpGet("download-result")]
    public async Task<IActionResult> DownloadResultAsync(
        [FromQuery] string sourceReferenceId,
        [FromQuery] Guid? transcriptionId,
        [FromQuery] string resultKey,
        [FromQuery] string language = "en")
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceId) && !transcriptionId.HasValue)
        {
            return BadRequest(new { message = "sourceReferenceId or transcriptionId is required." });
        }

        if (string.IsNullOrWhiteSpace(resultKey))
        {
            return BadRequest(new { message = "resultKey is required." });
        }

        TranscriptionDto transcription;
        if (transcriptionId.HasValue)
        {
            transcription = await _transcriptionAppService.GetAsync(transcriptionId.Value);
        }
        else
        {
            transcription = await _transcriptionAppService.GetBySourceReferenceIdAsync(sourceReferenceId);
        }

        if (transcription == null)
        {
            return NotFound(new { message = "Transcription not found.", sourceReferenceId, transcriptionId });
        }

        var normalizedResultKey = NormalizeResultKey(resultKey);
        var transcriptDownload = TryResolveStoredDocumentDownload(transcription, normalizedResultKey);
        if (transcriptDownload.HasValue)
        {
            var (bytes, storedContentType, storedFileName) = transcriptDownload.Value;
            return File(bytes, storedContentType, storedFileName);
        }

        if (normalizedResultKey is "docx" or "txt" or "linkdocx" or "linktxt")
        {
            return UnprocessableEntity(new
            {
                message = "Download was not successful. Transcript text is unavailable.",
                sourceReferenceId,
                transcriptionId,
                resultKey
            });
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

        if (normalizedResultKey == "pdf" && !IsPdfContentType(contentType))
        {
            var upstreamText = DecodeResponseText(fileBytes, response.Content.Headers.ContentType?.CharSet);
            var plainText = HtmlToPlainText(upstreamText);

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                var pdfBytes = BuildPdfBytesFromText(plainText);
                return File(pdfBytes, ResolveContentType("pdf"), BuildDownloadFileName(transcription, "pdf"));
            }
        }

        return File(fileBytes, contentType, fileName);
    }

    private static bool IsPdfContentType(string contentType)
    {
        return !string.IsNullOrWhiteSpace(contentType) &&
               contentType.Contains("application/pdf", StringComparison.OrdinalIgnoreCase);
    }

    private static string DecodeResponseText(byte[] bytes, string charset)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(charset))
            {
                return Encoding.GetEncoding(charset).GetString(bytes);
            }
        }
        catch (ArgumentException)
        {
            // Fallback to UTF-8 below when the upstream charset is invalid/unknown.
        }

        return Encoding.UTF8.GetString(bytes);
    }

    private static string HtmlToPlainText(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var withLineBreaks = Regex.Replace(html, "<(br|BR)\\s*/?>", "\n");
        withLineBreaks = Regex.Replace(withLineBreaks, "</(p|P|div|DIV|li|LI|h1|H2|H3|H4|H5|H6)>", "\n");
        var noTags = Regex.Replace(withLineBreaks, "<[^>]+>", " ");
        var decoded = WebUtility.HtmlDecode(noTags);
        var normalized = Regex.Replace(decoded ?? string.Empty, "[ \t]+", " ");

        return normalized
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Trim();
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
            "linktovideo" or "video" or "mp4" => transcription.LinkToVideo,
            "linktxt" or "txt" => transcription.LinkTxt,
            "linkdocx" or "docx" => transcription.LinkDocx,
            "pdf" => transcription.LinkHtml,
            "linkverbatimdocx" or "verbatimdocx" => transcription.LinkVerbatimDocx,
            _ => string.Empty
        };
    }

    private (byte[] bytes, string contentType, string fileName)? TryResolveStoredDocumentDownload(
        TranscriptionDto transcription,
        string normalizedResultKey)
    {
        var requestedExtension = normalizedResultKey switch
        {
            "linkdocx" or "docx" => "docx",
            "pdf" => "pdf",
            "linktxt" or "txt" => "txt",
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(requestedExtension))
        {
            return null;
        }

        var transcriptText = ExtractTranscriptTextFromStoredField(transcription.Transcript);
        if (!string.IsNullOrWhiteSpace(transcriptText))
        {
            var downloadableText = BuildEcowasTranscriptDownloadContent(transcriptText);
            var bytes = requestedExtension switch
            {
                "pdf" => BuildPdfBytesFromText(downloadableText),
                "docx" => BuildDocxBytesFromText(downloadableText),
                _ => Encoding.UTF8.GetBytes(downloadableText)
            };

            return (bytes, ResolveContentType(requestedExtension), BuildDownloadFileName(transcription, requestedExtension));
        }

        return null;
    }

    private string BuildEcowasTranscriptDownloadContent(string transcriptText)
    {
        var normalizedText = (transcriptText ?? string.Empty).Trim();

        return string.Join("\n", new[]
        {
            L["Transcription:DownloadHeaderTitle"],
            L["Transcription:DownloadHeaderSubtitle"],
            L["Transcription:DownloadHeaderOrganization"],
            L["Transcription:DownloadHeaderCopyright"],
            ".................................................",
            string.Empty,
            string.Empty,
            normalizedText,
            string.Empty,
            string.Empty,
            L["Transcription:DownloadSupportLabel"],
            L["Transcription:DownloadSupportEmail1"],
            // L["Transcription:DownloadSupportEmail2"]
        });
    }

    private static byte[] BuildPdfBytesFromText(string content)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(24);
                    page.DefaultTextStyle(style => style.FontSize(11));

                    page.Content().Column(column =>
                    {
                        column.Spacing(4);
                        foreach (var line in SplitLinesPreserveBlank(content))
                        {
                            column.Item().Text(line);
                        }
                    });
                });
            })
            .GeneratePdf();
    }

    private static byte[] BuildDocxBytesFromText(string content)
    {
        using var memoryStream = new MemoryStream();
        using (var document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            var body = new Body();

            foreach (var line in SplitLinesPreserveBlank(content))
            {
                var text = new Text(line) { Space = SpaceProcessingModeValues.Preserve };
                var run = new Run(text);
                body.AppendChild(new Paragraph(run));
            }

            mainPart.Document.AppendChild(body);
            mainPart.Document.Save();
        }

        return memoryStream.ToArray();
    }

    private static string[] SplitLinesPreserveBlank(string content)
    {
        return (content ?? string.Empty)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');
    }

    private static string ExtractTranscriptTextFromStoredField(string rawTranscript)
    {
        if (string.IsNullOrWhiteSpace(rawTranscript))
        {
            return string.Empty;
        }

        object current = rawTranscript;
        for (var depth = 0; depth < 3; depth++)
        {
            if (current is not string jsonText)
            {
                break;
            }

            var trimmed = jsonText.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return string.Empty;
            }

            try
            {
                current = JsonSerializer.Deserialize<JsonElement>(trimmed);
            }
            catch (JsonException)
            {
                return trimmed;
            }
        }

        if (current is not JsonElement element || element.ValueKind != JsonValueKind.Object)
        {
            return current?.ToString()?.Trim() ?? string.Empty;
        }

        var transcript = GetTranscriptTextFromJsonElement(element);
        return string.IsNullOrWhiteSpace(transcript)
            ? rawTranscript.Trim()
            : transcript.Trim();
    }

    private static string GetTranscriptTextFromJsonElement(JsonElement element)
    {
        if (element.TryGetProperty("transcript", out var transcriptValue) && transcriptValue.ValueKind == JsonValueKind.String)
        {
            return transcriptValue.GetString() ?? string.Empty;
        }

        if (element.TryGetProperty("result", out var resultValue) &&
            resultValue.ValueKind == JsonValueKind.Object &&
            resultValue.TryGetProperty("transcript", out var nestedTranscriptValue) &&
            nestedTranscriptValue.ValueKind == JsonValueKind.String)
        {
            return nestedTranscriptValue.GetString() ?? string.Empty;
        }

        if (element.TryGetProperty("results", out var resultsValue) &&
            resultsValue.ValueKind == JsonValueKind.Array &&
            resultsValue.GetArrayLength() > 0)
        {
            var first = resultsValue[0];
            if (first.ValueKind == JsonValueKind.Object &&
                first.TryGetProperty("transcript", out var firstTranscriptValue) &&
                firstTranscriptValue.ValueKind == JsonValueKind.String)
            {
                return firstTranscriptValue.GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }

    private static string BuildDownloadFileName(TranscriptionDto transcription, string extension)
    {
        var slug = string.IsNullOrWhiteSpace(transcription.Title)
            ? "transcription"
            : transcription.Title.Trim().ToLowerInvariant();

        var safeChars = slug.Select(ch => char.IsLetterOrDigit(ch) ? ch : '-').ToArray();
        slug = new string(safeChars).Trim('-');
        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = "transcription";
        }

        return $"{slug}.{extension}";
    }

    private static string ResolveContentType(string extension)
    {
        return extension switch
        {
            "txt" => "text/plain",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
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

        if (normalized.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[..^5] + ".mp4";
        }

        if (normalized.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[..^5] + ".mp4";
        }

        // Convert examples like *_en_mp4_en.mp4 to *_en.mp4
        var marker = "_mp4_";
        var markerIndex = normalized.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex > 0)
        {
            var prefix = normalized[..markerIndex];
            var suffix = normalized[(markerIndex + marker.Length)..];
            var dotIndex = suffix.IndexOf('.');
            if (dotIndex >= 0)
            {
                suffix = suffix[..dotIndex];
            }

            normalized = prefix + ".mp4";
        }

        return normalized;
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

    private static string NormalizeDocumentExtension(string fileNameOrExtension)
    {
        if (string.IsNullOrWhiteSpace(fileNameOrExtension))
        {
            return string.Empty;
        }

        var extension = fileNameOrExtension.Trim();
        extension = extension.StartsWith(".", StringComparison.Ordinal) ? extension[1..] : extension;

        if (extension.Contains('.'))
        {
            extension = Path.GetExtension(extension).TrimStart('.');
        }

        return extension.Trim().ToLowerInvariant();
    }

    private static async Task<byte[]> ReadFileBytesAsync(IFormFile file)
    {
        if (file == null || file.Length <= 0)
        {
            return Array.Empty<byte>();
        }

        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    private static InputSource ParseInputSource(string transcriptionMode)
    {
        return string.Equals(transcriptionMode, "record", StringComparison.OrdinalIgnoreCase)
            ? InputSource.Recording
            : InputSource.Upload;
    }

    private static bool IsCompletedStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        var normalized = status.Trim().ToLowerInvariant();
        return normalized is "finished" or "done" or "completed";
    }

    private async Task<PendingTranscriptionCacheItem> GetPendingTranscriptionAsync(string sourceReferenceId)
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
            return JsonSerializer.Deserialize<PendingTranscriptionCacheItem>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private async Task SetPendingTranscriptionAsync(PendingTranscriptionCacheItem item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.SourceReferenceId))
        {
            return;
        }

        var cacheKey = GetPendingTranscriptionCacheKey(item.SourceReferenceId);
        var json = JsonSerializer.Serialize(item);
        await _distributedCache.SetStringAsync(cacheKey, json, PendingTranscriptionCacheOptions);
    }

    private async Task RemovePendingTranscriptionAsync(string sourceReferenceId)
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceId))
        {
            return;
        }

        var cacheKey = GetPendingTranscriptionCacheKey(sourceReferenceId);
        await _distributedCache.RemoveAsync(cacheKey);
    }

    private static string GetPendingTranscriptionCacheKey(string sourceReferenceId)
    {
        return PendingTranscriptionCacheKeyPrefix + sourceReferenceId.Trim();
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

        [FromForm(Name = "publishedToWebCast")]
        public bool PublishedToWebCast { get; set; }

        [FromForm(Name = "dateOfTranscription")]
        public DateTime? DateOfTranscription { get; set; }

        [FromForm(Name = "eventDate")]
        public DateTime? EventDate { get; set; }

        [FromForm(Name = "inputSource")]
        public InputSource InputSource { get; set; } = InputSource.Upload;

        [FromForm(Name = "file")]
        public IFormFile File { get; set; }

        [FromForm(Name = "sourceReferenceId")]
        public string SourceReferenceId { get; set; }

        [FromForm(Name = "transcript")]
        public string Transcript { get; set; }

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
        public string MediaFile { get; set; }
        public string InputFormat { get; set; }
        public string Status { get; set; }
        public bool IsPublic { get; set; }
        public bool PublishedToWebCast { get; set; }
        public string Transcript { get; set; }
        public string LinkJson { get; set; }
        public string LinkHtml { get; set; }
        public string LinkToVideo { get; set; }
    }

    public class SaveTranscriptInput
    {
        public string SourceReferenceId { get; set; }
        public string Transcript { get; set; } = string.Empty;
    }

    public class PendingTranscriptionCacheItem
    {
        public Guid? TranscriptionId { get; set; }
        public string SourceReferenceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool PublishedToWebCast { get; set; }
        public DateTime? DateOfTranscription { get; set; }
        public DateTime? EventDate { get; set; }
        public string MediaFile { get; set; }
        public string Language { get; set; }
        public string InputFormat { get; set; }
        public byte[] DocumentData { get; set; }
        public string DocumentExtension { get; set; }
        public string Status { get; set; }
        public InputSource InputSource { get; set; }
        public string Transcript { get; set; }
        public string LinkJson { get; set; }
        public string LinkSrt { get; set; }
        public string LinkHtml { get; set; }
        public string LinkToVideo { get; set; }
        public string LinkTxt { get; set; }
        public string LinkDocx { get; set; }
        public string LinkVerbatimDocx { get; set; }
        public string WipoUploadResponseRaw { get; set; }
        public string WipoStatusResponseRaw { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
    }
}
