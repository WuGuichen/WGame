using System.Collections.Generic;
using Entitas;

public class AnimSpeedSystem : ReactiveSystem<GameEntity>
{
    public AnimSpeedSystem(Contexts contexts) : base(contexts.game)
    {
        
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AnimationSpeed);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasAnimatorService;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        entities.ForEach(DoChangeAnimSpeed);
    }

    void DoChangeAnimSpeed(GameEntity entity)
    {
        entity.animatorService.service.SetSpeed(entity.animationSpeed.speed);
    }
}
