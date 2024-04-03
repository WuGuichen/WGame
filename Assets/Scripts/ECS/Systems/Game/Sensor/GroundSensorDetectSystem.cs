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
            // startPoint.y += size.y;

            var num = Physics.OverlapBoxNonAlloc(startPoint, size, cols, Quaternion.identity, lm);
            if (num > 0)
            {
                if (entity.groundSensor.intersect == false)
                {
                    entity.ReplaceGroundSensor(true);
                    if (entity.hasPlanarVec)
                    {
                        entity.RemovePlanarVec();
                    }

                    var vec = entity.rigidbodyService.service.Velocity;
                    if (vec.y < -8f)
                    {
                        entity.linkMotion.Motion.motionService.service.TransMotionByMotionType(MotionType.JumpLand);
                    }
                    entity.rigidbodyService.service.MovePosition(new Vector3(0, -size.y, 0));
                }
            }
            else
            {
                if (entity.groundSensor.intersect)
                {
                    entity.ReplaceGroundSensor(false);
                    if (entity.hasPlanarVec == false)
                    {
                        var vec = entity.rigidbodyService.service.Velocity;
                        vec.y = 0;
                        entity.AddPlanarVec(vec);
                    }
                }
            }
        }
    }
}
