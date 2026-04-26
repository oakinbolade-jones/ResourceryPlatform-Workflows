using ResourceryPlatformWorkflow.Workflow.Mapping;
using Riok.Mapperly.Abstractions;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

[Mapper]
public partial class RequestToCreateUpdateDtoMapper : MapperBase<Request, CreateUpdateRequestDto>
{
    public override partial CreateUpdateRequestDto Map(Request source);

    public override partial void Map(Request source, CreateUpdateRequestDto destination);
}
