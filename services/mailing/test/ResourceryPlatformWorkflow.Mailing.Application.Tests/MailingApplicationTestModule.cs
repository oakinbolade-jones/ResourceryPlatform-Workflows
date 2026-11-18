using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Mailing;

[DependsOn(typeof(MailingApplicationModule))]
[DependsOn(typeof(MailingDomainTestModule))]
public class MailingApplicationTestModule : AbpModule { }
