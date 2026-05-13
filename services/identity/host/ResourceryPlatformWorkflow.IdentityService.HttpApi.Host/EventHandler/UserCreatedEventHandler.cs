using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.IdentityService.EventHandler;

public class UserCreatedEventHandler(
    ICurrentTenant currentTenant,
    IdentityUserManager userManager,
    ILogger<UserCreatedEventHandler> logger
) : ILocalEventHandler<EntityCreatedEventData<IdentityUser>>, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IdentityUserManager _userManager = userManager;
    private readonly ILogger<UserCreatedEventHandler> _logger = logger;

    public async Task HandleEventAsync(EntityCreatedEventData<IdentityUser> eventData)
    {
        var user = eventData.Entity;

        try
        {
            _logger.LogInformation(
                "Assigning workflow permissions to newly created user {UserId}...",
                user.Id
            );

            using (_currentTenant.Change(user.TenantId))
            {
                var requestsRoleResult = await _userManager.AddToRoleAsync(user, "Requests");
                if (!requestsRoleResult.Succeeded)
                {
                    _logger.LogWarning(
                        "Failed to add user {UserId} to Requests role. Errors: {Errors}",
                        user.Id,
                        string.Join(", ", requestsRoleResult.Errors)
                    );
                }

                var transcriptionsRoleResult = await _userManager.AddToRoleAsync(
                    user,
                    "Transcriptions"
                );
                if (!transcriptionsRoleResult.Succeeded)
                {
                    _logger.LogWarning(
                        "Failed to add user {UserId} to Transcriptions role. Errors: {Errors}",
                        user.Id,
                        string.Join(", ", transcriptionsRoleResult.Errors)
                    );
                }

                _logger.LogInformation(
                    "Finished role assignment for user {UserId}",
                    user.Id
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error assigning Requests/Transcriptions roles to user {UserId}",
                user.Id
            );
            throw;
        }
    }
}
