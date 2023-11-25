public class ProcessMotionSystems : Feature
{
    public ProcessMotionSystems(Contexts contexts)
    {
        // Add(new ProcessSignalSystem(contexts));
        Add(new ProcessMotionEndSystem(contexts));
        Add(new ProcessMotionStartSystem(contexts));
        Add(new ProcessMotionSignalUpdateSystem(contexts));
        Add(new ProcessMotionUpdateSystem(contexts));
        // Add(new ProcessWeaponStateSystem(contexts));
        // Add(new ProcessWeaponInfoSystem(contexts));
        Add(new ProcessWeaponAttackSystem(contexts));
    }
}
