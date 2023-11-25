using Entitas;

public class ProcessMotionSignalUpdateSystem : IExecuteSystem
{
    private readonly IGroup<MotionEntity> _motionGroup;
    
    public ProcessMotionSignalUpdateSystem(Contexts contexts)
    {
        _motionGroup = contexts.motion.GetGroup(MotionMatcher.MotionService);
    }


    public void Execute()
    {
        foreach (var entity in _motionGroup)
        {
            if (!entity.hasLinkCharacter)
                continue;
            var character = entity.linkCharacter.Character;

            // bool isSignal = false;
            if (character.hasSignalAttack)
            {
                if (entity.hasMotionJump && entity.motionStart.UID == entity.motionJump.UID)
                    character.isPrepareJumpAttackState = true;
                else
                    character.isPrepareAttackState = true;
                // isSignal = true;
            }

            if (character.hasSignalJump)
            {
                character.isPrepareJumpState = true;
                // isSignal = true;
            }

            if (character.hasSignalStep)
            {
                character.isPrepareStepState = true;
                // isSignal = true;
            }

            if (character.hasSignalLocalMotion)
            {
                character.isPrepareLocalMotionState = true;
                // isSignal = true;
            }

            // character.isStateChanged = false;

            // if (isSignal)
            // {
            //     // character.animatorService.service.SwitchAnimState();
            //     // character.isStateSwitchState = true;
            //     entity.motionService.service.StartMotion();
            // }

        }
    }
}
