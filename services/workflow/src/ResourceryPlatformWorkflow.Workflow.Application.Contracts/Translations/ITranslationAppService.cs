using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public interface ITranslationAppService : IApplicationService
{
    Task<TranslationResultDto> TranslateAsync(TranslateTextInputDto input);
}
