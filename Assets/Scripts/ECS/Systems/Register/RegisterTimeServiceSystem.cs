using Entitas;

public class RegisterTimeServiceSystem : IInitializeSystem
{
    private readonly MetaContext _metaContext;
    private readonly ITimeService _timeService;
    public RegisterTimeServiceSystem(Contexts contexts, ITimeService service)
    {
        _metaContext = contexts.meta;
        _timeService = service;
    }
    public void Initialize()
    {
        _metaContext.ReplaceTimeService(_timeService);
    }
}
