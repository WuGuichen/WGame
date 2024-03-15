using Entitas;
using UnityEngine;

public class RotateCharacterSystem : IExecuteSystem
{
    private readonly InputContext _inputContext;
    private Transform cameraTrans;
    private Transform playerTrans;
    private readonly IGroup<GameEntity> _moveGroup;
    private readonly ITimeService _time;

    public RotateCharacterSystem(Contexts contexts)
    {
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
            playerTrans = entity.gameViewService.service.Model.parent;
            if (isCamera)
            {
                moveDir = _inputContext.moveInput.value;
            }
            else
            {
                var tmp = entity.moveDirection.value;
                moveDir = new Vector2(tmp.x, tmp.z);
            }

            float leftAngle;
            if (entity.hasFocusEntity && !entity.isRotateInFocus)
            {
                if (isCamera)
                {
                    fwd = cameraTrans.forward;
                }
                else
                {
                    var dir = entity.focusEntity.entity.position.value - entity.position.value;
                    dir.y = 0;
                    fwd = dir.normalized;
                }
                fwd.y = 0;
                if (fwd.Equals(Vector3.zero))
                {
                    fwd = playerTrans.forward;
                }
                var tarRot = Quaternion.LookRotation(fwd);
                var rotRate = entity.rotationSpeed.value * entity.animRotateMulti.rate;
                var playerRot = Quaternion.RotateTowards(playerTrans.localRotation, tarRot, rotRate * _time.FixedDeltaTime);
                leftAngle = fwd.GetAngle(moveDir);
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
                    tarDir = parentTrans.forward * moveDir.y +parentTrans.right * moveDir.x;
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
