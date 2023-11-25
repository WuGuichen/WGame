using Entitas;
using WGame.Trigger;

public class UpdateInputSignalSystem : IExecuteSystem
{
    private readonly InputContext _inputContext;
    private readonly GameContext _gameContext;
    private readonly IGroup<GameEntity> _cameraGroup;
    
    public UpdateInputSignalSystem(Contexts contexts)
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
            HandleJumpState(entity);
        }
    }

    void HandleJumpState(GameEntity entity)
    {
        if (_inputContext.runInput.value && entity.linkMotion.Motion.motionStart.UID == entity.linkMotion.Motion.motionLocalMotion.UID)
        {
            entity.isRunState = true;
        }
        else
        {
            entity.isRunState = false;
        }
        if (_inputContext.jumpInput.value)
        {
            // 这里会有预输入
            entity.ReplaceSignalJump(1f);
        }

        if (_inputContext.defense.value)
        {
            entity.isPrepareDefenseState = true;
        }
        else
        {
            if (entity.isPrepareDefenseState)
            {
                WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Defense, InputSignalEvent.WasReleased, new WTrigger.Context(entity.entityID.id));
            }
            entity.isPrepareDefenseState = false;
        }

        if (_inputContext.attackHoldInput.value)
        {
            WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Attack, InputSignalEvent.IsHold, new WTrigger.Context(entity.entityID.id));
            entity.isPrepareHoldAttackState = true;
        }
        else
        {
            if(entity.isPrepareHoldAttackState)
                WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Attack,InputSignalEvent.WasReleased, new WTrigger.Context(entity.entityID.id));
            entity.isPrepareHoldAttackState = false;
        }
    }

}
