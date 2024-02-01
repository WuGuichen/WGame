using Entitas;

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

            entity.aiAgent.service.UpdateFSM();
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
