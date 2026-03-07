using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Workflow;

[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(WorkflowDomainSharedModule))]
public class WorkflowDomainModule : AbpModule { }
