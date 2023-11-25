//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class WeaponEntity {

    public WeaponStateComponent weaponState { get { return (WeaponStateComponent)GetComponent(WeaponComponentsLookup.WeaponState); } }
    public bool hasWeaponState { get { return HasComponent(WeaponComponentsLookup.WeaponState); } }

    public void AddWeaponState(int newState) {
        var index = WeaponComponentsLookup.WeaponState;
        var component = (WeaponStateComponent)CreateComponent(index, typeof(WeaponStateComponent));
        component.state = newState;
        AddComponent(index, component);
    }

    public void ReplaceWeaponState(int newState) {
        var index = WeaponComponentsLookup.WeaponState;
        var component = (WeaponStateComponent)CreateComponent(index, typeof(WeaponStateComponent));
        component.state = newState;
        ReplaceComponent(index, component);
    }

    public void RemoveWeaponState() {
        RemoveComponent(WeaponComponentsLookup.WeaponState);
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
public sealed partial class WeaponMatcher {

    static Entitas.IMatcher<WeaponEntity> _matcherWeaponState;

    public static Entitas.IMatcher<WeaponEntity> WeaponState {
        get {
            if (_matcherWeaponState == null) {
                var matcher = (Entitas.Matcher<WeaponEntity>)Entitas.Matcher<WeaponEntity>.AllOf(WeaponComponentsLookup.WeaponState);
                matcher.componentNames = WeaponComponentsLookup.componentNames;
                _matcherWeaponState = matcher;
            }

            return _matcherWeaponState;
        }
    }
}
