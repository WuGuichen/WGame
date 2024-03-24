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
            var agent = entity.netAgent.Agent;
            if (entity.hasGameViewService)
            {
                var trans = entity.gameViewService.service.Model.parent;
                trans.position = agent.SyncPos;
                trans.rotation = agent.SyncRot;
            }
            if(entity.hasLinkMotion)
            {
                entity.linkMotion.Motion.motionService.service.AnimProcessor.UpdateMoveSpeed(agent.AnimUp, agent.AnimRight);
            }
        }
    }
}
