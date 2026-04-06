using System;
using System.Collections.Generic;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class CreateUpdateRequestDto
{
    public RequestType RequestType { get; set; }
    public RequestStatus RequestStatus { get; set; }
    public string? DocumentSetUrl { get; set; }
    public string? Description { get; set; }
    public string? Comment { get; set; }
    public Guid ServiceId { get; set; }
    public IList<CreateUpdateRequestDocumentDto> Documents { get; set; } = new List<CreateUpdateRequestDocumentDto>();
}
