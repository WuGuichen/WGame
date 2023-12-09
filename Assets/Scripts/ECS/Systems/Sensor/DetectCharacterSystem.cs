using Entitas;

public class DetectCharacterSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> charDetectorGroup;
    public DetectCharacterSystem(Contexts contexts)
    {
        charDetectorGroup = contexts.sensor.GetGroup(SensorMatcher.AllOf(SensorMatcher.DetectCharOpen));
    }
    public void Execute()
    {
        foreach (var sensor in charDetectorGroup)
        {
            if (sensor.hasSensorCharacterService)
            {
                sensor.sensorCharacterService.service.UpdateDetect();
            }
        }
    }
}
