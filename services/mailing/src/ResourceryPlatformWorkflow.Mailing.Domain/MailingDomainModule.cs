using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Mailing;

[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(MailingDomainSharedModule))]
public class MailingDomainModule : AbpModule { }
