using Entitas;

public class DrawSensorCharacterSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> charSensorGroup;
    public DrawSensorCharacterSystem(Contexts contexts)
    {
        charSensorGroup = contexts.sensor.GetGroup(SensorMatcher.AllOf(SensorMatcher.SensorCharOpen));
    }
    public void Execute()
    {
        foreach (var sensor in charSensorGroup)
        {
            if (sensor.hasDetectorCharacterService)
            {
                if(EntityUtils.IsNetCamera(sensor.linkCharacter.Character))
                    continue;
                sensor.detectorCharacterService.service.UpdateSensorDrawer();
            }
        }
    }
}
