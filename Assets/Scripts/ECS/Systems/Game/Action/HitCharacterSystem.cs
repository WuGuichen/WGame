using System.Collections;
using System.Collections.Generic;
using Entitas;

public class HitCharacterSystem : ReactiveSystem<GameEntity>
{
    public HitCharacterSystem(Contexts contexts) : base(contexts.game)
    {
        
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.ActionHit);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasLinkMotion;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        entities.ForEach(DoHit);
    }

    void DoHit(GameEntity entity)
    {
        // entity.linkMotion.Motion.motionService.service.StartMotion(entity.linkMotion.Motion.motionHitFwd.UID);
        // entity.isActionHit = false;
    }
}
