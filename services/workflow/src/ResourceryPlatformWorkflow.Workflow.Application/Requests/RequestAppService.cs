using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

[Authorize(WorkflowPermissions.Requests.Default)]
public class RequestAppService : WorkflowAppService, IRequestAppService
{
    private readonly IRepository<Request, Guid> _requestRepository;
    private readonly RequestManager _requestManager;
    private readonly RequestToRequestDtoMapper _requestToRequestDtoMapper;
    private readonly IBackgroundJobManager _backgroundJobManager;

    public RequestAppService(
        IRepository<Request, Guid> requestRepository,
        RequestManager requestManager,
        RequestToRequestDtoMapper requestToRequestDtoMapper,
        IBackgroundJobManager backgroundJobManager
    )
    {
        _requestRepository = requestRepository;
        _requestManager = requestManager;
        _requestToRequestDtoMapper = requestToRequestDtoMapper;
        _backgroundJobManager = backgroundJobManager;
    }

    public async Task<RequestDto> GetAsync(Guid id)
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents);
        var request = await AsyncExecuter.FirstOrDefaultAsync(queryable, x => x.Id == id);

        if (request == null)
        {
            throw RequestException.NotFound(id);
        }

        return _requestToRequestDtoMapper.Map(request);
    }

    public async Task<List<RequestDto>> GetListAsync()
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents);
        var requests = await AsyncExecuter.ToListAsync(queryable);
        return requests.ConvertAll(_requestToRequestDtoMapper.Map);
    }

    public async Task<List<RequestDto>> GetByStatusAsync(RequestStatus requestStatus)
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents);
        var requests = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.RequestStatus == requestStatus)
        );
        return requests.ConvertAll(_requestToRequestDtoMapper.Map);
    }

    public async Task<List<RequestDto>> GetByUserAsync(Guid userId)
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents);
        var requests = await AsyncExecuter.ToListAsync(queryable.Where(x => x.CreatorId == userId));
        return requests.ConvertAll(_requestToRequestDtoMapper.Map);
    }

    public async Task<List<RequestDto>> GetByTypeAsync(RequestType requestType)
    {
        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents);
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
            input.RequestType
        );
        await _requestManager.SetRequestStatusAsync(request, input.RequestStatus);

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

        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents);
        var request = await AsyncExecuter.FirstOrDefaultAsync(queryable, x => x.Id == id);

        if (request == null)
        {
            throw RequestException.NotFound(id);
        }

        var previousStatus = request.RequestStatus;

        await _requestManager.SetDocumentSetUrlAsync(request, input.DocumentSetUrl);
        await _requestManager.SetDescriptionAsync(request, input.Description);
        await _requestManager.SetRequestTypeAsync(request, input.RequestType);
        await _requestManager.SetRequestStatusAsync(request, input.RequestStatus);

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

        var queryable = await _requestRepository.WithDetailsAsync(x => x.Documents);
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
    public Task DeleteAsync(Guid id) => _requestRepository.DeleteAsync(id, autoSave: true);

    private Task EnqueuePublishToSharePointAsync(Guid requestId) =>
        _backgroundJobManager.EnqueueAsync(new PublishRequestDocumentsJobArgs { RequestId = requestId });
}
