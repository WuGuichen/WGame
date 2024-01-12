using Entitas;
using UnityEngine;

public class RotatePlayerSystem : IExecuteSystem
{
    private readonly GameContext _gameContext;
    private readonly InputContext _inputContext;
    private Transform cameraTrans;
    private Transform playerTrans;
    private readonly IGroup<GameEntity> _cameraGroup;
    private readonly ITimeService _time;

    public RotatePlayerSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        _inputContext = contexts.input;
        this.cameraTrans = contexts.meta.mainCameraService.service.Camera;
        _cameraGroup = contexts.game.GetGroup(GameMatcher.Camera);
        _time = contexts.meta.timeService.instance;
    }

    public void Execute()
    {
        foreach (var entity in _cameraGroup)
        {
            if (entity.isMoveable == false) continue;
            if(entity.hasAiAgent && entity.aiAgent.service.IsActing) continue;

            var move = _inputContext.moveInput.value;
            playerTrans = entity.gameViewService.service.Model;

            if (entity.hasFocusEntity && !entity.isRotateInFocus)
            {
                var fwd = cameraTrans.forward;
                fwd.y = 0;
                var tarRot = Quaternion.LookRotation(fwd);
                var rotRate = entity.rotationSpeed.value * entity.animRotateMulti.rate;
                var playerRot = Quaternion.RotateTowards(playerTrans.localRotation, tarRot, rotRate * _time.FixedDeltaTime);
                playerTrans.localRotation = playerRot;
            }
            else
            {
                var tarDir = cameraTrans.forward * move.y;
                tarDir += cameraTrans.right * move.x;
                tarDir.Normalize();
                tarDir.y = 0;

                if (tarDir == Vector3.zero)
                {
                    tarDir = playerTrans.forward;
                }

                var tarRot = Quaternion.LookRotation(tarDir);
                var rotRate = entity.rotationSpeed.value * entity.animRotateMulti.rate;
                var playerRot = Quaternion.RotateTowards(playerTrans.localRotation, tarRot, rotRate * _time.FixedDeltaTime);

                playerTrans.localRotation = playerRot;
            }
        }
    }
}
