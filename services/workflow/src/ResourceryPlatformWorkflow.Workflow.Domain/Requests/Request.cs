using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class Request : FullAuditedAggregateRoot<Guid>
{
    public RequestType RequestType { get; private set; }
    public RequestStatus RequestStatus { get; private set; }
    public ICollection<RequestDocument> Documents { get; private set; } 
    public string DocumentSetUrl { get; private set; }
    public string Description { get; private set; }
    public DocumentMigrationStatus DocumentMigrationStatus { get; private set; }
    public DateTime? DocumentsPublishedAt { get; private set; }


    protected Request()
    {
        Documents = new List<RequestDocument>();
    }

    public Request(Guid id, string documentSetUrl, string description, RequestType requestType)
        : base(id)
    {
        Documents = new List<RequestDocument>();
        SetDocumentSetUrl(documentSetUrl);
        SetDescription(description);
        SetRequestType(requestType);
        SetRequestStatus(RequestStatus.Pending);
        SetDocumentMigrationStatus(DocumentMigrationStatus.NotStarted);
    }

    public void SetDocumentSetUrl(string documentSetUrl)
    {
        DocumentSetUrl = Check.NotNullOrWhiteSpace(documentSetUrl, nameof(documentSetUrl));
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(description, nameof(description));
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
