using Entitas;
using UnityEngine;

public class AIAgentUpdateSystem : IExecuteSystem, ICleanupSystem
{
    private readonly IGroup<GameEntity> _aiGroup;

    public AIAgentUpdateSystem(Contexts contexts)
    {
        _aiGroup = contexts.game.GetGroup(GameMatcher.AiAgent);
    }
    
    public void Execute()
    {
        foreach (var entity in _aiGroup)
        {
            if (entity.isMoveable == false) continue;

            // entity.aiAgent.service.RotateCharacter(entity.gameViewService.service.Model, 0.2f);
            // entity.aiAgent.service.MoveCharacter(entity.gameViewService.service.Position);
            entity.aiAgent.service.UpdateFSM();

            // if (Input.GetKeyDown(KeyCode.N))
            // {
            //     ActionHelper.DoReachPoint(entity, Vector3.zero);
            // }
            //
            // if(Input.GetKeyDown(KeyCode.M))
            //     ActionHelper.DoStopAIPathFinding(entity);
        }
    }

    public void Cleanup()
    {
        foreach (var entity in _aiGroup)
        {
            entity.linkVM.VM.vMService.service.CleanUp();
        }
    }
}
