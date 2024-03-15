using System.Collections.Generic;
using UnityEngine;
using UnityTimer;
using WGame.Ability;

public class AbilityServiceImplementation : IAbility
{
    private readonly GameEntity _entity;
    private AbilityData _curAbility;
    private SensorContext _sensorContext;

    private LinkedList<AbilityStatusCharacter> _abilityStatusList = new();

    public AbilityServiceImplementation(GameEntity entity)
    {
        _entity = entity;
        _sensorContext = Contexts.sharedInstance.sensor;
    }
    
    public void Do(string name)
    {
        var ability = WAbilityMgr.Inst.GetAbility(name);
        if (ability != null)
        {
            var status = AbilityStatusCharacter.Get(_entity, ability);
            _abilityStatusList.AddLast(status);
        }
    }

    public void Process(float deltaTime)
    {
        var node = _abilityStatusList.First;
        while (node != null)
        {
            var status = node.Value;
            if (status.IsEnable)
            {
                status.Process(deltaTime);
                node = node.Next;
            }
            else
            {
                var next = node.Next;
                _abilityStatusList.Remove(node);
                AbilityStatusCharacter.Push(node.Value);
                node = next;
            }
        }
    }

    public void GenEntity(AbilityEntityInfo info)
    {
        WLogger.Print("生成技能实体");
        var sensor = _sensorContext.CreateEntity();
        // 单向绑定
        sensor.AddLinkCharacter(_entity);
        var sphereTrigger = TriggerObject.GenSphere(sensor, _entity.gameViewService.service.HeadPos, 2f);
        sensor.AddTriggerObjectSensor(sphereTrigger);
        Timer.Register(5f, () =>
        {
            sensor.isDestroyed = true;
        });
    }
}
