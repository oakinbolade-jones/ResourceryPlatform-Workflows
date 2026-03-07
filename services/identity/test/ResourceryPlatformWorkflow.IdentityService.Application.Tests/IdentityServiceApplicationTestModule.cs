using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.IdentityService;

[DependsOn(typeof(IdentityServiceApplicationModule))]
[DependsOn(typeof(IdentityServiceDomainTestModule))]
public class IdentityServiceApplicationTestModule : AbpModule { }
