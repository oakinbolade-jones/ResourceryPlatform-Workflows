using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ResourceryPlatformWorkflow.Workflow.Meetings;

public class Meeting : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public string Title { get; private set; }
    public DateTime DepartureDate { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public MeetingType Type { get; private set; }
    public string ReferenceNumber { get; private set; }
    public int NumberOfParticipants { get; private set; }
    public string Location { get; private set; }
    public string ContactPhone { get; private set; }
    public string ContactEmail { get; private set; }
    public string ContactName { get; private set; }
    public string HostName { get; private set; }
    public string HostPhoneNumber { get; private set; }
    public string HostEmail { get; private set; }
    public string? CoHost1Name { get; private set; }
    public string? CoHost1PhoneNumber { get; private set; }
    public string? CoHost1Email { get; private set; }
    public string? CoHost2Name { get; private set; }
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