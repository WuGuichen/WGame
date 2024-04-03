using Entitas;

public class ProcessAbilitySystem : IExecuteSystem
{
    private readonly IGroup<AbilityEntity> _abilityGroup;
    private readonly ITimeService _timeService;

    public ProcessAbilitySystem(Contexts contexts)
    {
        _abilityGroup = contexts.ability.GetGroup(AbilityMatcher.AbilityService);
        _timeService = contexts.meta.timeService.instance;
    }
    
    public void Execute()
    {
        foreach (var ability in _abilityGroup)
        {
            if (ability.hasLinkCharacter)
            {
                ability.abilityService.service.Process(_timeService.DeltaTime(ability.linkCharacter.Character.characterTimeScale.rate));
            }
            else
            {
                ability.abilityService.service.Process(_timeService.TimeDeltaTime);
            }
            ability.abilityService.service.BuffManager.Update(_timeService.TimeDeltaTime);
        }
    }
}
