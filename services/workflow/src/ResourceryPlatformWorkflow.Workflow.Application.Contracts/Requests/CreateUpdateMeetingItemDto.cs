#nullable enable

using System;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class CreateUpdateMeetingItemDto
{
    public string ItemName { get; set; } = default!;
    public string ItemCode { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string ServiceCenterCode { get; set; } = default!;
    public int QuantityNo { get; set; }
    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }
    public decimal Budget { get; set; }
    public string RemarkObservation { get; set; } = default!;
}
