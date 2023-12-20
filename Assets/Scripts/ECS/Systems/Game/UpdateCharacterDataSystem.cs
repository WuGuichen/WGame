using Entitas;

public class UpdateCharacterDataSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _positionGroup;
    public UpdateCharacterDataSystem(Contexts contexts)
    {
        _positionGroup = contexts.game.GetGroup(GameMatcher.Position);
    }
    public void Execute()
    {
        foreach (var entity in _positionGroup)
        {
            entity.ReplacePosition(entity.gameViewService.service.Model.position);
        }
    }
}
