using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace ResourceryPlatformWorkflow.Workflow;

[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(AbpPermissionManagementDomainModule))]
[DependsOn(typeof(WorkflowDomainSharedModule))]
public class WorkflowDomainModule : AbpModule { }
