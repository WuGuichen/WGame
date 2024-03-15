using Entitas;

public class MoveTriggerObjectSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> _triggerObjectGroup;
    public MoveTriggerObjectSystem(Contexts contexts)
    {
        _triggerObjectGroup = contexts.sensor.GetGroup(SensorMatcher.TriggerObjectSensor);
    }
    public void Execute()
    {
        foreach (var sensor in _triggerObjectGroup)
        {
            var obj = sensor.triggerObjectSensor.obj;
        }
    }
}
