using Entitas;
using UnityEngine;
using WGame.UI;

public class MoveCharacterSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _movingGroup;
    private readonly IInputService _inputService;
    private readonly ITimeService _timeService;
    private float curSpeed = 0f;
    private float speedMulti = 1f;

    public MoveCharacterSystem(Contexts contexts)
    {
        _inputService = contexts.meta.inputService.instance;
        _timeService = contexts.meta.timeService.instance;
        _movingGroup = contexts.game.GetGroup(GameMatcher.Moveable);
    }

    public void Execute()
    {
        foreach (var entity in _movingGroup)
        {
            if(entity.hasLinkMotion == false)
                continue;
            curSpeed = entity.charCurSpeed.value;
            speedMulti = entity.charSpeedMulti.value;
            var motion = entity.linkMotion.Motion;
            if (entity.hasActionThrust)
            {
                motion.motionService.service.AnimProcessor.UpdateRootMotion(true);
                DoThrust(entity);
                continue;
            }

            if (motion.hasDoMove)
            {
                motion.motionService.service.AnimProcessor.UpdateRootMotion(true);
                DoMove(motion);
                continue;
            }
            if(entity.isUnbalanced)
                continue;
            if (entity.hasFocus)
            {
                var pos = entity.focus.target.position;
                Vector2 tarPos = new Vector2(pos.x, pos.z);
                pos = entity.gameViewService.service.Position;
                Vector2 myPos = new Vector2(pos.x, pos.z);
                var sqrDist = (myPos - tarPos).sqrMagnitude;
                entity.ReplaceTargetPlanarSqrDistance(sqrDist);
                if (entity.hasKeepTargetDistance && sqrDist <= entity.keepTargetDistance.value * entity.keepTargetDistance.value)
                {
                    motion.motionService.service.AnimProcessor.UpdateRootMotion(true);
                    continue;
                }
            }

            if (!entity.isMoveable
                || !entity.hasMoveDirection
                || entity.isLockPlanarVec)
            {
                continue;
            }

            var isRunning = entity.isRunState;
            var tarMuti = isRunning ? entity.runningMultiRate.rate : 100;
            if (Mathf.Abs(speedMulti - tarMuti) < 0.1f)
                speedMulti = tarMuti;
            else
                speedMulti = Mathf.Lerp(speedMulti, tarMuti, 1f);
            var totalMulti = entity.animMoveMulti.rate * speedMulti * 0.0001f;
            var speed = entity.movementSpeed.value * totalMulti;
            var speedY = entity.rigidbodyService.service.Velocity.y;
            if (entity.isCamera)
            {
                if (entity.hasFocus)
                {
                    if (motion.hasMotionService)
                    {
                        var mainModel = MainModel.Inst;
                        if (mainModel.IsUseJoystick)
                        {
                            motion.motionService.service.AnimProcessor.UpdateMoveSpeed(
                                mainModel.MoveDir.y * totalMulti,
                                mainModel.MoveDir.x * totalMulti);
                        }
                        else
                        {
                            motion.motionService.service.AnimProcessor.UpdateMoveSpeed(
                                _inputService.Move.y * totalMulti,
                                _inputService.Move.x * totalMulti);
                        }

                        motion.motionService.service.AnimProcessor.UpdateRootMotion();
                    }
                }
                else
                {
                    var vecMulti = Mathf.Clamp01(Vector3.Dot(
                        entity.gameViewService.service.Model.forward,
                        entity.moveDirection.value));
                    speed *= vecMulti;
                    if (motion.hasMotionService)
                    {
                        motion.motionService.service.AnimProcessor.UpdateMoveSpeed(vecMulti*totalMulti, 0);
                        motion.motionService.service.AnimProcessor.UpdateRootMotion();
                    }
                }

            }
            else
            {
                var vecMulti = Mathf.Clamp01(Vector3.Dot(
                    entity.gameViewService.service.Model.forward,
                    entity.moveDirection.value));
                speed *= vecMulti;
                if (motion.hasMotionService)
                {
                    motion.motionService.service.AnimProcessor.UpdateMoveSpeed(vecMulti*totalMulti, 0);
                    motion.motionService.service.AnimProcessor.UpdateRootMotion();
                }
            }

            if (Mathf.Abs(curSpeed - speed) < 0.1f)
                curSpeed = speed;
            else
            {
                curSpeed = Mathf.Lerp(curSpeed, speed, 0.1f);
            }
            var move = entity.moveDirection.value * curSpeed;
            entity.rigidbodyService.service.Velocity = new Vector3(move.x, speedY, move.z);
            entity.rigidbodyService.service.OnFixedUpdate(_timeService.fixedDeltaTime);
            entity.ReplaceCharCurSpeed(curSpeed);
            entity.ReplaceCharSpeedMulti(speedMulti);
            
            EntityUtils.GameBVH.MarkForUpdate(entity.gameViewService.service);
        }
    }

    private void DoThrust(GameEntity entity)
    {
        if (entity.hasGameViewService)
        {
            entity.gameViewService.service.OnUpdateMove(_timeService.deltaTime);
        }
    }

    private void DoMove(MotionEntity entity)
    {
        var gameEntity = entity.linkCharacter.Character;
        var tarPos = entity.doMove.tarPos;
        switch (entity.doMoveType.type)
        {
            case DoMoveType.Lerp:
                var movePos = tarPos * entity.doMoveSpeed.value * _timeService.fixedDeltaTime;
                var dirX = tarPos.x;
                var leftPos = tarPos - movePos;
                bool needRemove = false;
                if (dirX > 0)
                    needRemove = leftPos.x < 0;
                else
                    needRemove = leftPos.x > 0;
                if (needRemove || leftPos.sqrMagnitude < 0.1f)
                {
                    // movePos = tarPos;
                    entity.RemoveDoMove();
                }
                else
                {
                    entity.ReplaceDoMove(leftPos);
                }
                gameEntity.gameViewService.service.Model.parent.position += movePos;
                break;
            default:
                break;
        }
    }
}
