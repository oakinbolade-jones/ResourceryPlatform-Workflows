using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Documents;

[DependsOn(typeof(DocumentsDomainSharedModule))]
[DependsOn(typeof(AbpDddApplicationContractsModule))]
[DependsOn(typeof(AbpAuthorizationModule))]
public class DocumentsApplicationContractsModule : AbpModule { }
