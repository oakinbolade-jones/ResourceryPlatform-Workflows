using System.Threading;
using System.Threading.Tasks;

namespace ResourceryPlatformWorkflow.Workflow.Summarization;

public interface IAzureLanguageSummarizationService
{
    Task<string> SummarizeAsync(
        string transcript,
        string language = "en",
        CancellationToken cancellationToken = default
    );
}