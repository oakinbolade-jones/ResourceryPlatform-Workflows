using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

namespace ResourceryPlatformWorkflow.Middleware;

public class ExternalProfileSynchronizationMiddleware(
    RequestDelegate next,
    ILogger<ExternalProfileSynchronizationMiddleware> logger
)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExternalProfileSynchronizationMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, IdentityUserManager userManager)
    {
        var principal = context.User;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var userId = principal.FindFirstValue(AbpClaimTypes.UserId)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
        {
            await _next(context);
            return;
        }

        var givenName = principal.FindFirstValue(AbpClaimTypes.Name)
            ?? principal.FindFirstValue(ClaimTypes.GivenName)
            ?? principal.FindFirstValue("given_name");

        var surname = principal.FindFirstValue(AbpClaimTypes.SurName)
            ?? principal.FindFirstValue(ClaimTypes.Surname)
            ?? principal.FindFirstValue("family_name");

        var picture = principal.FindFirstValue("picture")
            ?? principal.FindFirstValue("photo");

        if (string.IsNullOrWhiteSpace(givenName)
            && string.IsNullOrWhiteSpace(surname)
            && string.IsNullOrWhiteSpace(picture))
        {
            await _next(context);
            return;
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var hasChanges = false;
            var nameUpdated = false;
            var surnameUpdated = false;

            if (!string.IsNullOrWhiteSpace(givenName) && !string.Equals(user.Name, givenName, StringComparison.Ordinal))
            {
                user.Name = givenName;
                hasChanges = true;
                nameUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(surname) && !string.Equals(user.Surname, surname, StringComparison.Ordinal))
            {
                user.Surname = surname;
                hasChanges = true;
                surnameUpdated = true;
            }

            if (hasChanges)
            {
                await userManager.UpdateAsync(user);
                _logger.LogInformation(
                    "External profile synchronized for user {UserId}. NameUpdated: {NameUpdated}, SurnameUpdated: {SurnameUpdated}",
                    userId,
                    nameUpdated,
                    surnameUpdated
                );
            }

            if (!string.IsNullOrWhiteSpace(picture))
            {
                var userClaims = await userManager.GetClaimsAsync(user);
                var existingPictureClaim = userClaims.FirstOrDefault(c => c.Type == "picture");

                if (existingPictureClaim == null)
                {
                    await userManager.AddClaimAsync(user, new Claim("picture", picture));
                    _logger.LogInformation(
                        "External profile picture claim added for user {UserId}",
                        userId
                    );
                }
                else if (!string.Equals(existingPictureClaim.Value, picture, StringComparison.Ordinal))
                {
                    await userManager.ReplaceClaimAsync(
                        user,
                        existingPictureClaim,
                        new Claim("picture", picture)
                    );
                    _logger.LogInformation(
                        "External profile picture claim updated for user {UserId}",
                        userId
                    );
                }
            }
        }

        await _next(context);
    }
}
