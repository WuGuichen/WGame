using Entitas;
using WGame.Runtime;

public class RegisterResourcesServiceSystem : IInitializeSystem, ITearDownSystem
{
    private readonly MetaContext _metaContext;
    private readonly IAssetService _service;
    public RegisterResourcesServiceSystem(Contexts contexts, IAssetService service)
    {
        _metaContext = contexts.meta;
        _service = service;
    }
    public void Initialize()
    {
        // _metaContext.ReplaceResourcesService(_service);
    }

    public void TearDown()
    {
        // _service.D();
    }
}
