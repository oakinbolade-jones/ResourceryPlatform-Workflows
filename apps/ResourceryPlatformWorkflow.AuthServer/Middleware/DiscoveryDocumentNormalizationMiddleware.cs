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
        // Only process the discovery endpoint
        if (context.Request.Path == "/.well-known/openid-configuration" && context.Request.Method == "GET")
        {
            // Capture the original response body stream
            var originalBodyStream = context.Response.Body;

            using (var memoryStream = new MemoryStream())
            {
                // Replace the response body stream with a memory stream so we can capture it
                context.Response.Body = memoryStream;

                await _next(context);

                // Only modify if the response was successful
                if (context.Response.StatusCode == 200)
                {
                    // Read the response body
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(memoryStream))
                    {
                        var responseBody = await reader.ReadToEndAsync();

                        // Parse the JSON and normalize the issuer
                        try
                        {
                            var jsonNode = JsonNode.Parse(responseBody);
                            if (jsonNode != null && jsonNode["issuer"] != null)
                            {
                                var issuerValue = jsonNode["issuer"].GetValue<string>();
                                if (!string.IsNullOrEmpty(issuerValue) && issuerValue.EndsWith('/'))
                                {
                                    // Remove trailing slash
                                    jsonNode["issuer"] = issuerValue.TrimEnd('/');
                                    responseBody = jsonNode.ToJsonString();
                                }
                            }
                        }
                        catch
                        {
                            // If parsing fails, just use the original response body
                        }

                        // Write the (possibly modified) response to the original stream
                        var responseBytes = Encoding.UTF8.GetBytes(responseBody);
                        await originalBodyStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                }
                else
                {
                    // If not successful, just copy the original response
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(originalBodyStream);
                }
            }

            context.Response.Body = originalBodyStream;
        }
        else
        {
            await _next(context);
        }
    }
}
