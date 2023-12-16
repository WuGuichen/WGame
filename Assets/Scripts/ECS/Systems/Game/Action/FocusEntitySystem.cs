using System.Collections.Generic;
using Entitas;
using Shapes;
using TWY.Physics;
using UnityEngine;

public class FocusEntitySystem : ReactiveSystem<GameEntity>
{
    private Collider[] hitResults = new Collider[8];
    private List<Transform> hitTargets = new List<Transform>();
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
        switch (entity.actionFocus.type)
        {
            case FocusType.Focus:
                Detect(entity);
                if (hitTargets.Count == 0)
                    entity.isActionLookFwd = true;

                SetTarget(entity);
                break;
            case FocusType.Switch:
                hitTargets.Clear();
                SetTarget(entity);
                Detect(entity);
                SetTarget(entity);
                break;
            case FocusType.Cancel:
                hitTargets.Clear();
                SetTarget(entity);
                break;
            case FocusType.Up:
                Detect(entity);
                hitTargets.Sort((a, b) =>
                {
                    if (camera.WorldToScreenPoint(a.position).y < camera.WorldToScreenPoint(b.position).y)
                        return 1;
                    return -1;
                });
                SetTarget(entity);
                break;
            case FocusType.Down:
                Detect(entity);
                hitTargets.Sort((a, b) =>
                {
                    if (camera.WorldToScreenPoint(a.position).y > camera.WorldToScreenPoint(b.position).y)
                        return 1;
                    return -1;
                });
                SetTarget(entity);
                break;
            case FocusType.Left:
                hitTargets.Sort((a, b) =>
                {
                    if (camera.WorldToScreenPoint(a.position).x > camera.WorldToScreenPoint(b.position).x)
                        return 1;
                    return -1;
                });
                SetTarget(entity);
                break;
            case FocusType.Right:
                hitTargets.Sort((a, b) =>
                {
                    if (camera.WorldToScreenPoint(a.position).x < camera.WorldToScreenPoint(b.position).x)
                        return 1;
                    return -1;
                });
                SetTarget(entity);
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
        var center = entity.gameViewService.service.Position + fwd * area * 0.3f;
        SphereF sphere = new SphereF(center.ToFloat3(), area);
        var info = WDrawer.Inst.RegisterCircle(entity.gameViewService.service.Model, center);
        info.Info.borderColor = Color.yellow;
        info.Info.center = center;
        info.Info.radius = area;
        info.Info.forward = entity.gameViewService.service.Model.right;
        Circle.Draw(info);
        WLogger.Info("Draw");
    }

    private void SetTarget(GameEntity entity)
    {
        Transform target = null;
        if (entity.hasFocus)
        {
            for (int i = 0; i < hitTargets.Count; i++)
            {
                if (hitTargets[i] == entity.focus.target)
                {
                    if (i == 0)
                        target = hitTargets[i];
                    break;
                }

                target = hitTargets[i];
            }
        }
        else
        {
            if(hitTargets.Count != 0)
                target = hitTargets[0];
        }

        if (entity.hasFocus && entity.focus.target != target)
        {
            entity.focusEntity.entity.gameViewService.service.BeFocused(false);
            entity.RemoveFocus();
            entity.RemoveFocusEntity();
        }

        if (target)
        {
            entity.ReplaceFocus(target);
            var tarGameService = target.GetComponentInParent<IGameViewService>();
            entity.ReplaceFocusEntity(tarGameService.GetEntity());
            tarGameService?.BeFocused(true);
        }
    }
}
