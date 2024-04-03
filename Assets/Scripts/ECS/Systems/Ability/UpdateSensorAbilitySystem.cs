using System.Collections.Generic;
using Entitas;
using TWY.Physics;
using UnityEngine;
using WGame.UI;

public class UpdateSensorAbilitySystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _whiteGroup;
    private List<HitInfo> hitInfos = new();
    private const float BACK_STAB_DIST = 2f;
    private const float BACK_STAB_DIST_OUT = BACK_STAB_DIST + 0.2f;
    private const float BACK_STAB_ANGLE = 30f;
    private const float BACK_STAB_ANGLE_TARGET = 172f;
    private const float BACK_STAB_MAX_Y_OFFSET = 0.4f;
    
    public UpdateSensorAbilitySystem(Contexts contexts)
    {
        _whiteGroup = contexts.game.GetGroup(GameMatcher.CampWhite);
    }
    
    public void Execute()
    {
        foreach (var entity in _whiteGroup)
        {
            var ability = entity.linkAbility.Ability;
            var mID = entity.instanceID.ID;
            var canStab = entity.linkMotion.Motion.motionService.service.CheckMotionType(MotionType.LocalMotion);
            if (ability.hasAbilityBackStab)
            {
                var targetId = ability.abilityBackStab.EntityID;
                var target = EntityUtils.GetGameEntity(targetId);
                if (!canStab || target.isUnbalanced)
                {
                    ability.RemoveAbilityBackStab();
                    if (entity.isCamera)
                    {
                        MainModel.Inst.HasWeakPoint = false;
                    }
                }

                var angleToTarget = DetectMgr.Inst.GetAngle(mID, targetId);
                var angleToEntity = DetectMgr.Inst.GetAngle(targetId, mID);
                var offsetY = target.position.value.y - entity.position.value.y;
                var checkDist = DetectMgr.Inst.GetDistance(mID, targetId);
                if (angleToTarget > BACK_STAB_ANGLE
                    || angleToEntity < BACK_STAB_ANGLE_TARGET
                    || offsetY > BACK_STAB_MAX_Y_OFFSET
                    || checkDist > (BACK_STAB_DIST_OUT + target.gameViewService.service.Radius))
                {
                    ability.RemoveAbilityBackStab();
                    if (entity.isCamera)
                    {
                        MainModel.Inst.HasWeakPoint = false;
                    }
                }

                if (entity.isCamera)
                {
                    MainModel.Inst.WeakPosition = target.gameViewService.service.FocusPoint;
                }
            }
            else
            {
                if (!canStab) continue;
                var position = entity.position.value;
                var sphere = new SphereF(position.ToFloat3(), BACK_STAB_DIST);
                hitInfos.Clear();
                EntityUtils.BvhRed.TestHitSphereNonAlloc(sphere, ref hitInfos);
                for (int i = 0; i < hitInfos.Count; i++)
                {
                    var hitInfo = hitInfos[i];
                    var target = EntityUtils.GetGameEntity(hitInfo.EntityId);
                    var targetMotion = target.linkMotion.Motion.motionService.service;
                    
                    var targetCanBeStab = targetMotion.CheckMotionType(MotionType.LocalMotion);
                    // if (targetCanBeStab)
                    // {
                    //     var hateInfo = DetectMgr.Inst.GetHatePointInfo(target.instanceID.ID);
                    //     if (hateInfo.TryGet(entity.instanceID.ID, out var hInfo))
                    //     {
                    //         targetCanBeStab = hInfo.Rank <= HateRankType.Null;
                    //     }
                    // }

                    if (targetCanBeStab && target.isUnbalanced == false)
                    {
                        var angleToTarget = DetectMgr.Inst.GetAngle(mID, hitInfo.EntityId);
                        var angleToEntity = DetectMgr.Inst.GetAngle(hitInfo.EntityId, mID);
                        if (angleToTarget < BACK_STAB_ANGLE
                            && angleToEntity > BACK_STAB_ANGLE_TARGET
                            && Mathf.Abs(hitInfo.Position.y - position.y) < BACK_STAB_MAX_Y_OFFSET)
                        {
                            ability.AddAbilityBackStab(hitInfo.EntityId);
                            if (entity.isCamera)
                            {
                                MainModel.Inst.HasWeakPoint = true;
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
