using Entitas;

public class UpdateTriggerObjectSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> _triggerObjectGroup;
    private readonly IGroup<SensorEntity> _lifeTimeGroup;
    private readonly ITimeService _time;
    
    public UpdateTriggerObjectSystem(Contexts contexts)
    {
        _triggerObjectGroup = contexts.sensor.GetGroup(SensorMatcher.TriggerObjectSensor);
        _lifeTimeGroup = contexts.sensor.GetGroup(SensorMatcher.LifeTime);
        _time = contexts.meta.timeService.instance;
    }
    
    public void Execute()
    {
        foreach (var sensor in _lifeTimeGroup)
        {
            var leftTime = sensor.lifeTime.value;
            if (!sensor.isDestroyed && leftTime <= 0.0f)
            {
                sensor.isDestroyed = true;
            }
            else
            {
                leftTime -= _time.DeltaTime(1f);
                sensor.ReplaceLifeTime(leftTime);
            }
        }
        
        foreach (var sensor in _triggerObjectGroup)
        {
            var triggerObject = sensor.triggerObjectSensor.obj;
            var num = triggerObject.ProcessDetect();
            for (int i = 0; i < num; i++)
            {
                var targetId = triggerObject.GetTarget(i);
                var parts = triggerObject.GetPart(i);
                var target = EntityUtils.GetGameEntity(targetId);
                if (target.hasLinkAbility)
                {
                    target.linkAbility.Ability.abilityGotHit.service.OnGotHit(target, sensor, parts);
                }
            }
        }
    }
}
