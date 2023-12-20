using Entitas;
using TWY.Physics;

public class UpdateDetectDataSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _whiteGroup;
    private readonly IGroup<GameEntity> _redGroup;
    public UpdateDetectDataSystem(Contexts contexts)
    {
        _whiteGroup = contexts.game.GetGroup(GameMatcher.CampRed);
    }

    public void Execute()
    {
        foreach (var entity in _whiteGroup)
        {
            var position = entity.position.value;
            var sphere = new SphereF(position.ToFloat3(), 20f);
        }
    }
}
