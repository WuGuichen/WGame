public class SensorDetectSystems : Feature
{
    public SensorDetectSystems(Contexts contexts)
    {
        // Add(new CharacterDetectSystem(contexts));
        Add(new GroundSensorDetectSystem(contexts));
        Add(new DropItemDetectSystem(contexts));
        Add(new SensorCharacterSystem(contexts));
        Add(new DetectCharacterSystem(contexts));
        // Add(new DrawDetectCharacterSystem(contexts));
        Add(new DrawSensorCharacterSystem(contexts));

        Add(new UpdateTriggerObjectSystem(contexts));
        Add(new MoveTriggerObjectSystem(contexts));
        Add(new DrawTriggerObjectSystem(contexts));
    }
}
