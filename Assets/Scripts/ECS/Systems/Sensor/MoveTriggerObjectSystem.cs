using Entitas;

public class MoveTriggerObjectSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> _triggerObjectGroup;
    private readonly ITimeService _time;
    public MoveTriggerObjectSystem(Contexts contexts)
    {
        _triggerObjectGroup = contexts.sensor.GetGroup(SensorMatcher.TriggerObjectSensor);
        _time = contexts.meta.timeService.instance;
    }
    public void Execute()
    {
        foreach (var sensor in _triggerObjectGroup)
        {
            var obj = sensor.triggerObjectSensor.obj;
            var moveInfo = sensor.moveInfo.value;
            obj.Translate(_time.FixedDeltaTime * sensor.moveDirection.value * moveInfo.Speed);
        }
    }
}
