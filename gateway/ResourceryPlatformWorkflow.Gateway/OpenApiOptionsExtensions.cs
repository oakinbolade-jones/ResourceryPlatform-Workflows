using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ResourceryPlatformWorkflow.Gateway;

public static class OpenApiOptionsExtensions
{
    public static OpenApiOptions UseJwtBearerAuthentication(this OpenApiOptions options)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
        };

        options.AddDocumentTransformer(
            (document, context, cancellationToken) =>
            {
                document.Components ??= new();
                document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = scheme;

                return Task.CompletedTask;
            }
        );

        options.AddOperationTransformer(
            (operation, context, cancellationToken) =>
            {
                if (
                    context
                        .Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>()
                        .Any()
                )
                {
                    // In OpenAPI 2.0+, reference the scheme by ID
                    var schemeReference = new OpenApiSecuritySchemeReference(
                        JwtBearerDefaults.AuthenticationScheme,
                        null,
                        null
                    );
                    operation.Security = [new OpenApiSecurityRequirement { [schemeReference] = [] }];
                }

                return Task.CompletedTask;
            }
        );

        return options;
    }
}
