using System.Collections.Generic;
using WGame.Ability;
using WGame.Utils;

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
    private AbilityEntity _ability;
    public BuffManager BuffManager => _buffManager;

    private IGotHitService _normalStateGotHit;
    private bool _isChangingGotHit = false;

    public AbilityServiceImplementation(GameEntity entity, AbilityEntity ability)
    {
        _entity = entity;
        _ability = ability;
        _owner = new EventOwnerEntity(entity);
        _sensorContext = Contexts.sharedInstance.sensor;
        _buffManager = new BuffManager(new BuffOwnerEntity(_entity, this));
        entity.characterState.state.onStateEnable += OnStateEnable;
        entity.characterState.state.onStateDisable += OnStateDisable;
    }
    
    private void OnStateEnable(int mask)
    {
        if ((mask & AStateType.Invincible) != 0)
        {
            if (_isChangingGotHit)
            {
                WLogger.Error("不允许同时改变受击方式");
            }
            if (_ability.hasAbilityGotHit)
            {
                _normalStateGotHit = _ability.abilityGotHit.service;
                _ability.ReplaceAbilityGotHit(GotHitImpls.Invincible);
            }
            else{
                _normalStateGotHit = null;
                _ability.AddAbilityGotHit(GotHitImpls.Invincible);
            }

            _isChangingGotHit = true;
        }
    }

    private void OnStateDisable(int mask)
    {
        if ((mask & AStateType.Invincible) != 0)
        {
            if (_normalStateGotHit != null)
            {
                _ability.ReplaceAbilityGotHit(_normalStateGotHit);
            }
            else
            {
                _ability.RemoveAbilityGotHit();
            }
            _isChangingGotHit = false;
        }
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

    public void Initialize()
    {
        
    }

    public void Destroy()
    {
        _entity.characterState.state.onStateEnable -= OnStateEnable;
        _entity.characterState.state.onStateDisable -= OnStateDisable;
    }
}
