using Entitas;

public class DrawDetectCharacterSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> charDetectorGroup;
    public DrawDetectCharacterSystem(Contexts contexts)
    {
        charDetectorGroup = contexts.sensor.GetGroup(SensorMatcher.AllOf(SensorMatcher.DetectCharOpen));
    }
    public void Execute()
    {
        foreach (var sensor in charDetectorGroup)
        {
            if (sensor.hasSensorCharacterService)
            {
                sensor.sensorCharacterService.service.UpdateDetectorDrawer();
            }
        }
    }
}
