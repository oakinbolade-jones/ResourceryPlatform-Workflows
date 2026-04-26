using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Meetings;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

[Authorize(WorkflowPermissions.Requests.Default)]
public class RequestAppService : WorkflowAppService, IRequestAppService
{
    private readonly IRepository<Request, Guid> _requestRepository;
    private readonly IRepository<Meeting, Guid> _meetingRepository;
    private readonly RequestManager _requestManager;
    private readonly RequestToRequestDtoMapper _requestToRequestDtoMapper;
    private readonly IBackgroundJobManager _backgroundJobManager;

    public RequestAppService(
        IRepository<Request, Guid> requestRepository,
        IRepository<Meeting, Guid> meetingRepository,
        RequestManager requestManager,
        RequestToRequestDtoMapper requestToRequestDtoMapper,
        IBackgroundJobManager backgroundJobManager
    )
    {
        _requestRepository = requestRepository;
        _meetingRepository = meetingRepository;
        _requestManager = requestManager;
        _requestToRequestDtoMapper = requestToRequestDtoMapper;
        _backgroundJobManager = backgroundJobManager;
    }

    public async Task<RequestDto> GetAsync(Guid id)
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents, x => x.MeetingForm);
        var request = await AsyncExecuter.FirstOrDefaultAsync(queryable, x => x.Id == id);

        if (request == null)
        {
            throw RequestException.NotFound(id);
        }

        return _requestToRequestDtoMapper.Map(request);
    }

    public async Task<List<RequestDto>> GetListAsync()
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents, x => x.MeetingForm);
        var requests = await AsyncExecuter.ToListAsync(queryable);
        return requests.ConvertAll(_requestToRequestDtoMapper.Map);
    }

    public async Task<List<RequestDto>> GetByStatusAsync(RequestStatus requestStatus)
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents, x => x.MeetingForm);
        var requests = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.RequestStatus == requestStatus)
        );
        return requests.ConvertAll(_requestToRequestDtoMapper.Map);
    }

    public async Task<List<RequestDto>> GetByUserAsync(Guid userId)
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents, x => x.MeetingForm);
        var requests = await AsyncExecuter.ToListAsync(queryable.Where(x => x.CreatorId == userId));
        return requests.ConvertAll(_requestToRequestDtoMapper.Map);
    }

    public async Task<List<RequestDto>> GetByTypeAsync(RequestType requestType)
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents, x => x.MeetingForm);
        var requests = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.RequestType == requestType)
        );
        return requests.ConvertAll(_requestToRequestDtoMapper.Map);
    }

    [Authorize(WorkflowPermissions.Requests.Create)]
    public async Task<RequestDto> CreateAsync(CreateUpdateRequestDto input)
    {
        Check.NotNull(input, nameof(input));

        var request = await _requestManager.CreateAsync(
            input.DocumentSetUrl,
            input.Description,
            input.ServiceId,
            input.RequestType,
            input.Comment
        );

        await _requestManager.SetRequestStatusAsync(request, input.RequestStatus);
        await UpsertMeetingFormAsync(request, input.MeetingForm);

        foreach (var document in input.Documents)
        {
            await _requestManager.AddDocumentAsync(
                request,
                document.Title,
                document.Description,
                document.DocumentUrl
            );
        }

        if (request.RequestStatus == RequestStatus.Completed)
        {
            await _requestManager.MarkMigrationPendingAsync(request);
        }

        request = await _requestRepository.InsertAsync(request, autoSave: true);

        if (request.RequestStatus == RequestStatus.Completed)
        {
            await EnqueuePublishToSharePointAsync(request.Id);
        }

        return _requestToRequestDtoMapper.Map(request);
    }

    [Authorize(WorkflowPermissions.Requests.Update)]
    public async Task<RequestDto> UpdateAsync(Guid id, CreateUpdateRequestDto input)
    {
        Check.NotNull(input, nameof(input));

        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents, x => x.MeetingForm);
        var request = await AsyncExecuter.FirstOrDefaultAsync(queryable, x => x.Id == id);

        if (request == null)
        {
            throw RequestException.NotFound(id);
        }

        var previousStatus = request.RequestStatus;

        await _requestManager.SetDocumentSetUrlAsync(request, input.DocumentSetUrl);
        await _requestManager.SetDescriptionAsync(request, input.Description);
        await _requestManager.SetServiceIdAsync(request, input.ServiceId);
        await _requestManager.SetRequestTypeAsync(request, input.RequestType);
        await _requestManager.SetCommentAsync(request, input.Comment);
        await _requestManager.SetRequestStatusAsync(request, input.RequestStatus);
        await UpsertMeetingFormAsync(request, input.MeetingForm);

        await _requestManager.ReplaceDocumentsAsync(
            request,
            input.Documents.Select(x => (x.Title, x.Description, x.DocumentUrl))
        );

        if (request.RequestStatus == RequestStatus.Completed)
        {
            await _requestManager.MarkMigrationPendingAsync(request);
        }

        request = await _requestRepository.UpdateAsync(request, autoSave: true);

        if (previousStatus != RequestStatus.Completed && request.RequestStatus == RequestStatus.Completed)
        {
            await EnqueuePublishToSharePointAsync(request.Id);
        }

        return _requestToRequestDtoMapper.Map(request);
    }

    [Authorize(WorkflowPermissions.RequestDocuments.Create)]
    public async Task<RequestDto> AddDocumentsAsync(
        Guid id,
        List<CreateUpdateRequestDocumentDto> documents
    )
    {
        Check.NotNull(documents, nameof(documents));

        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents, x => x.MeetingForm);
        var request = await AsyncExecuter.FirstOrDefaultAsync(queryable, x => x.Id == id);

        if (request == null)
        {
            throw RequestException.NotFound(id);
        }

        foreach (var document in documents)
        {
            Check.NotNull(document, nameof(document));

            await _requestManager.AddDocumentAsync(
                request,
                document.Title,
                document.Description,
                document.DocumentUrl
            );
        }

        if (request.RequestStatus == RequestStatus.Completed)
        {
            await _requestManager.MarkMigrationPendingAsync(request);
        }

        request = await _requestRepository.UpdateAsync(request, autoSave: true);

        if (request.RequestStatus == RequestStatus.Completed)
        {
            await EnqueuePublishToSharePointAsync(request.Id);
        }

        return _requestToRequestDtoMapper.Map(request);
    }

    [Authorize(WorkflowPermissions.Requests.Delete)]
    public Task DeleteAsync(Guid id) => _requestManager.DeleteAsync(id);

    private async Task UpsertMeetingFormAsync(Request request, CreateUpdateMeetingDto? input)
    {
        if (input == null)
        {
            if (request.MeetingForm != null)
            {
                await _meetingRepository.DeleteAsync(request.MeetingForm, autoSave: false);
                request.SetMeetingForm(null);
            }

            return;
        }

        if (request.MeetingForm == null)
        {
            var meeting = new Meeting(
                GuidGenerator.Create(),
                input.Title,
                input.DepartureDate,
                input.StartDate,
                input.EndDate,
                input.Type,
                input.ReferenceNumber,
                input.NumberOfParticipants,
                input.Location,
                input.ContactPhone,
                input.ContactEmail,
                input.ContactName,
                input.HostName,
                input.HostPhoneNumber,
                input.HostEmail
            );

            meeting.SetRequestId(request.Id);
            meeting.SetCoHost1(input.CoHost1Name, input.CoHost1PhoneNumber, input.CoHost1Email);
            meeting.SetCoHost2(input.CoHost2Name, input.CoHost2PhoneNumber, input.CoHost2Email);
            meeting.SetGLNumbers(
                input.GLNumberRefreshments,
                input.GLNumberHotel,
                input.GLNumberCarHire,
                input.GLNumberEquipment,
                input.GLNumberLanguageServices
            );
            meeting.SetCostCenterNumbers(
                input.CostCenterNumberRefreshments,
                input.CostCenterNumberHotel,
                input.CostCenterNumberCarHire,
                input.CostCenterNumberEquipment,
                input.CostCenterNumberLanguageServices
            );

            request.SetMeetingForm(meeting);
            await _meetingRepository.InsertAsync(meeting, autoSave: false);
            return;
        }

        request.MeetingForm.SetTitle(input.Title);
        request.MeetingForm.SetDepartureDate(input.DepartureDate);
        request.MeetingForm.SetStartDate(input.StartDate);
        request.MeetingForm.SetEndDate(input.EndDate);
        request.MeetingForm.SetType(input.Type);
        request.MeetingForm.SetReferenceNumber(input.ReferenceNumber);
        request.MeetingForm.SetNumberOfParticipants(input.NumberOfParticipants);
        request.MeetingForm.SetLocation(input.Location);
        request.MeetingForm.SetContactPhone(input.ContactPhone);
        request.MeetingForm.SetContactEmail(input.ContactEmail);
        request.MeetingForm.SetContactName(input.ContactName);
        request.MeetingForm.SetHostName(input.HostName);
        request.MeetingForm.SetHostPhoneNumber(input.HostPhoneNumber);
        request.MeetingForm.SetHostEmail(input.HostEmail);
        request.MeetingForm.SetRequestId(request.Id);
        request.MeetingForm.SetCoHost1(input.CoHost1Name, input.CoHost1PhoneNumber, input.CoHost1Email);
        request.MeetingForm.SetCoHost2(input.CoHost2Name, input.CoHost2PhoneNumber, input.CoHost2Email);
        request.MeetingForm.SetGLNumbers(
            input.GLNumberRefreshments,
            input.GLNumberHotel,
            input.GLNumberCarHire,
            input.GLNumberEquipment,
            input.GLNumberLanguageServices
        );
        request.MeetingForm.SetCostCenterNumbers(
            input.CostCenterNumberRefreshments,
            input.CostCenterNumberHotel,
            input.CostCenterNumberCarHire,
            input.CostCenterNumberEquipment,
            input.CostCenterNumberLanguageServices
        );

        await _meetingRepository.UpdateAsync(request.MeetingForm, autoSave: false);
    }

    private Task EnqueuePublishToSharePointAsync(Guid requestId) =>
        _backgroundJobManager.EnqueueAsync(new PublishRequestDocumentsJobArgs { RequestId = requestId });
}
