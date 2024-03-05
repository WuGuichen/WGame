//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class AbilityEntity {

    public AbilityServiceComponent abilityService { get { return (AbilityServiceComponent)GetComponent(AbilityComponentsLookup.AbilityService); } }
    public bool hasAbilityService { get { return HasComponent(AbilityComponentsLookup.AbilityService); } }

    public void AddAbilityService(IAbility newService) {
        var index = AbilityComponentsLookup.AbilityService;
        var component = (AbilityServiceComponent)CreateComponent(index, typeof(AbilityServiceComponent));
        component.service = newService;
        AddComponent(index, component);
    }

    public void ReplaceAbilityService(IAbility newService) {
        var index = AbilityComponentsLookup.AbilityService;
        var component = (AbilityServiceComponent)CreateComponent(index, typeof(AbilityServiceComponent));
        component.service = newService;
        ReplaceComponent(index, component);
    }

    public void RemoveAbilityService() {
        RemoveComponent(AbilityComponentsLookup.AbilityService);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class AbilityMatcher {

    static Entitas.IMatcher<AbilityEntity> _matcherAbilityService;

    public static Entitas.IMatcher<AbilityEntity> AbilityService {
        get {
            if (_matcherAbilityService == null) {
                var matcher = (Entitas.Matcher<AbilityEntity>)Entitas.Matcher<AbilityEntity>.AllOf(AbilityComponentsLookup.AbilityService);
                matcher.componentNames = AbilityComponentsLookup.componentNames;
                _matcherAbilityService = matcher;
            }

            return _matcherAbilityService;
        }
    }
}
