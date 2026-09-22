#nullable enable

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using ResourceryPlatformWorkflow.Workflow.Meetings;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class RequestSubmissionValidationService
{
    private const string ValidationCachePrefix = "workflow:request:validation:";
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
        SlidingExpiration = TimeSpan.FromMinutes(10),
    };

    private readonly IDistributedCache _distributedCache;

    public RequestSubmissionValidationService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task ValidateAsync(CreateUpdateRequestDto input, string validationKey)
    {
        if (input == null)
        {
            throw RequestException.RequestDataRequired();
        }

        if (input.ServiceId == Guid.Empty)
        {
            throw RequestException.RequestDataRequired();
        }

        if (input.RequestType == default)
        {
            throw RequestException.RequestDataRequired();
        }

        if (input.RequestType == RequestType.Meeting && input.Meeting == null)
        {
            throw RequestException.MeetingDataRequired();
        }

        ValidateMeeting(input.Meeting, input.ServiceId);

        await CacheValidationAsync(input, validationKey);
    }

    public async Task<CreateUpdateRequestDto?> GetCachedAsync(string validationKey)
    {
        var cacheKey = GetCacheKey(validationKey);
        var cached = await _distributedCache.GetStringAsync(cacheKey);

        if (string.IsNullOrWhiteSpace(cached))
        {
            return null;
        }

        return JsonSerializer.Deserialize<CreateUpdateRequestDto>(cached);
    }

    private async Task CacheValidationAsync(CreateUpdateRequestDto input, string validationKey)
    {
        var payload = new
        {
            RequestType = input.RequestType,
            RequestStatus = input.RequestStatus,
            DocumentSetUrl = input.DocumentSetUrl,
            Description = input.Description,
            Comment = input.Comment,
            ServiceId = input.ServiceId,
            Meeting = input.Meeting,
            Documents = input.Documents,
            ValidatedAtUtc = DateTime.UtcNow,
        };

        var cacheKey = GetCacheKey(validationKey);
        await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(payload), CacheOptions);
    }

    private static string GetCacheKey(string validationKey)
    {
        return $"{ValidationCachePrefix}{(string.IsNullOrWhiteSpace(validationKey) ? Guid.NewGuid().ToString("N") : validationKey)}";
    }

    private static void ValidateMeeting(CreateUpdateMeetingDto? meeting, Guid serviceId)
    {
        if (meeting == null)
        {
            throw RequestException.MeetingDataRequired();
        }

        if (string.IsNullOrWhiteSpace(meeting.Title))
        {
            throw RequestException.MeetingDataRequired();
        }

        if (meeting.StartDate == default || meeting.EndDate == default || meeting.StartDate > meeting.EndDate)
        {
            throw RequestException.MeetingDataRequired();
        }

        if (meeting.NumberOfParticipants <= 0)
        {
            throw RequestException.MeetingDataRequired();
        }

        if (string.IsNullOrWhiteSpace(meeting.Location) || string.IsNullOrWhiteSpace(meeting.ContactName))
        {
            throw RequestException.MeetingDataRequired();
        }

        if (string.IsNullOrWhiteSpace(meeting.ReferenceNumber))
        {
            meeting.ReferenceNumber = GenerateMeetingReferenceNumber(meeting, serviceId);
        }

        if (meeting.MeetingItems == null)
        {
            return;
        }

        foreach (var item in meeting.MeetingItems)
        {
            if (item == null)
            {
                throw RequestException.MeetingItemDataRequired();
            }

            if (string.IsNullOrWhiteSpace(item.ItemName)
                || string.IsNullOrWhiteSpace(item.ItemCode)
                || string.IsNullOrWhiteSpace(item.Category)
                || string.IsNullOrWhiteSpace(item.ServiceCenterCode))
            {
                throw RequestException.MeetingItemDataRequired(item.ItemName ?? "Selected meeting requirement");
            }

            if (item.QuantityNo <= 0)
            {
                throw RequestException.InvalidMeetingRequirement(item.ItemName);
            }

            if (item.PeriodFrom == default || item.PeriodTo == default || item.PeriodFrom > item.PeriodTo)
            {
                throw RequestException.InvalidMeetingRequirement(item.ItemName);
            }

            if (item.Budget <= 0m)
            {
                throw RequestException.InvalidMeetingRequirement(item.ItemName);
            }
        }
    }

    private static string GenerateMeetingReferenceNumber(CreateUpdateMeetingDto meeting, Guid serviceId)
    {
        var code = serviceId == Guid.Empty
            ? meeting.Title.Trim()
            : serviceId.ToString("N");

        var normalized = new string(code.Where(char.IsLetterOrDigit).ToArray());
        var shortCode = normalized.Length <= 8 ? normalized : normalized.Substring(0, 8);
        return $"MEET-{shortCode.ToUpperInvariant()}-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}
