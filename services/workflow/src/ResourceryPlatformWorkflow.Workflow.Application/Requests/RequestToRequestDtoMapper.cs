using ResourceryPlatformWorkflow.Workflow.Mapping;
using Riok.Mapperly.Abstractions;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

[Mapper]
public partial class RequestToRequestDtoMapper : MapperBase<Request, RequestDto>
{
    public override partial RequestDto Map(Request source);

    public override partial void Map(Request source, RequestDto destination);
}
