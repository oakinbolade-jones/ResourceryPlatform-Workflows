using System.Threading;
using System.Threading.Tasks;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public interface ISharePointDocumentPublisher
{
    Task<SharePointDocumentPublishResult> PublishAsync(
        Request request,
        RequestDocument requestDocument,
        CancellationToken cancellationToken = default
    );
}
