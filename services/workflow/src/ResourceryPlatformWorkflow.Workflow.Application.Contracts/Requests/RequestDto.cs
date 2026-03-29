using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class RequestDto : FullAuditedEntityDto<Guid>
{
    public RequestType RequestType { get; set; }
    public RequestStatus RequestStatus { get; set; }
    public string DocumentSetUrl { get; set; }
    public string Description { get; set; }
    public DocumentMigrationStatus DocumentMigrationStatus { get; set; }
    public DateTime? DocumentsPublishedAt { get; set; }
    public IList<RequestDocumentDto> Documents { get; set; } = new List<RequestDocumentDto>();
}
