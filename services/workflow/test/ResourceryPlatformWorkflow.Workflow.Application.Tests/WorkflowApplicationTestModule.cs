using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Workflow;

[DependsOn(typeof(WorkflowApplicationModule))]
[DependsOn(typeof(WorkflowDomainTestModule))]
public class WorkflowApplicationTestModule : AbpModule { }
