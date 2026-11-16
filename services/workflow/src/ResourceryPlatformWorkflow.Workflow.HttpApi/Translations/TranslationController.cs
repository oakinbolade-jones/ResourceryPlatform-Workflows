using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using ResourceryPlatformWorkflow.Workflow.Summarization.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[ApiExplorerSettings(IgnoreApi = false)]
[ApiController]
[Route("api/workflow/translations")]
public class TranslationController(
    ITranslationAppService appService)
    : WorkflowController,
        ITranslationAppService
{
    private readonly ITranslationAppService _appService = appService;

    [HttpPost("translate")]
    public Task<TranslationResultDto> TranslateAsync(TranslateTextInputDto input)
    {
        return _appService.TranslateAsync(input);
    }
}
