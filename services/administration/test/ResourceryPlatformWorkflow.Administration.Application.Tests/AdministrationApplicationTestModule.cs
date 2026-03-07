using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Administration;

[DependsOn(typeof(AdministrationApplicationModule))]
[DependsOn(typeof(AdministrationDomainTestModule))]
public class AdministrationApplicationTestModule : AbpModule { }
