//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class AbilityEntity {

    public AbilityParryAttackComponent abilityParryAttack { get { return (AbilityParryAttackComponent)GetComponent(AbilityComponentsLookup.AbilityParryAttack); } }
    public bool hasAbilityParryAttack { get { return HasComponent(AbilityComponentsLookup.AbilityParryAttack); } }

    public void AddAbilityParryAttack(ParryAttack newValue) {
        var index = AbilityComponentsLookup.AbilityParryAttack;
        var component = (AbilityParryAttackComponent)CreateComponent(index, typeof(AbilityParryAttackComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceAbilityParryAttack(ParryAttack newValue) {
        var index = AbilityComponentsLookup.AbilityParryAttack;
        var component = (AbilityParryAttackComponent)CreateComponent(index, typeof(AbilityParryAttackComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveAbilityParryAttack() {
        RemoveComponent(AbilityComponentsLookup.AbilityParryAttack);
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

    static Entitas.IMatcher<AbilityEntity> _matcherAbilityParryAttack;

    public static Entitas.IMatcher<AbilityEntity> AbilityParryAttack {
        get {
            if (_matcherAbilityParryAttack == null) {
                var matcher = (Entitas.Matcher<AbilityEntity>)Entitas.Matcher<AbilityEntity>.AllOf(AbilityComponentsLookup.AbilityParryAttack);
                matcher.componentNames = AbilityComponentsLookup.componentNames;
                _matcherAbilityParryAttack = matcher;
            }

            return _matcherAbilityParryAttack;
        }
    }
}