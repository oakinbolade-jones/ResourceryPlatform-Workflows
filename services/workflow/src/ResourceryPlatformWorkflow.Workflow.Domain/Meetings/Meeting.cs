#nullable enable

using System;
using System.Collections.Generic;
using ResourceryPlatformWorkflow.Workflow.Requests;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.Meetings;

public class Meeting : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public Guid? RequestId { get; private set; }
    public Request? Request { get; private set; }
    public string Title { get; private set; } = default!;
    public DateTime DepartureDate { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public MeetingType Type { get; private set; }
    public string ReferenceNumber { get; private set; } = default!;
    public int NumberOfParticipants { get; private set; }
    public string Location { get; private set; } = default!;
    public string ContactPhone { get; private set; } = default!;
    public string ContactEmail { get; private set; } = default!;
    public string ContactName { get; private set; } = default!;
    public string HostName { get; private set; } = default!;
    public string HostDesignation { get; private set; } = default!;
    public string HostPhoneNumber { get; private set; } = default!;
    public string HostEmail { get; private set; } = default!;
    public string? CoHost1Name { get; private set; }
    public string? CoHost1Designation { get; private set; }
    public string? CoHost1PhoneNumber { get; private set; }
    public string? CoHost1Email { get; private set; }
    public string? CoHost2Name { get; private set; }
    public string? CoHost2Designation { get; private set; }
    public string? CoHost2PhoneNumber { get; private set; }
    public string? CoHost2Email { get; private set; }
    public string? GLNumberRefreshments { get; private set; }
    public string? GLNumberHotel { get; private set; }
    public string? GLNumberCarHire { get; private set; }
    public string? GLNumberEquipment { get; private set; }
    public string? GLNumberLanguageServices { get; private set; }
    public string? CostCenterNumberRefreshments { get; private set; }
    public string? CostCenterNumberHotel { get; private set; }
    public string? CostCenterNumberCarHire { get; private set; }
    public string? CostCenterNumberEquipment { get; private set; }
    public string? CostCenterNumberLanguageServices { get; private set; }
    public virtual ICollection<MeetingItem> MeetingItems { get; set; }

    protected Meeting()
    {
        MeetingItems = new List<MeetingItem>();
    }

    public Meeting(
        Guid id,
        string title,
        DateTime departureDate,
        DateTime startDate,
        DateTime endDate,
        MeetingType type,
        string referenceNumber,
        int numberOfParticipants,
        string location,
        string contactPhone,
        string contactEmail,
        string contactName,
        string hostName,
        string hostPhoneNumber,
        string hostEmail)
        : base(id)
    {
        SetTitle(title);
        SetDepartureDate(departureDate);
        SetStartDate(startDate);
        SetEndDate(endDate);
        SetType(type);
        SetReferenceNumber(referenceNumber);
        SetNumberOfParticipants(numberOfParticipants);
        SetLocation(location);
        SetContactPhone(contactPhone);
        SetContactEmail(contactEmail);
        SetContactName(contactName);
        SetHostName(hostName);
        SetHostPhoneNumber(hostPhoneNumber);
        SetHostEmail(hostEmail);
        MeetingItems = new List<MeetingItem>();
    }

    public void SetTitle(string title) => Title = Check.NotNullOrWhiteSpace(title, nameof(title));
    public void SetDepartureDate(DateTime departureDate) => DepartureDate = departureDate;
    public void SetStartDate(DateTime startDate) => StartDate = startDate;
    public void SetEndDate(DateTime endDate) => EndDate = endDate;
    public void SetType(MeetingType type) => Type = type;
    public void SetReferenceNumber(string referenceNumber) => ReferenceNumber = Check.NotNullOrWhiteSpace(referenceNumber, nameof(referenceNumber));
    public void SetNumberOfParticipants(int numberOfParticipants)
    {
        if (numberOfParticipants <= 0)
            throw new ArgumentException("Number of participants must be greater than zero.", nameof(numberOfParticipants));
        NumberOfParticipants = numberOfParticipants;
    }
    public void SetLocation(string location) => Location = Check.NotNullOrWhiteSpace(location, nameof(location));
    public void SetContactPhone(string contactPhone) => ContactPhone = Check.NotNullOrWhiteSpace(contactPhone, nameof(contactPhone));
    public void SetContactEmail(string contactEmail) => ContactEmail = Check.NotNullOrWhiteSpace(contactEmail, nameof(contactEmail));
    public void SetContactName(string contactName) => ContactName = Check.NotNullOrWhiteSpace(contactName, nameof(contactName));
    public void SetHostName(string hostName) => HostName = Check.NotNullOrWhiteSpace(hostName, nameof(hostName));
    public void SetHostPhoneNumber(string hostPhoneNumber) => HostPhoneNumber = Check.NotNullOrWhiteSpace(hostPhoneNumber, nameof(hostPhoneNumber));
    public void SetHostEmail(string hostEmail) => HostEmail = Check.NotNullOrWhiteSpace(hostEmail, nameof(hostEmail));
    public void SetRequestId(Guid? requestId) => RequestId = requestId;

    public void SetCoHost1(string? name, string? phoneNumber, string? email)
    {
        CoHost1Name = name;
        CoHost1PhoneNumber = phoneNumber;
        CoHost1Email = email;
    }

    public void SetCoHost2(string? name, string? phoneNumber, string? email)
    {
        CoHost2Name = name;
        CoHost2PhoneNumber = phoneNumber;
        CoHost2Email = email;
    }

    public void SetGLNumbers(
        string? refreshments,
        string? hotel,
        string? carHire,
        string? equipment,
        string? languageServices)
    {
        GLNumberRefreshments = refreshments;
        GLNumberHotel = hotel;
        GLNumberCarHire = carHire;
        GLNumberEquipment = equipment;
        GLNumberLanguageServices = languageServices;
    }

    public void SetCostCenterNumbers(
        string? refreshments,
        string? hotel,
        string? carHire,
        string? equipment,
        string? languageServices)
    {
        CostCenterNumberRefreshments = refreshments;
        CostCenterNumberHotel = hotel;
        CostCenterNumberCarHire = carHire;
        CostCenterNumberEquipment = equipment;
        CostCenterNumberLanguageServices = languageServices;
    }
}