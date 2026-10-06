using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceryPlatformWorkflow.Workflow.Meetings;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class RequestManager : DomainService
{
    private readonly IRepository<Request, Guid> _requestRepository;
    private readonly IRepository<Meeting, Guid> _meetingRepository;

    public RequestManager(
        IRepository<Request, Guid> requestRepository,
        IRepository<Meeting, Guid> meetingRepository)
    {
        _requestRepository = requestRepository;
        _meetingRepository = meetingRepository;
    }

    public virtual Task<Request> CreateAsync(
        string? documentSetUrl,
        string description,
        Guid serviceId,
        RequestType requestType,
        string? comment = null)
    {
        var request = new Request(
            GuidGenerator.Create(),
            documentSetUrl,
            description,
            serviceId,
            requestType,
            comment
        );

        return Task.FromResult(request);
    }

    public virtual Task SetRequestTypeAsync(Request request, RequestType requestType)
    {
        Check.NotNull(request, nameof(request));

        request.SetRequestType(requestType);
        return Task.CompletedTask;
    }

    public virtual Task SetRequestStatusAsync(Request request, RequestStatus requestStatus)
    {
        Check.NotNull(request, nameof(request));

        request.SetRequestStatus(requestStatus);
        return Task.CompletedTask;
    }

    public virtual Task SetDocumentSetUrlAsync(Request request, string documentSetUrl)
    {
        Check.NotNull(request, nameof(request));

        request.SetDocumentSetUrl(documentSetUrl);
        return Task.CompletedTask;
    }

    public virtual Task SetDescriptionAsync(Request request, string description)
    {
        Check.NotNull(request, nameof(request));

        request.SetDescription(description);
        return Task.CompletedTask;
    }

    public virtual Task SetServiceIdAsync(Request request, Guid serviceId)
    {
        Check.NotNull(request, nameof(request));

        request.SetServiceId(serviceId);
        return Task.CompletedTask;
    }

    public virtual Task MarkMigrationPendingAsync(Request request)
    {
        Check.NotNull(request, nameof(request));

        request.SetDocumentMigrationStatus(DocumentMigrationStatus.Pending);
        request.SetDocumentsPublishedAt(null);
        foreach (var document in request.Documents)
        {
            document.MarkMigrationPending();
        }

        return Task.CompletedTask;
    }

    public virtual Task MarkMigrationInProgressAsync(Request request)
    {
        Check.NotNull(request, nameof(request));

        request.SetDocumentMigrationStatus(DocumentMigrationStatus.InProgress);
        return Task.CompletedTask;
    }

    public virtual Task FinalizeMigrationStatusAsync(Request request)
    {
        Check.NotNull(request, nameof(request));

        if (request.Documents.Any(x => x.MigrationStatus == DocumentMigrationStatus.Failed))
        {
            request.SetDocumentMigrationStatus(DocumentMigrationStatus.Failed);
            request.SetDocumentsPublishedAt(null);
            return Task.CompletedTask;
        }

        if (request.Documents.All(x => x.MigrationStatus == DocumentMigrationStatus.Completed))
        {
            request.SetDocumentMigrationStatus(DocumentMigrationStatus.Completed);
            request.SetDocumentsPublishedAt(DateTime.UtcNow);
        }

        return Task.CompletedTask;
    }

    public virtual Task AddDocumentAsync(
        Request request,
        string title,
        string description,
        string documentUrl
    )
    {
        Check.NotNull(request, nameof(request));

        request.AddDocument(GuidGenerator.Create(), title, description, documentUrl);
        return Task.CompletedTask;
    }

    public virtual Task ReplaceDocumentsAsync(
        Request request,
        IEnumerable<(string Title, string Description, string DocumentUrl)> documents
    )
    {
        Check.NotNull(request, nameof(request));
        Check.NotNull(documents, nameof(documents));

        request.Documents.Clear();

        foreach (var document in documents)
        {
            request.AddDocument(GuidGenerator.Create(), document.Title, document.Description, document.DocumentUrl);
        }

        return Task.CompletedTask;
    }

    public virtual Task MarkDocumentMigrationInProgressAsync(RequestDocument document)
    {
        Check.NotNull(document, nameof(document));

        document.MarkMigrationInProgress();
        return Task.CompletedTask;
    }

    public virtual Task MarkDocumentMigrationCompletedAsync(
        RequestDocument document,
        string sharePointDocumentUrl,
        string sharePointItemId
    )
    {
        Check.NotNull(document, nameof(document));

        document.MarkMigrationCompleted(sharePointDocumentUrl, sharePointItemId);
        return Task.CompletedTask;
    }

    public virtual Task MarkDocumentMigrationFailedAsync(RequestDocument document, string error)
    {
        Check.NotNull(document, nameof(document));

        document.MarkMigrationFailed(error);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(Guid id)
    {
        return _requestRepository.DeleteAsync(id, autoSave: true);
    }

    public Task SetCommentAsync(Request request, string? comment)
    {
        request.SetComment(comment);
        return Task.CompletedTask;
    }

    public virtual async Task UpsertMeetingAsync(Request request, Meeting? meeting)
    {
        if (meeting == null)
        {
            if (request.Meeting != null)
            {
                await _meetingRepository.DeleteAsync(request.Meeting, autoSave: false);
                request.SetMeeting(null);
            }

            return;
        }

        if (request.Meeting == null)
        {
            meeting.SetRequestId(request.Id);
            request.SetMeeting(meeting);
            await _meetingRepository.InsertAsync(meeting, autoSave: false);
            return;
        }

        request.Meeting.SetTitle(meeting.Title);
        request.Meeting.SetDepartureDate(meeting.DepartureDate);
        request.Meeting.SetStartDate(meeting.StartDate);
        request.Meeting.SetEndDate(meeting.EndDate);
        request.Meeting.SetType(meeting.Type);
        request.Meeting.SetReferenceNumber(meeting.ReferenceNumber);
        request.Meeting.SetNumberOfParticipants(meeting.NumberOfParticipants);
        request.Meeting.SetLocation(meeting.Location);
        request.Meeting.SetContactPhone(meeting.ContactPhone);
        request.Meeting.SetContactEmail(meeting.ContactEmail);
        request.Meeting.SetContactName(meeting.ContactName);
        request.Meeting.SetHostName(meeting.HostName);
        request.Meeting.SetHostDesignation(meeting.HostDesignation);
        request.Meeting.SetHostPhoneNumber(meeting.HostPhoneNumber);
        request.Meeting.SetHostEmail(meeting.HostEmail);
        request.Meeting.SetRequestId(request.Id);
        request.Meeting.SetCoHost1(
            meeting.CoHost1Name,
            meeting.CoHost1Designation,
            meeting.CoHost1PhoneNumber,
            meeting.CoHost1Email
        );
        request.Meeting.SetCoHost2(
            meeting.CoHost2Name,
            meeting.CoHost2Designation,
            meeting.CoHost2PhoneNumber,
            meeting.CoHost2Email
        );
        request.Meeting.SetGLNumbers(
            meeting.GLNumberRefreshments,
            meeting.GLNumberHotel,
            meeting.GLNumberCarHire,
            meeting.GLNumberEquipment,
            meeting.GLNumberLanguageServices
        );
        request.Meeting.SetCostCenterNumbers(
            meeting.CostCenterNumberRefreshments,
            meeting.CostCenterNumberHotel,
            meeting.CostCenterNumberCarHire,
            meeting.CostCenterNumberEquipment,
            meeting.CostCenterNumberLanguageServices
        );

        request.Meeting.MeetingItems.Clear();
        foreach (var item in meeting.MeetingItems)
        {
            request.Meeting.MeetingItems.Add(item);
        }

        await _meetingRepository.UpdateAsync(request.Meeting, autoSave: false);
    }
}
