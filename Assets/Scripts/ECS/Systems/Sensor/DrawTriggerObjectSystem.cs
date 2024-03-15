using Entitas;

public class DrawTriggerObjectSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> _triggerObjectGroup;
    
    public DrawTriggerObjectSystem(Contexts contexts)
    {
        _triggerObjectGroup = contexts.sensor.GetGroup(SensorMatcher.TriggerObjectSensor);
    }
    
    public void Execute()
    {
        foreach (var sensor in _triggerObjectGroup)
        {
            sensor.triggerObjectSensor.obj.Draw();
        }
    }
}
