using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using ResourceryPlatformWorkflow.Workflow.Summarization.Dtos;

namespace ResourceryPlatformWorkflow.Workflow;

public interface ISummarizationAppService : IApplicationService
{
    Task<MeetingSummaryDto> SummarizeAsync(
        SummarizeTranscriptInputDto input
    );
}