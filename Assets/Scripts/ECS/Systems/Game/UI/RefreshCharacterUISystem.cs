using Entitas;

public class RefreshCharacterUISystem: IExecuteSystem
{
    private readonly IGroup<GameEntity> _headPadGroup;
    private readonly GameContext _gameContext;

    public RefreshCharacterUISystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        _headPadGroup = _gameContext.GetGroup(GameMatcher.UIHeadPad);
    }
    
    public void Execute()
    {
        foreach (var entity in _headPadGroup)
        {
            var headPad = entity.uIHeadPad.service;
            headPad.UpdateUI(entity);
        }
    }
}
