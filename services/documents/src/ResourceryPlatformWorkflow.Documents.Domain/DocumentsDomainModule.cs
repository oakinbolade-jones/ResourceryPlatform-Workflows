using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Documents;

[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(DocumentsDomainSharedModule))]
public class DocumentsDomainModule : AbpModule { }
