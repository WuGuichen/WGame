using Entitas;

public class UpdateCharacterSyncDataSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _netAgentGroup;
    
    public UpdateCharacterSyncDataSystem(Contexts contexts)
    {
        _netAgentGroup = contexts.game.GetGroup(GameMatcher.NetAgent);
    }
    public void Execute()
    {
        foreach (var entity in _netAgentGroup)
        {
            if (entity.netAgent.Agent.IsOwner)
            {
                entity.netAgent.Agent.UpdatePosition(entity.position.value);
            }
        }
    }
}
