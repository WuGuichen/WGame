using Entitas;
using UnityEngine;

public class CharacterDetectSystem : IExecuteSystem
{
    private readonly float coolDownTime = 0.5f;
    private float coolDownTimer = 1f;
    private int lm = 0;
    private int entityIdx = 0;
    private int curFrameCheck = 0;
    private Collider[] cols = new Collider[8];
    private IGroup<GameEntity> detectGroup;
    private readonly ITimeService _time;

    public CharacterDetectSystem(Contexts contexts)
    {
        coolDownTimer = coolDownTime;
        detectGroup = contexts.game.GetGroup(GameMatcher.CharacterSensor);
        _time = contexts.meta.timeService.instance;
    }
    
    public void Execute()
    {
        coolDownTimer -= _time.DeltaTime;
        if (coolDownTimer > 0)
            return;
        else
            coolDownTimer = coolDownTime;
        curFrameCheck++;
        if (curFrameCheck > detectGroup.count)
            curFrameCheck = 1;
        entityIdx = 0;
        foreach (var entity in detectGroup)
        {
            entityIdx++;
            if(entityIdx != curFrameCheck)
                continue;
            lm = entity.characterSensor.detectMask;
            Physics.OverlapSphereNonAlloc(
                entity.position.value, entity.characterSensor.radius, cols, lm);
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i] != null)
                {
                    var target = cols[i].GetComponentInParent<IGameViewService>()?.GetEntity();
                    if(target.isCamera == false)
                        target?.aiAgent.service.OnDetectCharacter(entity);
                    cols[i] = null;
                }
            }
        }
    }
}
