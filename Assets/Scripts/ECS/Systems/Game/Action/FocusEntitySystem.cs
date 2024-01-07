using System.Collections.Generic;
using Entitas;
using TWY.Physics;
using UnityEngine;

public class FocusEntitySystem : ReactiveSystem<GameEntity>
{
    private Collider[] hitResults = new Collider[8];
    private List<HitInfo> hitTargets = new();
    private readonly int lm = 1 << LayerMask.NameToLayer("FocusSensor");
    public FocusEntitySystem(Contexts contexts) : base(contexts.game)
    {
        
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
        var camera = Camera.main;
        int resIndex = -1;
        switch (entity.actionFocus.type)
        {
            case FocusType.Focus:
                Detect(entity);
                if (hitTargets.Count == 0)
                    entity.isActionLookFwd = true;

                float minSqrDist = float.MaxValue;
                for (int i = 0; i < hitTargets.Count; i++)
                {
                    var checkTarget = hitTargets[i];
                    if (checkTarget.SqrDist < minSqrDist)
                    {
                        minSqrDist = checkTarget.SqrDist;
                        resIndex = i;
                    }
                }

                SetTarget(entity, resIndex);
                break;
            case FocusType.Switch:
                hitTargets.Clear();
                // SetTarget(entity);
                Detect(entity);
                // SetTarget(entity);
                break;
            case FocusType.Cancel:
                hitTargets.Clear();
                // SetTarget(entity);
                break;
            case FocusType.Up:
                Detect(entity);
                // SetTarget(entity);
                break;
            case FocusType.Down:
                Detect(entity);
                // SetTarget(entity);
                break;
            case FocusType.Left:
                // SetTarget(entity);
                break;
            case FocusType.Right:
                // SetTarget(entity);
                break;
            default:
                break;
        }
    }

    private void Detect(GameEntity entity)
    {
        for (int i = 0; i < hitResults.Length; i++)
        {
            hitResults[i] = null;
        }
        
        var fwd = entity.gameViewService.service.Model.forward;
        var area = entity.actionFocus.area;
        var center = entity.position.value + fwd * area * 0.9f;
        WDrawer.Inst.RegisterCircle(center+Vector3.up*0.2f,entity.gameViewService.service.Model.up, area);
        WLogger.Info("Draw");
        SphereF sphere = new SphereF(center.ToFloat3(), area);
        if (entity.isCampWhite)
        {
            EntityUtils.BvhRed.TestHitSphereNonAlloc(sphere, ref hitTargets);
            WLogger.Info(hitTargets.Count);
        }
    }

    private void SetTarget(GameEntity entity, int index)
    {
        if (hitTargets.Count <= 0)
            return;
        var dir = hitTargets[0].Position - entity.position.value;
        dir = dir.normalized;
        var model = entity.gameViewService.service.Model;
        var angle = dir.GetAngle360(model.forward, model.up);
    }

    private bool GetClosestTarget(out HitInfo info)
    {
        info = HitInfo.EMPTY;
        float minSqrDist = float.MaxValue;
        for (int i = 0; i < hitTargets.Count; i++)
        {
            var checkTarget = hitTargets[i];
            if (checkTarget.SqrDist < minSqrDist)
            {
                minSqrDist = checkTarget.SqrDist;
                info = checkTarget;
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
    
    private bool GetRightTarget(GameEntity entity, ref HitInfo curInfo, out HitInfo info)
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
            var angle = -dir.GetAngle360(curFwd, curUp);
            if (angle < minAngle)
            {
                minAngle = angle;
                info = checkTarget;
            }
        }
        return info.EntityId != curInfo.EntityId;
    }
}
