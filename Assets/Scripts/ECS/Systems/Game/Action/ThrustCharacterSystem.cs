using System.Collections.Generic;
using Entitas;

public class ThrustCharacterSystem : ReactiveSystem<GameEntity>
{
    
    public ThrustCharacterSystem(Contexts context) : base(context.game)
    {
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.ActionThrust);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGameViewService;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        entities.ForEach(DoThrust);
    }
    
    private void DoThrust(GameEntity entity)
    {
        entity.gameViewService.service.Thrust();
    }

}
