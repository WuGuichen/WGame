public class LateFixedUpdateSystems : Feature
{
    public LateFixedUpdateSystems(Contexts contexts)
    {
		Add(new CameraFollowTargetSystem(contexts));
		Add(new CameraRotateSystem(contexts));
    }
}
