#nullable enable

using System;
using ResourceryPlatformWorkflow.Workflow.Meetings;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class CreateUpdateMeetingDto
{
    public string Title { get; set; } = default!;
    public DateTime DepartureDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MeetingType Type { get; set; }
    public string ReferenceNumber { get; set; } = default!;
    public int NumberOfParticipants { get; set; }
    public string Location { get; set; } = default!;
    public string ContactPhone { get; set; } = default!;
    public string ContactEmail { get; set; } = default!;
    public string ContactName { get; set; } = default!;
    public string HostName { get; set; } = default!;
    public string HostPhoneNumber { get; set; } = default!;
    public string HostEmail { get; set; } = default!;
    public string? CoHost1Name { get; set; }
    public string? CoHost1PhoneNumber { get; set; }
    public string? CoHost1Email { get; set; }
    public string? CoHost2Name { get; set; }
    public string? CoHost2PhoneNumber { get; set; }
    public string? CoHost2Email { get; set; }
    public string? GLNumberRefreshments { get; set; }
    public string? GLNumberHotel { get; set; }
    public string? GLNumberCarHire { get; set; }
    public string? GLNumberEquipment { get; set; }
    public string? GLNumberLanguageServices { get; set; }
    public string? CostCenterNumberRefreshments { get; set; }
    public string? CostCenterNumberHotel { get; set; }
    public string? CostCenterNumberCarHire { get; set; }
    public string? CostCenterNumberEquipment { get; set; }
    public string? CostCenterNumberLanguageServices { get; set; }
}
