using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using ResourceryPlatformWorkflow.Workflow.Translations;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class TranslationAppService : ApplicationService, ITranslationAppService
{
    private readonly ITranslationService _translationService;

    public TranslationAppService(
        ITranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task<TranslationResultDto> TranslateAsync(
        TranslateTextInputDto input)
    {
        var translatedText =
            await _translationService.TranslateAsync(
                input.Text,
                input.TargetLanguage,
                input.SourceLanguage
            );

        return new TranslationResultDto
        {
            OriginalText = input.Text,
            TranslatedText = translatedText,
            SourceLanguage = input.SourceLanguage,
            TargetLanguage = input.TargetLanguage
        };
    }
}