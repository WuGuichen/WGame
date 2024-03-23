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
            var agent = entity.netAgent.Agent;
            if (agent.IsOwner)
            {
                agent.UpdatePosition(entity.position.value, entity.gameViewService.service.Model.parent.localRotation);
                if (entity.hasLinkMotion)
                {
                    var anim = entity.linkMotion.Motion.motionService.service.AnimProcessor;
                    var param = anim.MoveParam;
                    agent.SetAnimParam(param.x, param.y);
                }
            }
        }
    }
}
