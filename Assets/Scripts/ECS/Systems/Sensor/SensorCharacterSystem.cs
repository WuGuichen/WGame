using Entitas;

public class SensorCharacterSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> charSensorGroup;
    public SensorCharacterSystem(Contexts contexts)
    {
        charSensorGroup = contexts.sensor.GetGroup(SensorMatcher.AllOf(SensorMatcher.SensorCharOpen));
    }
    public void Execute()
    {
        foreach (var sensor in charSensorGroup)
        {
            if (sensor.hasSensorCharacterService)
            {
                sensor.sensorCharacterService.service.UpdateSensor();
            }
        }
    }
}
