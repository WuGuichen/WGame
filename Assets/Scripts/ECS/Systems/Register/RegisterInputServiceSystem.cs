using Entitas;
using UnityEngine;

public class RegisterInputServiceSystem : IInitializeSystem
{
    private readonly IInputService _inputService;
    private readonly MetaContext _metaContext;
    public RegisterInputServiceSystem(Contexts contexts, IInputService service)
    {
        _metaContext = contexts.meta;
        _inputService = service;
    }
    public void Initialize()
    {
        _metaContext.ReplaceInputService(_inputService);
    }
}
