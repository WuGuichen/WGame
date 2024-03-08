using System;
using System.Collections.Generic;
using Entitas;
using TWY.Physics;
using UnityEngine;

public class FocusEntitySystem : ReactiveSystem<GameEntity>
{
    private Collider[] hitResults = new Collider[8];
    private List<HitInfo> hitTargets = new();
    private readonly int lm = 1 << LayerMask.NameToLayer("FocusSensor");
    private readonly Camera mainCamera;
    public FocusEntitySystem(Contexts contexts) : base(contexts.game)
    {
        mainCamera = Camera.main;
    }


    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.ActionFocus);
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        entities.ForEach(DoFocus);
    }

    private void DoFocus(GameEntity entity)
    {
        int resIndex = -1;
        var focusType = entity.actionFocus.type;
        switch (focusType)
        {
            case FocusType.Focus:
                Detect(entity);
                if (GetClosestTarget(out var info))
                    EntityUtils.SetFocusTarget(entity, info.EntityId);
                else
                    EntityUtils.SetFocusTarget(entity, null);
                break;
            case FocusType.Switch:
                EntityUtils.SetFocusTarget(entity, null);
                WLogger.Print("Switch Focus Target");
                Detect(entity);
                if (GetClosestTarget(out var newInfo))
                {
                    if (newInfo.IsNotEmpty)
                    {
                        EntityUtils.SetFocusTarget(entity, newInfo.EntityId);
                    }
                }
                break;
            case FocusType.Cancel:
                EntityUtils.SetFocusTarget(entity, null);
                break;
            case FocusType.Left:
            case FocusType.Right:
                ChangeToOtherTarget(entity, focusType);
                break;
            default:
                break;
        }
    }

    private void Detect(GameEntity entity)
    {
        var length = hitResults.Length;
        for (var i = 0; i < length; i++)
        {
            hitResults[i] = null;
        }
        
        var fwd = entity.gameViewService.service.Model.forward;
        var area = entity.actionFocus.area;
        var center = entity.position.value + fwd * area * 0.9f;
        // WDrawer.Inst.RegisterCircle(center+Vector3.up*0.2f,entity.gameViewService.service.Model.up, area);
        SphereF sphere = new SphereF(center.ToFloat3(), area);
        if (entity.isCampWhite)
        {
            EntityUtils.BvhRed.TestHitSphereNonAlloc(sphere, ref hitTargets);
        }
    }

    private bool GetClosestTarget(out HitInfo info)
    {
        info = HitInfo.EMPTY;
        float minSqrDist = float.MaxValue;
        for (int i = 0; i < hitTargets.Count; i++)
        {
            var checkTarget = hitTargets[i];
            if (EntityUtils.GetGameEntity(checkTarget.EntityId).isDeadState == false)
            {
                if (checkTarget.SqrDist < minSqrDist)
                {
                    minSqrDist = checkTarget.SqrDist;
                    info = checkTarget;
                }
            }
        }

        return info.IsNotEmpty;
    }

    private bool GetLeftTarget(GameEntity entity, ref HitInfo curInfo, out HitInfo info)
    {
        info = curInfo;
        float minAngle = float.MaxValue;
        var curModel = entity.gameViewService.service.Model;
        var curFwd = curModel.forward;
        var curUp = curModel.up;
        for (int i = 0; i < hitTargets.Count; i++)
        {
            var checkTarget = hitTargets[i];
            var dir = checkTarget.Position - entity.position.value;
            var angle = dir.GetAngle360(curFwd, curUp);
            if (angle < minAngle)
            {
                minAngle = angle;
                info = checkTarget;
            }
        }
        return info.EntityId != curInfo.EntityId;
    }

    private void ChangeToOtherTarget(GameEntity entity, FocusType focusType)
    {
        Detect(entity);
        if (GetOtherTarget(entity, out var newInfo, focusType))
        {
            EntityUtils.SetFocusTarget(entity, newInfo.EntityId);
        }
    }
    
    private bool GetOtherTarget(GameEntity entity, out HitInfo info, FocusType focusType)
    {
        info = HitInfo.EMPTY;
        if (entity.hasFocusEntity == false)
        {
            return false;
        }

        var curTargetEntity = entity.focusEntity.entity;
        var curScreenPos = mainCamera.WorldToScreenPoint(curTargetEntity.gameViewService.service.FocusPoint);
        float minOffset = float.MaxValue;
        for (int i = 0; i < hitTargets.Count; i++)
        {
            var checkTarget = hitTargets[i];
            if(checkTarget.EntityId == curTargetEntity.instanceID.ID)
                continue;
            var screenPos = mainCamera.WorldToScreenPoint(checkTarget.Position);
            if (screenPos.z > 0.0f)
            {
                bool suitable = false;
                float offset = 0f;
                switch (focusType)
                {
                    case FocusType.Left:
                        offset = curScreenPos.x - screenPos.x;
                        break;
                    case FocusType.Right:
                        offset = screenPos.x - curScreenPos.x;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(focusType), focusType, null);
                }
                suitable = offset > 0;
                if (suitable && offset < minOffset)
                {
                    minOffset = offset;
                    info = hitTargets[i];
                }
            }
        }

        return info.IsNotEmpty;
    }
}
