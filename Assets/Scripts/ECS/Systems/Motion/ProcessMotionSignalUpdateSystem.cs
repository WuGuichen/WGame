using Entitas;
using WGame.Trigger;

public class ProcessMotionSignalUpdateSystem : IExecuteSystem, ICleanupSystem
{
    private readonly IGroup<MotionEntity> _motionGroup;
    private readonly ITimeService _timeService;
    
    public ProcessMotionSignalUpdateSystem(Contexts contexts)
    {
        _motionGroup = contexts.motion.GetGroup(MotionMatcher.MotionService);
        _timeService = contexts.meta.timeService.instance;
    }


    public void Execute()
    {
        foreach (var motion in _motionGroup)
        {
            if (!motion.hasLinkCharacter)
                continue;
            var entity = motion.linkCharacter.Character;

            if (entity.hasSignalAttack)
            {
                if (motion.hasMotionJump && motion.motionStart.UID == motion.motionJump.UID)
                    entity.isPrepareJumpAttackState = true;
                else
                    entity.isPrepareAttackState = true;
            }

            if (entity.hasSignalJump)
            {
                entity.isPrepareJumpState = true;
            }

            if (entity.hasSignalStep)
            {
                entity.isPrepareStepState = true;
            }

            if (entity.hasSignalLocalMotion)
            {
                entity.isPrepareLocalMotionState = true;
            }

            if (entity.hasSignalDefense)
            {
                entity.isPrepareDefenseState = true;
            }
        }
    }

    public void Cleanup()
    {
        foreach (var motion in _motionGroup)
        {
            if (!motion.hasLinkCharacter)
                continue;
            var entity = motion.linkCharacter.Character;
            float leftSignalTime = 0;
            float deltaTime = _timeService.DeltaTime;

            if (entity.hasSignalAttack)
            {
                leftSignalTime = entity.signalAttack.duration - deltaTime;
                if (leftSignalTime < 0)
                    entity.RemoveSignalAttack();
                else
                    entity.ReplaceSignalAttack(leftSignalTime);
            }

            if (entity.hasSignalJump)
            {
                leftSignalTime = entity.signalJump.duration - deltaTime;
                if (leftSignalTime < 0)
                    entity.RemoveSignalJump();
                else
                    entity.ReplaceSignalJump(leftSignalTime);
            }

            if (entity.hasSignalStep)
            {
                leftSignalTime = entity.signalStep.duration - deltaTime;
                if (leftSignalTime < 0)
                    entity.RemoveSignalStep();
                else
                    entity.ReplaceSignalStep(leftSignalTime);
            }

            if (entity.hasSignalLocalMotion)
            {
                leftSignalTime = entity.signalLocalMotion.duration - deltaTime;
                if (leftSignalTime < 0)
                    entity.RemoveSignalLocalMotion();
                else
                    entity.ReplaceSignalLocalMotion(leftSignalTime);
            }

            if (entity.hasSignalDefense)
            {
                leftSignalTime = entity.signalDefense.time - deltaTime;
                if (leftSignalTime < 0)
                {
                    entity.RemoveSignalDefense();
                    WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Defense,
                        InputSignalEvent.WasReleased, new WTrigger.Context(entity.entityID.id));
                }
                else
                    entity.ReplaceSignalDefense(leftSignalTime);
            }
            
        }
    }
}
