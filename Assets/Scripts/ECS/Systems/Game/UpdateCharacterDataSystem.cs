using Entitas;

public class UpdateCharacterDataSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _positionGroup;
    private readonly IGroup<GameEntity> _netAgentGroup;
    public UpdateCharacterDataSystem(Contexts contexts)
    {
        _positionGroup = contexts.game.GetGroup(GameMatcher.Position);
        _netAgentGroup = contexts.game.GetGroup(GameMatcher.NetAgent);
    }
    public void Execute()
    {
        foreach (var entity in _positionGroup)
        {
            if (entity.hasGameViewService)
            {
                var pos = entity.gameViewService.service.Model.position;
                entity.ReplacePosition(pos);
                if (entity.hasNetAgent)
                {
                    if (entity.netAgent.Agent.IsOwner)
                    {
                        entity.netAgent.Agent.UpdatePosition(pos);
                    }
                }
            }
        }
    }
}
