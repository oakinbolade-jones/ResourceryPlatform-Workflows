using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class Request : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public RequestType RequestType { get; private set; }
    public RequestStatus RequestStatus { get; private set; }
    public string? Comment { get; private set; }

    public ICollection<RequestDocument> Documents { get; private set; } 
    public string? DocumentSetUrl { get; private set; }
    public string? Description { get; private set; }
    public Guid ServiceId { get; private set; }
    public DocumentMigrationStatus DocumentMigrationStatus { get; private set; }
    public DateTime? DocumentsPublishedAt { get; private set; }


    protected Request()
    {
        Documents = new List<RequestDocument>();
    }

    public Request(Guid id, string documentSetUrl, string description, Guid serviceId, RequestType requestType, string comment)
        : base(id)
    {
        Documents = new List<RequestDocument>();
        SetDocumentSetUrl(documentSetUrl);
        SetDescription(description);
        SetServiceId(serviceId);
        SetRequestType(requestType);
        SetRequestStatus(RequestStatus.Pending);
        SetDocumentMigrationStatus(DocumentMigrationStatus.NotStarted);
        SetComment(comment);
        Comment = comment;
    }

    public void SetDocumentSetUrl(string documentSetUrl)
    {
        DocumentSetUrl = string.IsNullOrWhiteSpace(documentSetUrl)
            ? null
            : documentSetUrl.Trim();
    }

    public void SetDescription(string description)
    {
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
            // Check.NotNullOrWhiteSpace(description, nameof(description));
    }

    public void SetServiceId(Guid serviceId)
    {
        ServiceId = Check.NotNull(serviceId, nameof(serviceId));
    }

    public void SetRequestType(RequestType requestType)
    {
        RequestType = requestType;
    }

    public void SetRequestStatus(RequestStatus requestStatus)
    {
        RequestStatus = requestStatus;
    }

    public void SetDocumentMigrationStatus(DocumentMigrationStatus documentMigrationStatus)
    {
        DocumentMigrationStatus = documentMigrationStatus;
    }

    public void SetDocumentsPublishedAt(DateTime? documentsPublishedAt)
    {
        DocumentsPublishedAt = documentsPublishedAt;
    }
     public void SetComment(string? comment)
    {
        Comment = string.IsNullOrWhiteSpace(comment)
            ? null
            : comment.Trim();
    }

    public void AddDocument(Guid documentId, string title, string description, string documentUrl)
    {
        Check.NotNull(documentId, nameof(documentId));
        var normalizedTitle = Check.NotNullOrWhiteSpace(title, nameof(title)).Trim();

        if (Documents.Any(x => x.Title.Trim().Equals(normalizedTitle, StringComparison.OrdinalIgnoreCase)))
        {
            throw RequestException.DuplicateDocument(normalizedTitle);
        }

        Documents.Add(new RequestDocument(documentId, Id, normalizedTitle, description, documentUrl));
    }
}
