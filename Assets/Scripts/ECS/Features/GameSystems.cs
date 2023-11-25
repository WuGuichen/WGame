
public class GameSystems : Feature
{
    public GameSystems(Contexts contexts)
    {
        // Add(new HandleDebugLogMessageSystem(contexts));
        Add(new EmitInputSystem(contexts));
        Add(new MultiDestroySystem(contexts));
    }
}
