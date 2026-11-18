using System.Threading;
using System.Threading.Tasks;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public interface ISummarizationService
{
    Task<string> SummarizeAsync(
        string text,
        string? language,
        int sentenceCount,
        CancellationToken cancellationToken = default);
}
