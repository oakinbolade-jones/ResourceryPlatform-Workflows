using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class RequestDocumentDto : EntityDto<Guid>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string DocumentUrl { get; set; }
    public string SharePointDocumentUrl { get; set; }
    public string SharePointItemId { get; set; }
    public DocumentMigrationStatus MigrationStatus { get; set; }
    public string LastMigrationError { get; set; }
    public DateTime? MigratedAt { get; set; }
}
