using Entitas;
using UnityEngine;

public class RotatePlayerSystem : IExecuteSystem
{
    private readonly GameContext _gameContext;
    private readonly InputContext _inputContext;
    private Transform cameraTrans;
    private Transform playerTrans;
    private readonly IGroup<GameEntity> _moveGroup;
    private readonly ITimeService _time;

    public RotatePlayerSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        _inputContext = contexts.input;
        this.cameraTrans = contexts.meta.mainCameraService.service.Camera;
        _moveGroup = contexts.game.GetGroup(GameMatcher.Moveable);
        _time = contexts.meta.timeService.instance;
    }

    public void Execute()
    {
        foreach (var entity in _moveGroup)
        {
            Vector2 moveDir;
            Vector3 tarDir;
            Vector3 fwd;
            bool isCamera = entity.isCamera;
            playerTrans = entity.gameViewService.service.Model;
            if (isCamera)
            {
                moveDir = _inputContext.moveInput.value;
            }
            else
            {
                var tmp = entity.moveDirection.value;
                moveDir = new Vector2(tmp.x, tmp.z);
            }

            if (entity.hasFocusEntity && !entity.isRotateInFocus)
            {
                if (isCamera)
                {
                    fwd = cameraTrans.forward;
                }
                else
                {
                    fwd = entity.moveDirection.value;
                }
                fwd.y = 0;
                if (fwd.Equals(Vector3.zero))
                {
                    fwd = playerTrans.forward;
                }
                var tarRot = Quaternion.LookRotation(fwd);
                var rotRate = entity.rotationSpeed.value * entity.animRotateMulti.rate;
                var playerRot = Quaternion.RotateTowards(playerTrans.localRotation, tarRot, rotRate * _time.FixedDeltaTime);
                playerTrans.localRotation = playerRot;
            }
            else
            {
                if (entity.isCamera)
                {
                    // 相机控制角色的目标方向计算
                    tarDir = cameraTrans.forward * moveDir.y;
                    tarDir += cameraTrans.right * moveDir.x;
                    tarDir.Normalize();
                }
                else
                {
                    var parentTrans = playerTrans.parent;
                    tarDir = parentTrans.forward * moveDir.y + parentTrans.right * moveDir.x;
                    tarDir.Normalize();
                }
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
