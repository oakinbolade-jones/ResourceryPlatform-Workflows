using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using ResourceryPlatformWorkflow.Workflow.Summarization.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Summarization;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[ApiExplorerSettings(IgnoreApi = false)]
[ApiController]
[Route("api/workflow/summarization")]
public class SummarizationController(
    ISummarizationAppService appService)
    : WorkflowController,
        ISummarizationAppService
{
    private readonly ISummarizationAppService _appService = appService;

    [HttpPost("summarize")]
    public Task<MeetingSummaryDto> SummarizeAsync(SummarizeTranscriptInputDto input)
    {
        return _appService.SummarizeAsync(input);
    }
}
