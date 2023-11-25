using Entitas;

public class ProcessMotionUpdateSystem : IExecuteSystem
{
    private readonly IGroup<MotionEntity> _motionGroup;
    public ProcessMotionUpdateSystem(Contexts contexts)
    {
        _motionGroup = contexts.motion.GetGroup(MotionMatcher.MotionService);
    }
    public void Execute()
    {
        foreach (var entity in _motionGroup)
        {
            var service = entity.motionService.service;
            // if (entity.hasLinkCharacter)
            // {
            //     var character = entity.linkCharacter.Character;
            //     if (character.hasTriggerEvent)
            //     {
            //         var eventData = entity.linkCharacter.Character.triggerEvent;
            //         service.OnTriggerEvent(eventData.type, eventData.param1, eventData.param2);
            //         character.RemoveTriggerEvent();
            //     }
            // }
            service.UpdateMotion();
        }
    }
}
