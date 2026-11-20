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

        context.Services.AddAutoMapperObjectMapper<WorkflowApplicationModule>();
        context.Services.AddTransient<RequestSubmissionValidationService>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WorkflowApplicationModule>(true);
        });

        var azureTranslatorOptions = AzureEnvironmentOptions.Resolve<AzureTranslatorOptions>(
            AzureTranslatorOptions.SectionName,
            configuration
        );
        azureTranslatorOptions.Endpoint = AzureEnvironmentOptions.GetValue(
            configuration,
            AzureTranslatorOptions.SectionName,
            nameof(AzureTranslatorOptions.Endpoint)
        );
        azureTranslatorOptions.Key = AzureEnvironmentOptions.GetValue(
            configuration,
            AzureTranslatorOptions.SectionName,
            nameof(AzureTranslatorOptions.Key)
        );
        azureTranslatorOptions.Region = AzureEnvironmentOptions.GetValue(
            configuration,
            AzureTranslatorOptions.SectionName,
            nameof(AzureTranslatorOptions.Region)
        );

        context.Services.Configure<AzureTranslatorOptions>(options =>
        {
            options.Endpoint = azureTranslatorOptions.Endpoint;
            options.Key = azureTranslatorOptions.Key;
            options.Region = azureTranslatorOptions.Region;
            options.MaxBatchSize = azureTranslatorOptions.MaxBatchSize;
            options.MaxBatchCharacters = azureTranslatorOptions.MaxBatchCharacters;
        });

        var azureLanguageServiceOptions = AzureEnvironmentOptions.Resolve<AzureLanguageServiceOptions>(
            AzureLanguageServiceOptions.SectionName,
            configuration
        );
        azureLanguageServiceOptions.Endpoint = AzureEnvironmentOptions.GetValue(
            configuration,
            AzureLanguageServiceOptions.SectionName,
            nameof(AzureLanguageServiceOptions.Endpoint)
        );
        azureLanguageServiceOptions.Key1 = AzureEnvironmentOptions.GetValue(
            configuration,
            AzureLanguageServiceOptions.SectionName,
            nameof(AzureLanguageServiceOptions.Key1)
        );
        azureLanguageServiceOptions.Key2 = AzureEnvironmentOptions.GetValue(
            configuration,
            AzureLanguageServiceOptions.SectionName,
            nameof(AzureLanguageServiceOptions.Key2)
        );
        azureLanguageServiceOptions.Key = AzureEnvironmentOptions.GetValue(
            configuration,
            AzureLanguageServiceOptions.SectionName,
            nameof(AzureLanguageServiceOptions.Key)
        );
        if (string.IsNullOrWhiteSpace(azureLanguageServiceOptions.Key) ||
            azureLanguageServiceOptions.Key == "__SET_VIA_ENVIRONMENT_VARIABLE__")
        {
            azureLanguageServiceOptions.Key = !string.IsNullOrWhiteSpace(azureLanguageServiceOptions.Key1) &&
                                             azureLanguageServiceOptions.Key1 != "__SET_VIA_ENVIRONMENT_VARIABLE__"
                ? azureLanguageServiceOptions.Key1
                : azureLanguageServiceOptions.Key2;
        }

        context.Services.Configure<AzureLanguageServiceOptions>(options =>
        {
            options.Endpoint = azureLanguageServiceOptions.Endpoint;
            options.Key = azureLanguageServiceOptions.Key;
            options.Key1 = azureLanguageServiceOptions.Key1;
            options.Key2 = azureLanguageServiceOptions.Key2;
            options.DefaultSentenceCount = azureLanguageServiceOptions.DefaultSentenceCount;
            options.PollingTimeoutSeconds = azureLanguageServiceOptions.PollingTimeoutSeconds;
        });

        var azureKeyVaultOptions = AzureEnvironmentOptions.Resolve<AzureKeyVaultOptions>(
            AzureKeyVaultOptions.SectionName,
            configuration
        );
        azureKeyVaultOptions.VaultUri = AzureEnvironmentOptions.GetValue(
            configuration,
            AzureKeyVaultOptions.SectionName,
            nameof(AzureKeyVaultOptions.VaultUri)
        );
        context.Services.Configure<AzureKeyVaultOptions>(options =>
        {
            options.VaultUri = azureKeyVaultOptions.VaultUri;
        });

        context.Services.AddHttpClient<ITranslationService, AzureTranslatorService>((_, client) =>
        {
            if (string.IsNullOrWhiteSpace(azureTranslatorOptions.Endpoint))
            {
                azureTranslatorOptions.Endpoint = DefaultAzureTranslatorEndpoint;
            }

            client.BaseAddress = ResolveTranslatorBaseAddress(azureTranslatorOptions.Endpoint);
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        context.Services.AddHttpClient<ISummarizationService, AzureLanguageService>((_, client) =>
        {
            if (string.IsNullOrWhiteSpace(azureLanguageServiceOptions.Endpoint))
            {
                throw new InvalidOperationException(
                    "AzureLanguageService Endpoint is not configured. Set AZURE__LANGUAGESERVICES__ENDPOINT."
                );
            }

            client.BaseAddress = new Uri(azureLanguageServiceOptions.Endpoint.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        Configure<AzureLanguageOptions>(
            configuration.GetSection(AzureLanguageOptions.SectionName)
        );
        Configure<AzureLanguageOptions>(options =>
        {
            options.Key = ResolveFromEnvironment(
                options.Key,
                "AZURE__LANGUAGESERVICES__KEY1",
                "AZURE__LANGUAGESERVICES__KEY2"
            );
            options.Endpoint = ResolveFromEnvironment(
                options.Endpoint,
                "AZURE__LANGUAGESERVICES__ENDPOINT"
            );
            options.Region = ResolveFromEnvironment(
                options.Region,
                "AZURE__LANGUAGESERVICES__REGION"
            );
        });

        context.Services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<AzureLanguageOptions>>()
                .Value;

            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                throw new InvalidOperationException("Azure Language Endpoint is not configured.");
            }

            if (string.IsNullOrWhiteSpace(options.Key))
            {
                throw new InvalidOperationException("Azure Language Key is not configured.");
            }

            return new TextAnalyticsClient(
                new Uri(options.Endpoint),
                new AzureKeyCredential(options.Key)
            );
        });

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
                $"Invalid Azure Translator endpoint '{endpoint}'. Configure AZURE__LANGUAGESERVICES__TRANSLATOR__ENDPOINT as an absolute URL."
            );
        }

        return new Uri(baseUri.ToString().TrimEnd('/') + "/");
    }
}
