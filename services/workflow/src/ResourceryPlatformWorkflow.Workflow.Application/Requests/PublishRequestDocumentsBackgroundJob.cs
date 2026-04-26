using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class PublishRequestDocumentsBackgroundJob(
    IRepository<Request, Guid> requestRepository,
    RequestManager requestManager,
    ISharePointDocumentPublisher sharePointDocumentPublisher,
    ILogger<PublishRequestDocumentsBackgroundJob> logger
) : AsyncBackgroundJob<PublishRequestDocumentsJobArgs>, ITransientDependency
{
    private readonly IRepository<Request, Guid> _requestRepository = requestRepository;
    private readonly RequestManager _requestManager = requestManager;
    private readonly ISharePointDocumentPublisher _sharePointDocumentPublisher = sharePointDocumentPublisher;
    private readonly ILogger<PublishRequestDocumentsBackgroundJob> _logger = logger;

    [UnitOfWork]
    public override async Task ExecuteAsync(PublishRequestDocumentsJobArgs args)
    {
        var request = await _requestRepository.FindAsync(args.RequestId, includeDetails: true);
        if (request == null)
        {
            _logger.LogWarning("Request {RequestId} not found for SharePoint publish job.", args.RequestId);
            return;
        }

        if (request.RequestStatus != RequestStatus.Completed)
        {
            _logger.LogInformation(
                "Skipping publish for request {RequestId} because status is {RequestStatus}.",
                request.Id,
                request.RequestStatus
            );
            return;
        }

        await _requestManager.MarkMigrationInProgressAsync(request);

        foreach (var document in request.Documents.Where(x => x.MigrationStatus != DocumentMigrationStatus.Completed))
        {
            try
            {
                await _requestManager.MarkDocumentMigrationInProgressAsync(document);

                var publishResult = await _sharePointDocumentPublisher.PublishAsync(request, document);
                await _requestManager.MarkDocumentMigrationCompletedAsync(
                    document,
                    publishResult.SharePointDocumentUrl,
                    publishResult.SharePointItemId
                );
            }
            catch (Exception ex)
            {
                await _requestManager.MarkDocumentMigrationFailedAsync(document, ex.Message);
                _logger.LogError(
                    ex,
                    "Failed to publish request document {RequestDocumentId} for request {RequestId}.",
                    document.Id,
                    request.Id
                );
            }
        }

        await _requestManager.FinalizeMigrationStatusAsync(request);
        await _requestRepository.UpdateAsync(request, autoSave: true);
    }
}
