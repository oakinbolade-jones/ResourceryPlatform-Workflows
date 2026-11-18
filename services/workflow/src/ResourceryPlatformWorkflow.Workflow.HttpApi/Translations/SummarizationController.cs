using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[ApiExplorerSettings(IgnoreApi = false)]
[ApiController]
[Route("api/workflow/summarization")]
public class SummarizationController(ISummarizationAppService appService)
    : WorkflowController,
        ISummarizationAppService
{
    private readonly ISummarizationAppService _appService = appService;

    [HttpPost("summarize")]
    public Task<SummarizationResultDto> SummarizeAsync(SummarizeTextInputDto input)
    {
        return _appService.SummarizeAsync(input);
    }
}
