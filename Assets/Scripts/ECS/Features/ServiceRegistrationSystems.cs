
public class ServiceRegistrationSystems : Feature
{
    public ServiceRegistrationSystems(Contexts contexts, Services services)
    {
        Add(new RegisterInputServiceSystem(contexts, services.Input));
        Add(new RegisterCameraServiceSystem(contexts, services.Camera));
        Add(new RegisterResourcesServiceSystem(contexts, services.Resources));
        Add(new RegisterFactoryServiceSystem(contexts, services.Factory));
        Add(new RegisterTimeServiceSystem(contexts, services.Time));
    }
}
