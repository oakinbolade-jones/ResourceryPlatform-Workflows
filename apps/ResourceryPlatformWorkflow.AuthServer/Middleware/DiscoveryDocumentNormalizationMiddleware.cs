using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ResourceryPlatformWorkflow.Middleware;

/// <summary>
/// Middleware to normalize the OpenID Connect discovery document by removing trailing slashes
/// from the issuer value to ensure consistency with client expectations.
/// </summary>
public class DiscoveryDocumentNormalizationMiddleware
{
    private readonly RequestDelegate _next;

    public DiscoveryDocumentNormalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.TrimEnd('/');

        // Only process the discovery endpoint
        if (string.Equals(path, "/.well-known/openid-configuration", StringComparison.OrdinalIgnoreCase)
            && string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
        {
            var originalBodyStream = context.Response.Body;

            await using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                try
                {
                    await _next(context);

                    if (context.Response.StatusCode == 200)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(memoryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                        {
                            var responseBody = await reader.ReadToEndAsync();

                            try
                            {
                                var jsonNode = JsonNode.Parse(responseBody);
                                if (jsonNode != null && jsonNode["issuer"] != null)
                                {
                                    var issuerValue = jsonNode["issuer"].GetValue<string>();
                                    if (!string.IsNullOrEmpty(issuerValue) && issuerValue.EndsWith('/'))
                                    {
                                        jsonNode["issuer"] = issuerValue.TrimEnd('/');
                                        responseBody = jsonNode.ToJsonString();
                                    }
                                }
                            }
                            catch
                            {
                                // If parsing fails, just use the original response body.
                            }

                            var responseBytes = Encoding.UTF8.GetBytes(responseBody);
                            context.Response.ContentLength = responseBytes.Length;
                            await originalBodyStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                            await originalBodyStream.FlushAsync();
                        }
                    }
                    else
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        context.Response.ContentLength = memoryStream.Length;
                        await memoryStream.CopyToAsync(originalBodyStream);
                    }
                }
                finally
                {
                    context.Response.Body = originalBodyStream;
                }
            }
        }
        else
        {
            await _next(context);
        }
    }
}
