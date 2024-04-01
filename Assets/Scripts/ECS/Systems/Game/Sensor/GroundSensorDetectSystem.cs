using Entitas;
using UnityEngine;

public class GroundSensorDetectSystem : IExecuteSystem
{
    private readonly GameContext _gameContext;
    readonly int lm = 1 << LayerMask.NameToLayer("Ground");
    private Collider[] cols = new Collider[1];
    private readonly Vector3 size = new Vector3(0.2f, 0.2f, 0.2f);
    private readonly IGroup<GameEntity> _groundSensorGroup;

    public GroundSensorDetectSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        _groundSensorGroup = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.GroundSensor, GameMatcher.GameViewService));
    }

    public void Execute()
    {
        foreach (var entity in _groundSensorGroup)
        {
            var startPoint = entity.position.value;

            var num = Physics.OverlapBoxNonAlloc(startPoint, size, cols, Quaternion.identity, lm);
            if (num > 0)
            {
                if (entity.groundSensor.intersect == false)
                {
                    entity.ReplaceGroundSensor(true);
                }
            }
            else
            {
                if (entity.groundSensor.intersect)
                    entity.ReplaceGroundSensor(false);
            }
        }
    }
}
