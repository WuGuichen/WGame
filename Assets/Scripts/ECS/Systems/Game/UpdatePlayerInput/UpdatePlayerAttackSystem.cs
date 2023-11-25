using Entitas;

public class UpdateAttackInputSystem : IExecuteSystem
{
    private readonly InputContext _input;
    private GameEntity entity;
    private readonly GameContext _gameContext;
    private readonly IGroup<GameEntity> _cameraGroup;
    public UpdateAttackInputSystem(Contexts contexts)
    {
        _input = contexts.input;
        _gameContext = contexts.game;
        _cameraGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Camera, GameMatcher.GameViewService));
    }

    public void Execute()
    {
        foreach (var entity in _cameraGroup)
        {
            if (_input.attackInput.value)
            {
                entity.ReplaceSignalAttack(1f);
            }
        }
    }
}
