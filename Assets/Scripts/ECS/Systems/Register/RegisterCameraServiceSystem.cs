using Entitas;

public class RegisterCameraServiceSystem : IInitializeSystem
{
    private readonly ICameraService _cameraService;
    private readonly MetaContext _metaContext;
    public RegisterCameraServiceSystem(Contexts contexts, ICameraService service)
    {
        _metaContext = contexts.meta;
        _cameraService = service;
    }

    public void Initialize()
    {
        _metaContext.ReplaceMainCameraService(_cameraService);
    }
}
