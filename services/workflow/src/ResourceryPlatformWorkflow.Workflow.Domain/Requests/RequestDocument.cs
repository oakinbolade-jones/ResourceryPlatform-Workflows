using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class RequestDocument : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public Guid RequestId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string DocumentUrl { get; private set; }
    public string? SharePointDocumentUrl { get; private set; }
    public string? SharePointItemId { get; private set; }
    public DocumentMigrationStatus MigrationStatus { get; private set; }
    public string LastMigrationError { get; private set; }
    public DateTime? MigratedAt { get; private set; }

    protected RequestDocument() { }

    public RequestDocument(Guid id, Guid requestId, string title, string description, string documentUrl)
        : base(id)
    {
        RequestId = requestId;
        SetTitle(title);
        SetDescription(description);
        SetDocumentUrl(documentUrl);
        SetMigrationStatus(DocumentMigrationStatus.NotStarted);
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(description, nameof(description));
    }

    public void SetDocumentUrl(string documentUrl)
    {
        DocumentUrl = Check.NotNullOrWhiteSpace(documentUrl, nameof(documentUrl));
    }

    public void SetMigrationStatus(DocumentMigrationStatus migrationStatus)
    {
        MigrationStatus = migrationStatus;
    }

    public void MarkMigrationPending()
    {
        MigrationStatus = DocumentMigrationStatus.Pending;
        LastMigrationError = null;
    }

    public void MarkMigrationInProgress()
    {
        MigrationStatus = DocumentMigrationStatus.InProgress;
        LastMigrationError = null;
    }

    public void MarkMigrationCompleted(string sharePointDocumentUrl, string sharePointItemId)
    {
        SharePointDocumentUrl = Check.NotNullOrWhiteSpace(
            sharePointDocumentUrl,
            nameof(sharePointDocumentUrl)
        );
        SharePointItemId = Check.NotNullOrWhiteSpace(sharePointItemId, nameof(sharePointItemId));
        MigrationStatus = DocumentMigrationStatus.Completed;
        LastMigrationError = null;
        MigratedAt = DateTime.UtcNow;
    }

    public void MarkMigrationFailed(string error)
    {
        MigrationStatus = DocumentMigrationStatus.Failed;
        LastMigrationError = Check.NotNullOrWhiteSpace(error, nameof(error));
    }
}
