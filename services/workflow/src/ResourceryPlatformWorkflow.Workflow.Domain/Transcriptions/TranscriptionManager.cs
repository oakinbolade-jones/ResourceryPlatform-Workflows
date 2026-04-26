using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public class TranscriptionManager(IRepository<Transcription, Guid> transcriptionRepository) : DomainService
{
    private readonly IRepository<Transcription, Guid> _transcriptionRepository = transcriptionRepository;

    public Task<Transcription> CreateAsync(
        string title,
        string description,
        bool isPublic,
        bool publishedToWebCast,
        DateTime dateOfTranscription,
        DateTime? eventDate,
        string mediaFile,
        string language,
        string inputeFormat,
        string status,
        InputSource inputSource
    )
    {
        var transcription = new Transcription(
            GuidGenerator.Create(),
            title,
            description,
            isPublic,
            publishedToWebCast,
            dateOfTranscription,
            eventDate,
            mediaFile,
            language,
            inputeFormat,
            status,
            inputSource
        );

        return Task.FromResult(transcription);
    }

    public Task SetSubmissionInfoAsync(Transcription transcription, string sourceReferenceId, string status)
    {
        Check.NotNull(transcription, nameof(transcription));

        transcription.SetSourceReferenceId(sourceReferenceId);
        transcription.SetStatus(status);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        return _transcriptionRepository.DeleteAsync(id, autoSave: true);
    }
}
