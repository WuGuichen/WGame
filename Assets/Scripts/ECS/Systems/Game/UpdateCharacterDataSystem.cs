using Entitas;

public class UpdateCharacterDataSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _positionGroup;
    private readonly IGroup<GameEntity> _rigidBodyGroup;
    private readonly ITimeService _timeService;
    public UpdateCharacterDataSystem(Contexts contexts)
    {
        _positionGroup = contexts.game.GetGroup(GameMatcher.Position);
        _rigidBodyGroup = contexts.game.GetGroup(GameMatcher.RigidbodyService);
        _timeService = contexts.meta.timeService.instance;
    }
    public void Execute()
    {
        foreach (var entity in _rigidBodyGroup)
        {
            entity.rigidbodyService.service.OnUpdate(_timeService.DeltaTime(entity.characterTimeScale.rate));
        }
        foreach (var entity in _positionGroup)
        {
            if (entity.hasGameViewService)
            {
                var pos = entity.gameViewService.service.Model.position;
                entity.ReplacePosition(pos);
            }
        }
    }
}
