using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Mailing;

[DependsOn(typeof(MailingDomainSharedModule))]
[DependsOn(typeof(AbpDddApplicationContractsModule))]
[DependsOn(typeof(AbpAuthorizationModule))]
public class MailingApplicationContractsModule : AbpModule { }
