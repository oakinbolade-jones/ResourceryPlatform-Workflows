namespace ResourceryPlatformWorkflow.Workflow.Mapping;

public abstract class MapperBase<TSource, TDest> : Volo.Abp.DependencyInjection.ITransientDependency
{
    public abstract TDest Map(TSource source);
    public abstract void Map(TSource source, TDest destination);
}
