using System.Collections.Generic;
using Entitas;

public class ProcessMotionEndSystem : ReactiveSystem<MotionEntity>
{
    public ProcessMotionEndSystem(Contexts contexts) : base(contexts.motion)
    {
    }

    protected override ICollector<MotionEntity> GetTrigger(IContext<MotionEntity> context)
    {
        return context.CreateCollector(MotionMatcher.MotionEnd);
    }

    protected override bool Filter(MotionEntity entity)
    {
        return entity.hasMotionService;
    }

    protected override void Execute(List<MotionEntity> entities)
    {
        entities.ForEach(OnMotionEnd);
    }

    private void OnMotionEnd(MotionEntity entity)
    {
        var character = entity.linkCharacter.Character;
        // var noticeService = character.notice.service;
        
        entity.motionService.service.OnMotionExit();
        // var motionDB = character.motionDB.data;
        // var abilityID = entity.motionStart.UID;
        
        // 根据动作类型处理行为
    }
}
