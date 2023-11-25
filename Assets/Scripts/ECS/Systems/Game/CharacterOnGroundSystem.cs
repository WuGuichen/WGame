using System.Collections.Generic;
using Entitas;
using WGame.Trigger;

public class CharacterOnGroundSystem : ReactiveSystem<GameEntity>
{
    public CharacterOnGroundSystem(Contexts contexts) : base(contexts.game)
    {
        
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.GroundSensor);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGroundSensor;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        entities.ForEach(DoOnGround);
    }

    void DoOnGround(GameEntity entity)
    {
        entity.isOnGroundState = entity.groundSensor.intersect;

        if (entity.isOnGroundState)
        {
            // 处理一些落地操作
            // if (entity.isJumpState || entity.isAttackState)
            // {
                entity.ReplaceSignalLocalMotion(2f);
            // }
            WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.Sensor, SensorSubType.OnGround, SensorEvent.Enter, new WTrigger.Context(entity.entityID.id));
        }
    }
}
