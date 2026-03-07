using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ResourceryPlatformWorkflow.Middleware;

public class PostLogoutRedirectUriNormalizationMiddleware(
    RequestDelegate next,
    IConfiguration configuration
)
{
    private static readonly HashSet<PathString> LogoutPaths = new()
    {
        new PathString("/connect/endsession"),
        new PathString("/connect/logout"),
    };

    private const string PostLogoutRedirectUriKey = "post_logout_redirect_uri";

    private readonly RequestDelegate _next = next;
    private readonly HashSet<string> _allowedPostLogoutRedirectUris = BuildAllowedPostLogoutRedirectUris(
        configuration
    );
    private readonly string _defaultPostLogoutRedirectUri = NormalizePostLogoutRedirectUri(
        configuration["App:ClientUrl"]
    );

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsLogoutRequest(context.Request.Path))
        {
            var requestedPostLogoutRedirectUri = string.Empty;
            if (
                context.Request.Query.TryGetValue(
                    PostLogoutRedirectUriKey,
                    out var postLogoutRedirectUris
                )
                && !StringValues.IsNullOrEmpty(postLogoutRedirectUris)
            )
            {
                requestedPostLogoutRedirectUri = NormalizePostLogoutRedirectUri(
                    postLogoutRedirectUris[0]
                );
            }

            var fallbackRedirectUri = string.IsNullOrWhiteSpace(_defaultPostLogoutRedirectUri)
                ? string.Empty
                : _defaultPostLogoutRedirectUri;
            var targetRedirectUri =
                !string.IsNullOrWhiteSpace(requestedPostLogoutRedirectUri)
                && _allowedPostLogoutRedirectUris.Contains(requestedPostLogoutRedirectUri)
                    ? requestedPostLogoutRedirectUri
                    : fallbackRedirectUri;

            if (!string.IsNullOrWhiteSpace(targetRedirectUri))
            {
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                context.Response.Redirect(targetRedirectUri);
                return;
            }

            if (
                context.Request.Query.TryGetValue(
                    PostLogoutRedirectUriKey,
                    out var queryPostLogoutRedirectUris
                )
                && !StringValues.IsNullOrEmpty(queryPostLogoutRedirectUris)
            )
            {
                // OpenIddict requires exact per-client redirect URI registration.
                // If fallback redirect cannot be determined, remove the unsupported value.
                var query = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
                var queryBuilder = new QueryBuilder();

                foreach (var queryItem in query)
                {
                    if (queryItem.Key.Equals(PostLogoutRedirectUriKey, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    foreach (var queryValue in queryItem.Value)
                    {
                        queryBuilder.Add(queryItem.Key, queryValue);
                    }
                }

                context.Request.QueryString = queryBuilder.ToQueryString();
            }
        }

        await _next(context);
    }

    private static bool IsLogoutRequest(PathString path)
    {
        foreach (var logoutPath in LogoutPaths)
        {
            if (path.Equals(logoutPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string NormalizePostLogoutRedirectUri(string postLogoutRedirectUri)
    {
        if (
            string.IsNullOrWhiteSpace(postLogoutRedirectUri)
            || !Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri)
        )
        {
            return postLogoutRedirectUri;
        }

        var authority = uri.IsDefaultPort
            ? $"{uri.Scheme}://{uri.Host}"
            : $"{uri.Scheme}://{uri.Host}:{uri.Port}";

        var normalizedPath = uri.AbsolutePath;
        if (normalizedPath == "/")
        {
            normalizedPath = string.Empty;
        }
        else if (normalizedPath.Length > 1 && normalizedPath.EndsWith("/", StringComparison.Ordinal))
        {
            normalizedPath = normalizedPath.TrimEnd('/');
        }

        return authority + normalizedPath + uri.Query + uri.Fragment;
    }

    private static HashSet<string> BuildAllowedPostLogoutRedirectUris(IConfiguration configuration)
    {
        var allowedUrls = new HashSet<string>(StringComparer.Ordinal);

        var clientUrl = NormalizePostLogoutRedirectUri(configuration["App:ClientUrl"]);
        if (!string.IsNullOrWhiteSpace(clientUrl))
        {
            allowedUrls.Add(clientUrl);
        }

        var redirectAllowedUrls = configuration["App:RedirectAllowedUrls"];
        if (string.IsNullOrWhiteSpace(redirectAllowedUrls))
        {
            return allowedUrls;
        }

        var parts = redirectAllowedUrls.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var normalized = NormalizePostLogoutRedirectUri(part.Trim());
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                allowedUrls.Add(normalized);
            }
        }

        return allowedUrls;
    }
}
