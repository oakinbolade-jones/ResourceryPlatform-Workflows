using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public interface ISummarizationAppService : IApplicationService
{
    Task<SummarizationResultDto> SummarizeTextAsync(SummarizeTextInputDto input);
}
