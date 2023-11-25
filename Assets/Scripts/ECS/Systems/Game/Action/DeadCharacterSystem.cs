using System.Collections.Generic;
using Entitas;
using WGame.Attribute;

public class DeadCharacterSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _attrGroup;
    private readonly IGroup<GameEntity> _focusGroup;
    private readonly IGroup<GameEntity> _detectedCharGroup;
    public DeadCharacterSystem(Contexts contexts)
    {
        _attrGroup = contexts.game.GetGroup(GameMatcher.Attribute);
        _focusGroup = contexts.game.GetGroup(GameMatcher.FocusEntity);
        _detectedCharGroup = contexts.game.GetGroup(GameMatcher.DetectedCharacter);
    }

    public void Execute()
    {
        foreach (var entity in _attrGroup)
        {
            if (!entity.isDeadState && entity.attribute.value.Get(WAttrType.CurHP) <= 0)
            {
                entity.isDeadState = true;
                entity.gameViewService.service.OnDead();
                if(entity.hasUIHeadPad)
                    entity.uIHeadPad.service.OnDead(entity);
                
                var motion = entity.linkMotion.Motion;
                if (motion.hasMotionDead)
                    motion.motionService.service.SwitchMotion(motion.motionDead.UID);
            }
        }

        var listFocus = new List<GameEntity>();
        var listDetected = new List<GameEntity>();

        foreach (var entity in _focusGroup)
        {
            if(entity.focusEntity.entity.isDeadState)
                listFocus.Add(entity);
        }

        foreach (var entity in _detectedCharGroup)
        {
            if(entity.detectedCharacter.entity.isDeadState)
                listDetected.Add(entity);
        }
        for(int i= 0 ; i < listDetected.Count; i++)
        {
            listDetected[i].RemoveDetectedCharacter();
        }
        for(int i= 0 ; i < listFocus.Count; i++)
        {
            listFocus[i].RemoveFocus();
            listFocus[i].RemoveFocusEntity();
        }
    }
}
