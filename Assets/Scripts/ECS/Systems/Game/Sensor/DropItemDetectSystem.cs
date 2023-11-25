using Entitas;
using UnityEngine;
using WGame.UI;

public class DropItemDetectSystem : IExecuteSystem
{
    private readonly float coolDownTime = 0.5f;
    private float coolDownTimer = 0f;
    private readonly int lm = 1 << LayerMask.NameToLayer("DropItem");
    private Collider[] cols = new Collider[8];
    private IGroup<GameEntity> detectGroup;
    private readonly ITimeService _time;

    private readonly MainModel mainModel;

    public DropItemDetectSystem(Contexts contexts)
    {
        coolDownTimer = coolDownTime;
        detectGroup = contexts.game.GetGroup(GameMatcher.DropItemSensor);
        _time = contexts.meta.timeService.instance;
        mainModel = MainModel.Inst;
    }

    public void Execute()
    {
        coolDownTimer -= _time.deltaTime;
        if (coolDownTimer < 0)
        {
            Detect();
            coolDownTimer = coolDownTime;
        }
    }

    private void Detect()
    {
        foreach (var entity in detectGroup)
        {
            if (entity.isCamera)
            {
                bool notTarget = true;
                bool loseTarget = true;
                Interactable tarInter = null;
                float minSqrDist = float.MaxValue;
                Physics.OverlapSphereNonAlloc(
                    entity.gameViewService.service.Position + entity.gameViewService.service.Model.forward, 1.5f, cols,
                    lm);
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i] != null)
                    {
                        var interaction = cols[i].GetComponent<Interactable>();

                        var sqrDist = (entity.gameViewService.service.Position - interaction.TagPos).sqrMagnitude;
                        if (sqrDist < minSqrDist)
                        {
                            minSqrDist = sqrDist - 0.001f;
                            tarInter = interaction;
                        }

                        cols[i] = null;
                        notTarget = false;
                    }
                }

                if (notTarget)
                {
                    mainModel.ResetInteractTag(null, -1);
                }
                else
                {
                    mainModel.ResetInteractTag(tarInter
                        , minSqrDist);
                }
            }
        }
    }
}
