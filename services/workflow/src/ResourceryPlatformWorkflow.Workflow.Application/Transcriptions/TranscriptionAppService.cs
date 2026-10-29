using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

//[Authorize(WorkflowPermissions.Transcriptions.Default)]
public class TranscriptionAppService(
    IRepository<Transcription, Guid> transcriptionRepository,
    TranscriptionManager transcriptionManager
) : WorkflowAppService, ITranscriptionAppService
{
    private readonly IRepository<Transcription, Guid> _transcriptionRepository = transcriptionRepository;
    private readonly TranscriptionManager _transcriptionManager = transcriptionManager;

    [AllowAnonymous]
    //[Authorize(WorkflowPermissions.Transcriptions.View)]
    public async Task<TranscriptionDto> GetAsync(Guid id)
    {
        var transcription = await _transcriptionRepository.FindAsync(id);
        if (transcription == null)
        {
            throw TranscriptionException.NotFound(id);
        }

        return Map(transcription);
    }

    [AllowAnonymous]
    //[Authorize(WorkflowPermissions.Transcriptions.List)]
    public async Task<List<TranscriptionDto>> GetListAsync()
    {
        var queryable = await _transcriptionRepository.GetQueryableAsync();
        var transcriptions = await AsyncExecuter.ToListAsync(
            queryable
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    IsPublic = x.IsPublic,
                    PublishedToWebCast = x.PublishedToWebCast,
                    DateOfTranscription = x.DateOfTranscription,
                    EventDate = x.EventDate,
                    MediaFile = x.MediaFile,
                    Language = x.Language,
                    InputeFormat = x.InputeFormat,
                    Status = x.Status,
                    InputSource = x.InputSource,
                    SourceReferenceId = x.SourceReferenceId,
                    DocumentExtension = x.DocumentExtension,
                    Transcript = x.Transcript,
                    LinkJson = x.LinkJson,
                    LinkSrt = x.LinkSrt,
                    LinkHtml = x.LinkHtml,
                    LinkTxt = x.LinkTxt,
                    LinkDocx = x.LinkDocx,
                    LinkVerbatimDocx = x.LinkVerbatimDocx,
                    CreationTime = x.CreationTime,
                    CreatorId = x.CreatorId,
                    LastModificationTime = x.LastModificationTime,
                    LastModifierId = x.LastModifierId,
                    IsDeleted = x.IsDeleted,
                    DeleterId = x.DeleterId,
                    DeletionTime = x.DeletionTime,
                })
        );

        return transcriptions.Select(x => new TranscriptionDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            IsPublic = x.IsPublic,
            PublishedToWebCast = x.PublishedToWebCast,
            DateOfTranscription = x.DateOfTranscription,
            EventDate = x.EventDate,
            MediaFile = x.MediaFile,
            Language = x.Language,
            InputeFormat = x.InputeFormat,
            Status = x.Status,
            InputSource = x.InputSource,
            SourceReferenceId = x.SourceReferenceId,
            DocumentData = null,
            DocumentExtension = x.DocumentExtension,
            Transcript = x.Transcript,
            LinkJson = x.LinkJson,
            LinkSrt = x.LinkSrt,
            LinkHtml = x.LinkHtml,
            LinkToVideo = DeriveLinkToVideo(null, x.LinkJson, x.LinkHtml),
            LinkTxt = x.LinkTxt,
            LinkDocx = x.LinkDocx,
            LinkVerbatimDocx = x.LinkVerbatimDocx,
            CreationTime = x.CreationTime,
            CreatorId = x.CreatorId,
            LastModificationTime = x.LastModificationTime,
            LastModifierId = x.LastModifierId,
            IsDeleted = x.IsDeleted,
            DeleterId = x.DeleterId,
            DeletionTime = x.DeletionTime
        }).ToList();
    }

    [AllowAnonymous]
    public async Task<TranscriptionDto> CreateAsync(CreateUpdateTranscriptionDto input)
    {
        Check.NotNull(input, nameof(input));

        input.LinkToVideo = DeriveLinkToVideo(input.LinkToVideo, input.LinkJson, input.LinkHtml);

        var transcription = await _transcriptionManager.CreateAsync(
            input.Title,
            input.Description,
            input.IsPublic,
            input.PublishedToWebCast,
            input.DateOfTranscription,
            input.EventDate,
            input.MediaFile,
            input.Language,
            input.InputeFormat,
            input.Status,
            input.InputSource
        );

        transcription.SetSourceReferenceId(input.SourceReferenceId);
        transcription.SetLinkJson(input.LinkJson);
        transcription.SetLinkSrt(input.LinkSrt);
        transcription.SetLinkHtml(input.LinkHtml);
        transcription.SetLinkTxt(input.LinkTxt);
        transcription.SetLinkDocx(input.LinkDocx);
        transcription.SetLinkVerbatimDocx(input.LinkVerbatimDocx);
        transcription.SetDocumentData(Array.Empty<byte>());
        transcription.SetDocumentExtension(input.DocumentExtension);

        transcription = await _transcriptionRepository.InsertAsync(transcription, autoSave: true);
        return Map(transcription);
    }

    [AllowAnonymous]
    public async Task<TranscriptionDto> UpdateAsync(Guid id, UpdateTranscriptionDto input)
    {
        Check.NotNull(input, nameof(input));

        input.LinkToVideo = DeriveLinkToVideo(input.LinkToVideo, input.LinkJson, input.LinkHtml);

        var transcription = await _transcriptionRepository.FindAsync(id);
        if (transcription == null)
        {
            throw TranscriptionException.NotFound(id);
        }

        transcription.SetTitle(input.Title);
        transcription.SetDescription(input.Description);
        transcription.SetIsPublic(input.IsPublic);
        transcription.SetPublishedToWebCast(input.PublishedToWebCast);
        transcription.SetDateOfTranscription(input.DateOfTranscription);
        transcription.SetEventDate(input.EventDate);
        transcription.SetMediaFile(input.MediaFile);
        transcription.SetLanguage(input.Language);
        transcription.SetInputeFormat(input.InputeFormat);
        transcription.SetStatus(input.Status);
        transcription.SetInputSource(input.InputSource);
        transcription.SetSourceReferenceId(input.SourceReferenceId);
        transcription.SetLinkJson(input.LinkJson);
        transcription.SetLinkSrt(input.LinkSrt);
        transcription.SetLinkHtml(input.LinkHtml);
        transcription.SetLinkTxt(input.LinkTxt);
        transcription.SetLinkDocx(input.LinkDocx);
        transcription.SetLinkVerbatimDocx(input.LinkVerbatimDocx);
        transcription.SetDocumentData(Array.Empty<byte>());
        transcription.SetDocumentExtension(input.DocumentExtension);

        transcription = await _transcriptionRepository.UpdateAsync(transcription, autoSave: true);
        return Map(transcription);
    }

    [AllowAnonymous]
    //[Authorize(WorkflowPermissions.Transcriptions.Delete)]
    public Task DeleteAsync(Guid id)
    {
        return _transcriptionManager.DeleteAsync(id);
    }

    [AllowAnonymous]
    public async Task<TranscriptionDto> GetBySourceReferenceIdAsync(string sourceReferenceId)
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceId))
        {
            return null;
        }

        sourceReferenceId = sourceReferenceId.Trim();

        var queryable = await _transcriptionRepository.GetQueryableAsync();
        var transcription = await AsyncExecuter.FirstOrDefaultAsync(
            queryable
                .Where(x => x.SourceReferenceId == sourceReferenceId)
                .Select(x => new
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    IsPublic = x.IsPublic,
                    PublishedToWebCast = x.PublishedToWebCast,
                    DateOfTranscription = x.DateOfTranscription,
                    EventDate = x.EventDate,
                    MediaFile = x.MediaFile,
                    Language = x.Language,
                    InputeFormat = x.InputeFormat,
                    Status = x.Status,
                    InputSource = x.InputSource,
                    SourceReferenceId = x.SourceReferenceId,
                    DocumentExtension = x.DocumentExtension,
                    Transcript = x.Transcript,
                    LinkJson = x.LinkJson,
                    LinkSrt = x.LinkSrt,
                    LinkHtml = x.LinkHtml,
                    LinkTxt = x.LinkTxt,
                    LinkDocx = x.LinkDocx,
                    LinkVerbatimDocx = x.LinkVerbatimDocx,
                    CreationTime = x.CreationTime,
                    CreatorId = x.CreatorId,
                    LastModificationTime = x.LastModificationTime,
                    LastModifierId = x.LastModifierId,
                    IsDeleted = x.IsDeleted,
                    DeleterId = x.DeleterId,
                    DeletionTime = x.DeletionTime,
                })
        );

        if (transcription == null)
        {
            return null;
        }

        return new TranscriptionDto
        {
            Id = transcription.Id,
            Title = transcription.Title,
            Description = transcription.Description,
            IsPublic = transcription.IsPublic,
            PublishedToWebCast = transcription.PublishedToWebCast,
            DateOfTranscription = transcription.DateOfTranscription,
            EventDate = transcription.EventDate,
            MediaFile = transcription.MediaFile,
            Language = transcription.Language,
            InputeFormat = transcription.InputeFormat,
            Status = transcription.Status,
            InputSource = transcription.InputSource,
            SourceReferenceId = transcription.SourceReferenceId,
            DocumentData = null,
            DocumentExtension = transcription.DocumentExtension,
            Transcript = transcription.Transcript,
            LinkJson = transcription.LinkJson,
            LinkSrt = transcription.LinkSrt,
            LinkHtml = transcription.LinkHtml,
            LinkToVideo = DeriveLinkToVideo(null, transcription.LinkJson, transcription.LinkHtml),
            LinkTxt = transcription.LinkTxt,
            LinkDocx = transcription.LinkDocx,
            LinkVerbatimDocx = transcription.LinkVerbatimDocx,
            CreationTime = transcription.CreationTime,
            CreatorId = transcription.CreatorId,
            LastModificationTime = transcription.LastModificationTime,
            LastModifierId = transcription.LastModifierId,
            IsDeleted = transcription.IsDeleted,
            DeleterId = transcription.DeleterId,
            DeletionTime = transcription.DeletionTime
        };
    }

    [AllowAnonymous]
    public async Task<TranscriptionDto> SaveTranscriptAsync(string sourceReferenceId, string transcript)
    {
        Check.NotNullOrWhiteSpace(sourceReferenceId, nameof(sourceReferenceId));
        sourceReferenceId = sourceReferenceId.Trim();

        var queryable = await _transcriptionRepository.GetQueryableAsync();
        var transcription = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(x => x.SourceReferenceId == sourceReferenceId)
        );

        if (transcription == null)
        {
            throw TranscriptionException.NotFound(Guid.Empty);
        }

        transcription.SetTranscript(transcript);
        transcription = await _transcriptionRepository.UpdateAsync(transcription, autoSave: true);
        return Map(transcription);
    }

    private static TranscriptionDto Map(Transcription transcription)
    {
        return new TranscriptionDto
        {
            Id = transcription.Id,
            Title = transcription.Title,
            Description = transcription.Description,
            IsPublic = transcription.IsPublic,
            PublishedToWebCast = transcription.PublishedToWebCast,
            DateOfTranscription = transcription.DateOfTranscription,
            EventDate = transcription.EventDate,
            MediaFile = transcription.MediaFile,
            Language = transcription.Language,
            InputeFormat = transcription.InputeFormat,
            Status = transcription.Status,
            InputSource = transcription.InputSource,
            SourceReferenceId = transcription.SourceReferenceId,
            DocumentData = null,
            DocumentExtension = transcription.DocumentExtension,
            Transcript = transcription.Transcript,
            LinkJson = transcription.LinkJson,
            LinkSrt = transcription.LinkSrt,
            LinkHtml = transcription.LinkHtml,
            LinkToVideo = DeriveLinkToVideo(null, transcription.LinkJson, transcription.LinkHtml),
            LinkTxt = transcription.LinkTxt,
            LinkDocx = transcription.LinkDocx,
            LinkVerbatimDocx = transcription.LinkVerbatimDocx,
            CreationTime = transcription.CreationTime,
            CreatorId = transcription.CreatorId,
            LastModificationTime = transcription.LastModificationTime,
            LastModifierId = transcription.LastModifierId,
            IsDeleted = transcription.IsDeleted,
            DeleterId = transcription.DeleterId,
            DeletionTime = transcription.DeletionTime
        };
    }

    internal static TranscriptionDto MapForQuery(Transcription transcription)
    {
        var dto = Map(transcription);
        dto.DocumentData = null;
        return dto;
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

        // Examples:
        // ..._xx_mp3_en.html -> ..._xx.mp3
        // ..._xx_webm_en.json -> ..._xx.webm
        normalized = Regex.Replace(
            normalized,
            @"_(?<lang>[a-z]{2})_(?<format>[a-z0-9]+)_[a-z]{2}\.(json|html)$",
            "_${lang}.${format}",
            RegexOptions.IgnoreCase
        );

        return normalized;
    }
}
