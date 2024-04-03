using System.Collections.Generic;
using WGame.Ability;

public class AbilityServiceImplementation : IAbility
{
    private readonly GameEntity _entity;
    private readonly EventOwner _owner;
    private SensorContext _sensorContext;
    private bool _isProcessAbility = false;
    public EventOwner Owner => _owner;
    public bool IsProcessAbility => _isProcessAbility;

    private LinkedList<AbilityStatusCharacter> _abilityStatusList = new();

    private BuffManager _buffManager;
    public BuffManager BuffManager => _buffManager;

    public AbilityServiceImplementation(GameEntity entity)
    {
        _entity = entity;
        _owner = new EventOwnerEntity(entity);
        _sensorContext = Contexts.sharedInstance.sensor;
        _buffManager = new BuffManager(new BuffOwnerEntity(_entity, this));
    }
    
    public bool Do(string name, bool unique = false)
    {
        return false;
    }

    public bool SwitchMotionAbility(int id)
    {
        return true;
    }

    public void Process(float deltaTime)
    {
        var node = _abilityStatusList.First;
        _isProcessAbility = (node != null);
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

        if (_entity.groundSensor.intersect)
        {
            _entity.characterState.state.EnableState(AStateType.IsOnGround);
        }
        _entity.characterState.state.CheckStateChange();
    }

    public void GenEntity(EntityMoveInfo info)
    {
        var sensor = _sensorContext.CreateEntity();
        // 单向绑定
        sensor.AddLinkCharacter(_entity);
        var sphereTrigger = TriggerObject.GenSphere(sensor, _entity.gameViewService.service.HeadPos, 2f);
        sphereTrigger.AttachEffect("HCFX_ElementOrb_03", 0.7f);
        sensor.AddTriggerObjectSensor(sphereTrigger);
        sensor.AddMoveDirection(_entity.gameViewService.service.Model.forward);
        sensor.AddMoveInfo(info);
        sensor.AddLifeTime(8f);
    }
}
