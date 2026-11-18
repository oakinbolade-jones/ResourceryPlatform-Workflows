using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class SummarizationAppService : ApplicationService, ISummarizationAppService
{
    private readonly ISummarizationService _summarizationService;

    public SummarizationAppService(ISummarizationService summarizationService)
    {
        _summarizationService = summarizationService;
    }

    public async Task<SummarizationResultDto> SummarizeAsync(SummarizeTextInputDto input)
    {
        var summary = await _summarizationService.SummarizeAsync(
            input.Text,
            input.Language,
            input.SentenceCount
        );

        return new SummarizationResultDto
        {
            OriginalText = input.Text,
            Summary = summary,
            Language = input.Language,
            SentenceCount = input.SentenceCount
        };
    }
}
