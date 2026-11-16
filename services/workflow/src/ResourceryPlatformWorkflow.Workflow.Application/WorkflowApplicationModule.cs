using System;
using Azure;
using Azure.AI.Translation.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ResourceryPlatformWorkflow.Workflow.Requests;
using ResourceryPlatformWorkflow.Workflow.Translations;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ResourceryPlatformWorkflow.Workflow.Translations;
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

          Configure<AzureTranslatorOptions>(
            configuration.GetSection(
                AzureTranslatorOptions.SectionName
            )
        );

        context.Services.AddHttpClient<
            ITranslationService,
            AzureTranslatorService>((serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<AzureTranslatorOptions>>()
                .Value;

            client.BaseAddress = new Uri(
                options.Endpoint.TrimEnd('/') + "/"
            );

            client.Timeout = TimeSpan.FromMinutes(2);


        });
    }
}
