using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ResourceryPlatformWorkflow.Workflow.Meetings;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class RequestSubmissionValidationService_Tests
{
    [Fact]
    public async Task ValidateAsync_ShouldRejectMissingRequestData()
    {
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var validator = new RequestSubmissionValidationService(cache);

        var exception = await Should.ThrowAsync<BusinessException>(() =>
            validator.ValidateAsync(new CreateUpdateRequestDto { ServiceId = Guid.Empty, RequestType = RequestType.Meeting }, "test-request"));

        exception.Code.ShouldBe(WorkflowErrorCodes.Requests.RequestDataRequired);
    }

    [Fact]
    public async Task ValidateAsync_ShouldRejectMissingMeetingDataForMeetingRequest()
    {
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var validator = new RequestSubmissionValidationService(cache);

        var request = new CreateUpdateRequestDto
        {
            ServiceId = Guid.NewGuid(),
            RequestType = RequestType.Meeting,
            Description = "Quarterly meeting",
            Meeting = null,
            Documents = new List<CreateUpdateRequestDocumentDto>()
        };

        var exception = await Should.ThrowAsync<BusinessException>(() =>
            validator.ValidateAsync(request, "test-request"));

        exception.Code.ShouldBe(WorkflowErrorCodes.Requests.MeetingDataRequired);
    }

    [Fact]
    public async Task ValidateAsync_ShouldAllowMeetingWhenNoRequirementItemIsSelected()
    {
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var validator = new RequestSubmissionValidationService(cache);

        var request = new CreateUpdateRequestDto
        {
            ServiceId = Guid.NewGuid(),
            RequestType = RequestType.Meeting,
            Description = "Quarterly meeting",
            Meeting = new CreateUpdateMeetingDto
            {
                Title = "Quarterly Review",
                DepartureDate = DateTime.UtcNow.AddDays(-1),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Type = MeetingType.Physical,
                NumberOfParticipants = 20,
                Location = "Abuja",
                ContactPhone = "+2348000000000",
                ContactEmail = "contact@example.com",
                ContactName = "Contact Person",
                HostName = "Host Name",
                HostPhoneNumber = "+2348000000001",
                HostEmail = "host@example.com",
                MeetingItems = new List<CreateUpdateMeetingItemDto>()
            },
            Documents = new List<CreateUpdateRequestDocumentDto>()
        };

        await Should.NotThrowAsync(() => validator.ValidateAsync(request, "test-request"));
    }

    [Fact]
    public async Task ValidateAsync_ShouldRejectInvalidSelectedRequirementWhenMeetingItemDetailsAreMissing()
    {
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var validator = new RequestSubmissionValidationService(cache);

        var request = new CreateUpdateRequestDto
        {
            ServiceId = Guid.NewGuid(),
            RequestType = RequestType.Meeting,
            Description = "Quarterly meeting",
            Meeting = new CreateUpdateMeetingDto
            {
                Title = "Quarterly Review",
                DepartureDate = DateTime.UtcNow.AddDays(-1),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Type = MeetingType.Physical,
                NumberOfParticipants = 20,
                Location = "Abuja",
                ContactPhone = "+2348000000000",
                ContactEmail = "contact@example.com",
                ContactName = "Contact Person",
                HostName = "Host Name",
                HostPhoneNumber = "+2348000000001",
                HostEmail = "host@example.com",
                MeetingItems = new List<CreateUpdateMeetingItemDto>
                {
                    new()
                    {
                        ItemName = "English Interpreter",
                        ItemCode = "LANG-ENG-INT",
                        Category = "Language Service",
                        ServiceCenterCode = "LANG",
                        QuantityNo = 0,
                        PeriodFrom = default,
                        PeriodTo = default,
                        Budget = 0m,
                        RemarkObservation = string.Empty
                    }
                }
            },
            Documents = new List<CreateUpdateRequestDocumentDto>()
        };

        var exception = await Should.ThrowAsync<BusinessException>(() =>
            validator.ValidateAsync(request, "test-request"));

        exception.Code.ShouldBe(WorkflowErrorCodes.Requests.InvalidMeetingRequirement);
        exception.Data["itemName"].ShouldBe("English Interpreter");
    }

    [Fact]
    public async Task ValidateAsync_ShouldRejectInvalidSelectedLanguageServiceRequirement()
    {
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var validator = new RequestSubmissionValidationService(cache);

        var request = new CreateUpdateRequestDto
        {
            ServiceId = Guid.NewGuid(),
            RequestType = RequestType.Meeting,
            Description = "Quarterly meeting",
            Meeting = new CreateUpdateMeetingDto
            {
                Title = "Quarterly Review",
                DepartureDate = DateTime.UtcNow.AddDays(-1),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Type = MeetingType.Physical,
                NumberOfParticipants = 20,
                Location = "Abuja",
                ContactPhone = "+2348000000000",
                ContactEmail = "contact@example.com",
                ContactName = "Contact Person",
                HostName = "Host Name",
                HostPhoneNumber = "+2348000000001",
                HostEmail = "host@example.com",
                MeetingItems = new List<CreateUpdateMeetingItemDto>
                {
                    new()
                    {
                        ItemName = "English Interpreter",
                        ItemCode = "LANG-ENG-INT",
                        Category = "Language Service",
                        ServiceCenterCode = "LANG",
                        QuantityNo = 0,
                        PeriodFrom = default,
                        PeriodTo = default,
                        Budget = 0m,
                        RemarkObservation = string.Empty
                    },
                    new()
                    {
                        ItemName = "French Interpreter",
                        ItemCode = "LANG-FRA-INT",
                        Category = "Language Service",
                        ServiceCenterCode = "LANG",
                        QuantityNo = 2,
                        PeriodFrom = DateTime.UtcNow,
                        PeriodTo = DateTime.UtcNow.AddDays(3),
                        Budget = 1500m,
                        RemarkObservation = string.Empty
                    }
                }
            },
            Documents = new List<CreateUpdateRequestDocumentDto>()
        };

        var exception = await Should.ThrowAsync<BusinessException>(() =>
            validator.ValidateAsync(request, "test-request"));

        exception.Code.ShouldBe(WorkflowErrorCodes.Requests.InvalidMeetingRequirement);
        exception.Data["itemName"].ShouldBe("English Interpreter");
    }

    [Fact]
    public async Task ValidateAsync_ShouldGenerateMeetingReferenceNumberWhenMissing()
    {
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var validator = new RequestSubmissionValidationService(cache);

        var meeting = new CreateUpdateMeetingDto
        {
            Title = "Quarterly Review",
            DepartureDate = DateTime.UtcNow.AddDays(-1),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Type = MeetingType.Physical,
            ReferenceNumber = "",
            NumberOfParticipants = 20,
            Location = "Abuja",
            ContactPhone = "+2348000000000",
            ContactEmail = "contact@example.com",
            ContactName = "Contact Person",
            HostName = "Host Name",
            HostPhoneNumber = "+2348000000001",
            HostEmail = "host@example.com",
            MeetingItems = new List<CreateUpdateMeetingItemDto>
            {
                new()
                {
                    ItemName = "Meeting Hall",
                    ItemCode = "MEET-VEN-MH",
                    Category = "Venue Requirement",
                    ServiceCenterCode = "CONF",
                    QuantityNo = 1,
                    PeriodFrom = DateTime.UtcNow,
                    PeriodTo = DateTime.UtcNow.AddDays(1),
                    Budget = 1000m,
                    RemarkObservation = "Needs projector"
                }
            }
        };

        var request = new CreateUpdateRequestDto
        {
            ServiceId = Guid.NewGuid(),
            RequestType = RequestType.Meeting,
            Description = "Quarterly meeting",
            Meeting = meeting,
            Documents = new List<CreateUpdateRequestDocumentDto>()
        };

        await validator.ValidateAsync(request, "test-request");

        meeting.ReferenceNumber.ShouldNotBeNullOrWhiteSpace();
        meeting.ReferenceNumber.ShouldStartWith("MEET-");
    }

    [Fact]
    public async Task ValidateAsync_ShouldCacheValidationState()
    {
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var validator = new RequestSubmissionValidationService(cache);

        var request = new CreateUpdateRequestDto
        {
            ServiceId = Guid.NewGuid(),
            RequestType = RequestType.Meeting,
            Description = "Quarterly meeting",
            Meeting = new CreateUpdateMeetingDto
            {
                Title = "Quarterly Review",
                DepartureDate = DateTime.UtcNow.AddDays(-1),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Type = MeetingType.Physical,
                ReferenceNumber = "REF-001",
                NumberOfParticipants = 20,
                Location = "Abuja",
                ContactPhone = "+2348000000000",
                ContactEmail = "contact@example.com",
                ContactName = "Contact Person",
                HostName = "Host Name",
                HostPhoneNumber = "+2348000000001",
                HostEmail = "host@example.com",
                MeetingItems = new List<CreateUpdateMeetingItemDto>
                {
                    new()
                    {
                        ItemName = "Meeting Hall",
                        ItemCode = "MH-01",
                        Category = "Venue Requirement",
                        ServiceCenterCode = "SC-01",
                        QuantityNo = 1,
                        PeriodFrom = DateTime.UtcNow,
                        PeriodTo = DateTime.UtcNow.AddDays(1),
                        Budget = 1000m,
                        RemarkObservation = "Needs projector"
                    }
                }
            },
            Documents = new List<CreateUpdateRequestDocumentDto>()
        };

        await validator.ValidateAsync(request, "test-request");

        var cached = await cache.GetStringAsync("workflow:request:validation:test-request");
        cached.ShouldNotBeNullOrEmpty();
    }
}
