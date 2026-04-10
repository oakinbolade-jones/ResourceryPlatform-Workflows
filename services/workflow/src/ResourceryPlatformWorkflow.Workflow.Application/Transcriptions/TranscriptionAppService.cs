using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

[Authorize(WorkflowPermissions.Transcriptions.Default)]
public class TranscriptionAppService(
    IRepository<Transcription, Guid> transcriptionRepository,
    TranscriptionManager transcriptionManager
) : WorkflowAppService, ITranscriptionAppService
{
    private readonly IRepository<Transcription, Guid> _transcriptionRepository = transcriptionRepository;
    private readonly TranscriptionManager _transcriptionManager = transcriptionManager;

    public async Task<TranscriptionDto> GetAsync(Guid id)
    {
        var transcription = await _transcriptionRepository.FindAsync(id);
        if (transcription == null)
        {
            throw TranscriptionException.NotFound(id);
        }

        return Map(transcription);
    }

    public async Task<List<TranscriptionDto>> GetListAsync()
    {
        var queryable = await _transcriptionRepository.GetQueryableAsync();
        var transcriptions = await AsyncExecuter.ToListAsync(queryable.OrderByDescending(x => x.CreationTime));
        return transcriptions.ConvertAll(Map);
    }

    [Authorize(WorkflowPermissions.Transcriptions.Create)]
    [AllowAnonymous]
    public async Task<TranscriptionDto> CreateAsync(CreateUpdateTranscriptionDto input)
    {
        Check.NotNull(input, nameof(input));

        var transcription = await _transcriptionManager.CreateAsync(
            input.Title,
            input.Description,
            input.IsPublic,
            input.DateOfTranscription,
            input.EventDate,
            input.MediaFile,
            input.Language,
            input.InputeFormat,
            input.Status,
            input.InputSource,
            input.ThumbNailImage
        );

        transcription.SetSourceReferenceId(input.SourceReferenceId);
        transcription.SetLinkJson(input.LinkJson);
        transcription.SetLinkSrt(input.LinkSrt);
        transcription.SetLinkHtml(input.LinkHtml);
        transcription.SetLinkTxt(input.LinkTxt);
        transcription.SetLinkDocx(input.LinkDocx);
        transcription.SetLinkVerbatimDocx(input.LinkVerbatimDocx);

        transcription = await _transcriptionRepository.InsertAsync(transcription, autoSave: true);
        return Map(transcription);
    }

    [Authorize(WorkflowPermissions.Transcriptions.Update)]
    [AllowAnonymous]
    public async Task<TranscriptionDto> UpdateAsync(Guid id, CreateUpdateTranscriptionDto input)
    {
        Check.NotNull(input, nameof(input));

        var transcription = await _transcriptionRepository.FindAsync(id);
        if (transcription == null)
        {
            throw TranscriptionException.NotFound(id);
        }

        transcription.SetTitle(input.Title);
        transcription.SetDescription(input.Description);
        transcription.SetIsPublic(input.IsPublic);
        transcription.SetDateOfTranscription(input.DateOfTranscription);
        transcription.SetEventDate(input.EventDate);
        transcription.SetMediaFile(input.MediaFile);
        transcription.SetLanguage(input.Language);
        transcription.SetInputeFormat(input.InputeFormat);
        transcription.SetStatus(input.Status);
        transcription.SetInputSource(input.InputSource);
        transcription.SetThumbNailImage(input.ThumbNailImage);
        transcription.SetSourceReferenceId(input.SourceReferenceId);
        transcription.SetLinkJson(input.LinkJson);
        transcription.SetLinkSrt(input.LinkSrt);
        transcription.SetLinkHtml(input.LinkHtml);
        transcription.SetLinkTxt(input.LinkTxt);
        transcription.SetLinkDocx(input.LinkDocx);
        transcription.SetLinkVerbatimDocx(input.LinkVerbatimDocx);

        transcription = await _transcriptionRepository.UpdateAsync(transcription, autoSave: true);
        return Map(transcription);
    }

    [Authorize(WorkflowPermissions.Transcriptions.Delete)]
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

        var queryable = await _transcriptionRepository.GetQueryableAsync();
        var transcription = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(x => x.SourceReferenceId == sourceReferenceId)
        );

        return transcription == null ? null : Map(transcription);
    }

    private static TranscriptionDto Map(Transcription transcription)
    {
        return new TranscriptionDto
        {
            Id = transcription.Id,
            Title = transcription.Title,
            Description = transcription.Description,
            IsPublic = transcription.IsPublic,
            DateOfTranscription = transcription.DateOfTranscription,
            EventDate = transcription.EventDate,
            MediaFile = transcription.MediaFile,
            Language = transcription.Language,
            InputeFormat = transcription.InputeFormat,
            Status = transcription.Status,
            InputSource = transcription.InputSource,
            ThumbNailImage = transcription.ThumbNailImage,
            SourceReferenceId = transcription.SourceReferenceId,
            LinkJson = transcription.LinkJson,
            LinkSrt = transcription.LinkSrt,
            LinkHtml = transcription.LinkHtml,
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
}
