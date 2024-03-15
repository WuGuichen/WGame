using Entitas;
using UnityEngine;

public class UpdateTriggerObjectSystem : IExecuteSystem
{
    private readonly IGroup<SensorEntity> _triggerObjectGroup;
    
    public UpdateTriggerObjectSystem(Contexts contexts)
    {
        _triggerObjectGroup = contexts.sensor.GetGroup(SensorMatcher.TriggerObjectSensor);
    }
    
    public void Execute()
    {
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
                    target.linkAbility.Ability.abilityGotHit.service.OnGotHit(sensor, parts);
                }
            }
        }
    }
}
