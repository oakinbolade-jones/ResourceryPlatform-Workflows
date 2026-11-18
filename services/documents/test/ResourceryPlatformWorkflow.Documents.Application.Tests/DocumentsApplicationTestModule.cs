using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Documents;

[DependsOn(typeof(DocumentsApplicationModule))]
[DependsOn(typeof(DocumentsDomainTestModule))]
public class DocumentsApplicationTestModule : AbpModule { }
