using Entitas;
using UnityEngine;
using WGame.Ability;
using WGame.Trigger;

public class UpdateDeviceInputSignalSystem : IExecuteSystem
{
    private readonly InputContext _inputContext;
    private readonly GameContext _gameContext;
    private readonly IGroup<GameEntity> _cameraGroup;
    
    public UpdateDeviceInputSignalSystem(Contexts contexts)
    {
        _inputContext = contexts.input;
        _gameContext = contexts.game;
        _cameraGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Camera, GameMatcher.GameViewService));
    }
    
    public void Execute()
    {
        // var entities = _gameContext.GetEntities();
        foreach (var entity in _cameraGroup)
        {
            HandleInputSignal(entity);
        }
    }

    void HandleInputSignal(GameEntity entity)
    {
        var inputState = entity.inputState.state;
        if (_inputContext.runInput.value)
        {
            inputState.EnableState(InputType.Jump);
        }
        if (_inputContext.runInput.value && entity.linkMotion.Motion.motionStart.UID == entity.linkMotion.Motion.motionLocalMotion.UID)
        {
            // 跑
            entity.isRunState = true;
        }
        else
        {
            entity.isRunState = false;
        }
        
        if (_inputContext.jumpInput.value)
        {
            inputState.EnableState(InputType.Jump);
            // 跳
            // 这里会有预输入
            // entity.ReplaceSignalJump(1f);
        }

        if (_inputContext.defense.value)
        {
            // 防御
            // entity.isPrepareDefenseState = true;
            entity.ReplaceSignalDefense(0.1f);
            inputState.EnableState(InputType.Defense);
        }

        if (_inputContext.attackHoldInput.value)
        {
            // 蓄力攻击
            WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Attack, InputSignalEvent.IsHold, new WTrigger.Context(entity.entityID.id));
            entity.isPrepareHoldAttackState = true;
            inputState.EnableState(InputType.HoldAttack);
        }
        else
        {
            if(entity.isPrepareHoldAttackState)
                WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Attack,InputSignalEvent.WasReleased, new WTrigger.Context(entity.entityID.id));
            entity.isPrepareHoldAttackState = false;
        }

        if (_inputContext.attackInput.value)
        {
            inputState.EnableState(InputType.Attack);
            if (entity.hasLinkAbility)
            {
                var ability = entity.linkAbility.Ability;
                if (ability.hasAbilityBackStab)
                {
                    ActionHelper.DoFinishAttack(ability);
                }
                else
                {
                    entity.ReplaceSignalAttack(0.2f);
                }
            }
            else
            {
                entity.ReplaceSignalAttack(0.2f);
            }
            // 攻击
        }

        if (_inputContext.stepInput.value)
        {
            inputState.EnableState(InputType.Step);
            entity.ReplaceSignalStep(0.2f);
        }

        if (_inputContext.special.value)
        {
            inputState.EnableState(InputType.Special);
            // entity.linkAbility.Ability.abilityService.service.Do("FireBall");
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.H))
        {
        }
        inputState.CheckStateChange();
    }
}
