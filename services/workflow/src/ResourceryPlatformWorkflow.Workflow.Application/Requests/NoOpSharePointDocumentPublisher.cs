using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class NoOpSharePointDocumentPublisher : ISharePointDocumentPublisher, ITransientDependency
{
    public Task<SharePointDocumentPublishResult> PublishAsync(
        Request request,
        RequestDocument requestDocument,
        CancellationToken cancellationToken = default
    )
    {
        // Placeholder implementation so the migration flow can be tested end-to-end.
        // Replace this service with a real SharePoint implementation.
        return Task.FromResult(
            new SharePointDocumentPublishResult
            {
                SharePointDocumentUrl = requestDocument.DocumentUrl,
                SharePointItemId = Guid.NewGuid().ToString("N")
            }
        );
    }
}
