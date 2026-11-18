using System;
using Microsoft.Extensions.DependencyInjection;
using ResourceryPlatformWorkflow.Workflow.Requests;
using ResourceryPlatformWorkflow.Workflow.Translations;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;
using SmartServe.Workflow.Translations;
namespace ResourceryPlatformWorkflow.Workflow;

[DependsOn(typeof(WorkflowDomainModule))]
[DependsOn(typeof(WorkflowApplicationContractsModule))]
[DependsOn(typeof(AbpDddApplicationModule))]
[DependsOn(typeof(AbpBackgroundJobsAbstractionsModule))]
[DependsOn(typeof(AbpAutoMapperModule))]
public class WorkflowApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
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
                throw new InvalidOperationException(
                    "AzureTranslator Endpoint is not configured. Set Azure__Translator__Endpoint or AZURETRANSLATOR_ENDPOINT."
                );
            }

            client.BaseAddress = new Uri(azureTranslatorOptions.Endpoint.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        context.Services.AddHttpClient<ISummarizationService, AzureLanguageService>((_, client) =>
        {
            if (string.IsNullOrWhiteSpace(azureLanguageServiceOptions.Endpoint))
            {
                throw new InvalidOperationException(
                    "AzureLanguageService Endpoint is not configured. Set Azure__LanguageService__Endpoint or AZURELANGUAGESERVICE_ENDPOINT."
                );
            }

            client.BaseAddress = new Uri(azureLanguageServiceOptions.Endpoint.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromMinutes(2);
        });
    }
}
