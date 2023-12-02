public class SensorDetectSystems : Feature
{
    public SensorDetectSystems(Contexts contexts)
    {
        // Add(new CharacterDetectSystem(contexts));
        Add(new GroundSensorDetectSystem(contexts));
        Add(new DropItemDetectSystem(contexts));
        Add(new DrawDetectCharacterSystem(contexts));
        Add(new DrawSensorCharacterSystem(contexts));
    }
}
