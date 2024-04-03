using BaseData.Character;
using Entitas;
using UnityEngine;
using WGame.Ability;
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
            if(EntityUtils.IsNetCamera(entity))
                continue;
            if(entity.hasLinkMotion == false)
                continue;
            curSpeed = entity.charCurSpeed.value;
            speedMulti = entity.charSpeedMulti.value;
            var motion = entity.linkMotion.Motion;
            var anim = motion.motionService.service.AnimProcessor;
            var deltaRootMotion = anim.DeltaRootMotionPos + anim.DeltaEventMovePos;
            if (entity.hasActionThrust)
            {
                anim.ClearMotionMove();
                DoThrust(entity);
                continue;
            }

            if (motion.hasDoMove)
            {
                anim.ClearMotionMove();
                anim.UpdateMoveSpeed(0,0);
                DoMove(motion);
                continue;
            }
            if(entity.isUnbalanced)
                continue;
            if(entity.isNotMove)
                continue;
            if (entity.hasFocusEntity)
            {
                // var pos = entity.focusEntity.entity.position.value;
                // Vector2 tarPos = new Vector2(pos.x, pos.z);
                // pos = entity.position.value;
                // Vector2 myPos = new Vector2(pos.x, pos.z);
                // var sqrDist = (myPos - tarPos).sqrMagnitude;
                // entity.ReplaceTargetPlanarSqrDistance(sqrDist);
                // if (entity.hasKeepTargetDistance && sqrDist <= entity.keepTargetDistance.value * entity.keepTargetDistance.value)
                // {
                //     anim.ClearMotionMove();
                //     continue;
                // }
            }

            if (!entity.isMoveable
                || !entity.hasMoveDirection)
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
            float vecMulti;
            var model = entity.gameViewService.service.Model;
            if (entity.isCamera)
            {
                if (entity.hasFocusEntity)
                {
                    if (motion.hasMotionService)
                    {
                        var mainModel = MainModel.Inst;
                        if (mainModel.IsUseJoystick)
                        {
                            anim.UpdateMoveSpeed(
                                mainModel.MoveDir.y * totalMulti,
                                mainModel.MoveDir.x * totalMulti);
                        }
                        else
                        {
                            anim.UpdateMoveSpeed(
                                _inputService.Move.y * totalMulti,
                                _inputService.Move.x * totalMulti);
                        }
                    }
                }
                else
                {
                    vecMulti = Mathf.Clamp01(Vector3.Dot(
                        model.forward,
                        entity.moveDirection.value));
                    speed *= vecMulti;
                    if (motion.hasMotionService)
                    {
                        anim.UpdateMoveSpeed(vecMulti*totalMulti, 0);
                    }
                }

            }
            else
            {
                var moveDir = entity.moveDirection.value;
                moveDir = model.InverseTransformVector(moveDir);

                if (entity.hasFocusEntity)
                {
                    anim.UpdateMoveSpeed(
                        moveDir.z,
                        moveDir.x);
                }
                else
                {
                    vecMulti = Mathf.Clamp01(Vector3.Dot(
                        model.forward,
                        entity.moveDirection.value));
                    speed *= vecMulti;
                    anim.UpdateMoveSpeed(vecMulti * totalMulti, 0);
                }
            }

            var rateDeltaTime = entity.characterTimeScale.rate * _timeService.FixedDeltaTime;
            if (entity.hasPlanarVec)
            {
                entity.rigidbodyService.service.Velocity = entity.planarVec.value*rateDeltaTime + deltaRootMotion;
            }
            else
            {
                if (Mathf.Abs(curSpeed - speed) < 0.1f)
                {
                    curSpeed = speed;
                }
                else
                {
                    curSpeed = Mathf.Lerp(curSpeed, speed, 0.1f);
                }

                var move = entity.moveDirection.value * curSpeed;
                var totalSpeed = new Vector3(move.x, 0, move.z) * rateDeltaTime +
                                 deltaRootMotion;
                entity.rigidbodyService.service.Velocity = totalSpeed;
                entity.ReplaceCharCurSpeed(curSpeed);
                entity.ReplaceCharSpeedMulti(speedMulti);
            }
            entity.rigidbodyService.service.OnFixedUpdate(rateDeltaTime);
            anim.ClearMotionMove();

            if (entity.characterInfo.value.camp == Camp.Red)
            {
                EntityUtils.BvhRed.MarkForUpdate(entity.gameViewService.service);
            }
        }
    }

    private void DoThrust(GameEntity entity)
    {
        if (entity.hasGameViewService)
        {
            
        }
    }

    private void DoMove(MotionEntity entity)
    {
        var gameEntity = entity.linkCharacter.Character;
        var tarPos = entity.doMove.tarPos;
        var rateDeltaTime = gameEntity.characterTimeScale.rate * _timeService.FixedDeltaTime;
        switch (entity.doMoveType.type)
        {
            case DoMoveType.Lerp:
                var movePos = tarPos * entity.doMoveSpeed.value * rateDeltaTime;
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
                gameEntity.rigidbodyService.service.Velocity = movePos * rateDeltaTime;
                break;
            default:
                break;
        }
    }
}
