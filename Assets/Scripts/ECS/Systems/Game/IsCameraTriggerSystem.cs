using System.Collections.Generic;
using Entitas;
using WGame.Runtime;

public class IsCameraTriggerSystem : ReactiveSystem<GameEntity>
{
    public IsCameraTriggerSystem(Contexts contexts) : base(contexts.game)
    {
        
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Camera);
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            if (entity.hasNetAgent)
            {
                entity.netAgent.Agent.IsCamera = entity.isCamera;
            }
        }
    }
}
