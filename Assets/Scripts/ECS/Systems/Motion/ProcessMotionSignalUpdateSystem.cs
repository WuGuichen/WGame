using Entitas;
using WGame.Ability;
using WGame.Trigger;

public class ProcessMotionSignalUpdateSystem : IExecuteSystem, ICleanupSystem
{
    private readonly IGroup<MotionEntity> _motionGroup;
    private readonly ITimeService _timeService;
    private readonly InputContext _inputContext;
    
    public ProcessMotionSignalUpdateSystem(Contexts contexts)
    {
        _motionGroup = contexts.motion.GetGroup(MotionMatcher.MotionService);
        _timeService = contexts.meta.timeService.instance;
        _inputContext = contexts.input;
    }


    public void Execute()
    {
        foreach (var motion in _motionGroup)
        {
            if (!motion.hasLinkCharacter)
                continue;
            var entity = motion.linkCharacter.Character;
            var ability = entity.linkAbility.Ability.abilityService.service;
            var inputState = entity.signalState.state;
            bool notChange = true;

            if (entity.hasSignalAttack)
            {
                inputState.EnableState(InputType.Attack);
                if (notChange && ability.Owner.TryGetNextAbilityID(InputType.Attack, out var id, out var motionType))
                {
                    motion.motionService.service.TransMotionByMotionType(motionType);
                    notChange = false;
                }
            }

            if (entity.hasSignalJump)
            {
                inputState.EnableState(InputType.Jump);
                if (notChange && ability.Owner.TryGetNextAbilityID(InputType.Jump, out var id, out var motionType))
                {
                    motion.motionService.service.TransMotionByMotionType(motionType);
                    notChange = false;
                }
            }

            if (entity.hasSignalStep)
            {
                inputState.EnableState(InputType.Step);
                if (notChange && ability.Owner.TryGetNextAbilityID(InputType.Step, out var id, out var motionType))
                {
                    motion.motionService.service.TransMotionByMotionType(motionType);
                    notChange = false;
                }
            }

            if (entity.hasSignalLocalMotion)
            {
                inputState.EnableState(InputType.LocalMotion);
                if (!motion.motionService.service.CheckMotionType(MotionType.LocalMotion))
                {
                    if (notChange &&
                        ability.Owner.TryGetNextAbilityID(InputType.LocalMotion, out var id, out var motionType))
                    {
                        motion.motionService.service.TransMotionByMotionType(motionType);
                        notChange = false;
                    }
                }
            }

            if (entity.hasSignalDefense)
            { 
                inputState.EnableState(InputType.Defense);
                if (notChange && ability.Owner.TryGetNextAbilityID(InputType.Defense, out var id, out var motionType))
                {
                    motion.motionService.service.TransMotionByMotionType(motionType);
                    notChange = false;
                }
            }
            
            inputState.CheckStateChange();
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
            float deltaTime = _timeService.DeltaTime(0.2f);

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
                // leftSignalTime = -1;
                // if (leftSignalTime < 0)
                // {
                    entity.RemoveSignalDefense();
                    WTriggerMgr.Inst.TriggerEvent(MainTypeDefine.InputSignal, InputSignalSubType.Defense,
                        InputSignalEvent.WasReleased, new WTrigger.Context(entity.entityID.id));
                // }
                // else
                //     entity.ReplaceSignalDefense(leftSignalTime);
            }
        }
    }
}
