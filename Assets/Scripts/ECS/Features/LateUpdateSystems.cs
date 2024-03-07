public class LateUpdateSystems : Feature
{
    public LateUpdateSystems(Contexts _contexts)
    {
		Add(new DeadCharacterSystem(_contexts));
    }
}
