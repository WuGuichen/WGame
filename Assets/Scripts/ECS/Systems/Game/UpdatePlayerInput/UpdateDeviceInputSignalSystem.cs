using Entitas;
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
            // 跳
            // 这里会有预输入
            // entity.ReplaceSignalJump(1f);
        }

        if (_inputContext.defense.value)
        {
            // 防御
            // entity.isPrepareDefenseState = true;
            entity.ReplaceSignalDefense(0.1f);
        }

        if (_inputContext.attackHoldInput.value)
        {
            // 蓄力攻击
            WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Attack, InputSignalEvent.IsHold, new WTrigger.Context(entity.entityID.id));
            entity.isPrepareHoldAttackState = true;
        }
        else
        {
            if(entity.isPrepareHoldAttackState)
                WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Attack,InputSignalEvent.WasReleased, new WTrigger.Context(entity.entityID.id));
            entity.isPrepareHoldAttackState = false;
        }

        if (_inputContext.attackInput.value)
        {
            // 攻击
            entity.ReplaceSignalAttack(0.2f);
        }

        if (_inputContext.stepInput.value)
        {
            entity.ReplaceSignalStep(0.2f);
        }
    }
}
