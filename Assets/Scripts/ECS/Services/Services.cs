using WGame.Runtime;

public class Services
{
    public readonly IInputService Input;
    public readonly ICameraService Camera;
    public readonly IAssetService Resources;
    public readonly IFactoryService Factory;
    public readonly ITimeService Time;

    public Services(IInputService input, ICameraService camera, IAssetService resources, IFactoryService factory, ITimeService time)
    {
        Input = input;
        Camera = camera;
        Resources = resources;
        Factory = factory;
        Time = time;
    }
}
