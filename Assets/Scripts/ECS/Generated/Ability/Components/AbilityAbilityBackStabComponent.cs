//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class AbilityEntity {

    public AbilityBackStabComponent abilityBackStab { get { return (AbilityBackStabComponent)GetComponent(AbilityComponentsLookup.AbilityBackStab); } }
    public bool hasAbilityBackStab { get { return HasComponent(AbilityComponentsLookup.AbilityBackStab); } }

    public void AddAbilityBackStab(int newEntityID) {
        var index = AbilityComponentsLookup.AbilityBackStab;
        var component = (AbilityBackStabComponent)CreateComponent(index, typeof(AbilityBackStabComponent));
        component.EntityID = newEntityID;
        AddComponent(index, component);
    }

    public void ReplaceAbilityBackStab(int newEntityID) {
        var index = AbilityComponentsLookup.AbilityBackStab;
        var component = (AbilityBackStabComponent)CreateComponent(index, typeof(AbilityBackStabComponent));
        component.EntityID = newEntityID;
        ReplaceComponent(index, component);
    }

    public void RemoveAbilityBackStab() {
        RemoveComponent(AbilityComponentsLookup.AbilityBackStab);
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

    static Entitas.IMatcher<AbilityEntity> _matcherAbilityBackStab;

    public static Entitas.IMatcher<AbilityEntity> AbilityBackStab {
        get {
            if (_matcherAbilityBackStab == null) {
                var matcher = (Entitas.Matcher<AbilityEntity>)Entitas.Matcher<AbilityEntity>.AllOf(AbilityComponentsLookup.AbilityBackStab);
                matcher.componentNames = AbilityComponentsLookup.componentNames;
                _matcherAbilityBackStab = matcher;
            }

            return _matcherAbilityBackStab;
        }
    }
}