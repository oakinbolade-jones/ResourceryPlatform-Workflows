
using System;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ResourceryPlatformWorkflow.Workflow.Requests;
using ResourceryPlatformWorkflow.Workflow.Summarization;
using ResourceryPlatformWorkflow.Workflow.Translations;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;
namespace ResourceryPlatformWorkflow.Workflow;

[DependsOn(typeof(WorkflowDomainModule))]
[DependsOn(typeof(WorkflowApplicationContractsModule))]
[DependsOn(typeof(AbpDddApplicationModule))]
[DependsOn(typeof(AbpBackgroundJobsAbstractionsModule))]
[DependsOn(typeof(AbpAutoMapperModule))]
public class WorkflowApplicationModule : AbpModule
{
    private const string DefaultAzureTranslatorEndpoint = "https://api.cognitive.microsofttranslator.com/";

    public override void ConfigureServices(
        ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        // AutoMapper configuration
        context.Services.AddAutoMapperObjectMapper<WorkflowApplicationModule>();

        context.Services.AddTransient<RequestSubmissionValidationService>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WorkflowApplicationModule>(true);
        });

        // ==========================================
        // AZURE TRANSLATOR CONFIGURATION
        // ==========================================

        Configure<AzureTranslatorOptions>(
            configuration.GetSection(
                AzureTranslatorOptions.SectionName
            )
        );

        Configure<AzureTranslatorOptions>(options =>
        {
            options.Key = ResolveFromEnvironment(
                options.Key,
                "AZURETRANSLATOR_KEY1",
                "AZURETRANSLATOR_KEY2"
            );
            options.Endpoint = ResolveFromEnvironment(
                options.Endpoint,
                "AZURETRANSLATOR_ENDPOINT"
            );
            options.Region = ResolveFromEnvironment(
                options.Region,
                "AZURETRANSLATOR_REGION"
            );

            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                options.Endpoint = DefaultAzureTranslatorEndpoint;
            }
        });

        context.Services.AddHttpClient<
            ITranslationService,
            AzureTranslatorService>((serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<AzureTranslatorOptions>>()
                .Value;

            client.BaseAddress = ResolveTranslatorBaseAddress(options.Endpoint);

            client.Timeout = TimeSpan.FromMinutes(2);
        });

      
        // Bind Azure Language configuration
        Configure<AzureLanguageOptions>(
            configuration.GetSection(
                AzureLanguageOptions.SectionName
            )
        );

        Configure<AzureLanguageOptions>(options =>
        {
            options.Key = ResolveFromEnvironment(
                options.Key,
                "AZURELANGUAGE_KEY1",
                "AZURELANGUAGE_KEY2"
            );
            options.Endpoint = ResolveFromEnvironment(
                options.Endpoint,
                "AZURELANGUAGE_ENDPOINT"
            );
            options.Region = ResolveFromEnvironment(
                options.Region,
                "AZURELANGUAGE_REGION"
            );
        });


        // Register Azure Language client
        context.Services.AddSingleton(sp =>
        {
            var options = sp
                .GetRequiredService<IOptions<AzureLanguageOptions>>()
                .Value;

            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                throw new InvalidOperationException(
                    "Azure Language Endpoint is not configured.");
            }

            if (string.IsNullOrWhiteSpace(options.Key))
            {
                throw new InvalidOperationException(
                    "Azure Language Key is not configured.");
            }

            return new TextAnalyticsClient(
                new Uri(options.Endpoint),
                new AzureKeyCredential(options.Key)
            );
        });

        // Register summarization service
        context.Services.AddTransient<
                IAzureLanguageSummarizationService,
                AzureLanguageSummarizationService
            >();
    }

    private static string ResolveFromEnvironment(
        string fallbackValue,
        params string[] variableNames)
    {
        foreach (var variableName in variableNames)
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return fallbackValue;
    }

    private static Uri ResolveTranslatorBaseAddress(string endpoint)
    {
        var normalizedEndpoint = (endpoint ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalizedEndpoint))
        {
            normalizedEndpoint = DefaultAzureTranslatorEndpoint;
        }

        if (!normalizedEndpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !normalizedEndpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            normalizedEndpoint = $"https://{normalizedEndpoint}";
        }

        if (!Uri.TryCreate(normalizedEndpoint, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException(
                $"Invalid Azure Translator endpoint '{endpoint}'. Configure AZURETRANSLATOR_ENDPOINT as an absolute URL."
            );
        }

        return new Uri(baseUri.ToString().TrimEnd('/') + "/");
    }
}
