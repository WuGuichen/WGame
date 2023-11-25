using Entitas;

public class RegisterFactoryServiceSystem : IInitializeSystem, ITearDownSystem
{
    private readonly MetaContext _metaContext;
    private readonly IFactoryService _factoryService;
    public RegisterFactoryServiceSystem(Contexts contexts, IFactoryService service)
    {
        _metaContext = contexts.meta;
        _factoryService = service;
    }

    public void Initialize()
    {
        _metaContext.ReplaceFactoryService(_factoryService);
    }

    public void TearDown()
    {
        _factoryService.Dispose();
    }
}
