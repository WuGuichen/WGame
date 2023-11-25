using Entitas;
using UnityEngine;

public class UpdateStepInputSystem : IExecuteSystem
{
    private readonly GameContext _gameContext;
    private readonly InputContext _inputContext;
    private readonly IGroup<GameEntity> _cameraGroup;
    public UpdateStepInputSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        _inputContext = contexts.input;
        _cameraGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Camera, GameMatcher.GameViewService));
    }

    public void Execute()
    {
        foreach (var entity in _cameraGroup)
        {
            if (_inputContext.stepInput.value)
            {
                entity.ReplaceSignalStep(1f);
            }
        }
    }
}
