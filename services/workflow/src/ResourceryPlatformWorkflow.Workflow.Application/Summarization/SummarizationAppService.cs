using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using ResourceryPlatformWorkflow.Workflow.Summarization.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Summarization;

public class SummarizationAppService :
    ApplicationService,
    ISummarizationAppService
    
{
    private readonly IAzureLanguageSummarizationService
        _summarizationService;

    public SummarizationAppService(
        IAzureLanguageSummarizationService summarizationService)
    {
        _summarizationService = summarizationService;
    }

    public async Task<MeetingSummaryDto> SummarizeAsync(
        SummarizeTranscriptInputDto input)
    {
        if (input == null ||
            string.IsNullOrWhiteSpace(input.Transcript))
        {
            throw new UserFriendlyException(
                "Please provide a valid meeting transcript.");
        }

        var language = string.IsNullOrWhiteSpace(input.Language)
            ? "en"
            : input.Language.Trim();

        var summary = await _summarizationService.SummarizeAsync(
            input.Transcript,
            language
        );

        return new MeetingSummaryDto
        {
            Summary = summary,
            SummaryType = "AbstractiveSummary",
            Language = language
        };
    }
}
