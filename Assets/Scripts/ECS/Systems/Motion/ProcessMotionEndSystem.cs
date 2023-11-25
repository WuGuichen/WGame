using System.Collections.Generic;
using Entitas;
using Motion;

public class ProcessMotionEndSystem : ReactiveSystem<MotionEntity>
{
    readonly IFactoryService _factoryService;
    // private readonly ITriggerService _triggerService;
    public ProcessMotionEndSystem(Contexts contexts) : base(contexts.motion)
    {
        _factoryService = contexts.meta.factoryService.instance;
        // _triggerService = contexts.meta.triggerService.instance;
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
        switch (entity.motionEnd.UID)
        {
            case MotionType.Attack1:
                break;
            case MotionType.Attack2:
                break;
            case MotionType.Attack3:
                break;
            default:
                break;
        }
        entity.motionService.service.OnMotionExit();
    }
}
