using Entitas;
using UnityEngine;

public class SyncServerCharacterSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _netAgentGroup;

    public SyncServerCharacterSystem(Contexts contexts)
    {
        _netAgentGroup = contexts.game.GetGroup(GameMatcher.NetAgent);
    }

    public void Execute()
    {
        foreach (var entity in _netAgentGroup)
        {
            if(entity.netAgent.Agent.IsOwner)
                continue;
            if (entity.hasGameViewService)
            {
                entity.gameViewService.service.Model.parent.transform.position = entity.netAgent.Agent.SyncPos;
            }
        }
        // Physics.SyncTransforms();
    }
}
