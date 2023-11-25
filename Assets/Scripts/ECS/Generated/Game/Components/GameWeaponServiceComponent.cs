//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public WeaponServiceComponent weaponService { get { return (WeaponServiceComponent)GetComponent(GameComponentsLookup.WeaponService); } }
    public bool hasWeaponService { get { return HasComponent(GameComponentsLookup.WeaponService); } }

    public void AddWeaponService(IWeaponService newService) {
        var index = GameComponentsLookup.WeaponService;
        var component = (WeaponServiceComponent)CreateComponent(index, typeof(WeaponServiceComponent));
        component.service = newService;
        AddComponent(index, component);
    }

    public void ReplaceWeaponService(IWeaponService newService) {
        var index = GameComponentsLookup.WeaponService;
        var component = (WeaponServiceComponent)CreateComponent(index, typeof(WeaponServiceComponent));
        component.service = newService;
        ReplaceComponent(index, component);
    }

    public void RemoveWeaponService() {
        RemoveComponent(GameComponentsLookup.WeaponService);
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
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherWeaponService;

    public static Entitas.IMatcher<GameEntity> WeaponService {
        get {
            if (_matcherWeaponService == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.WeaponService);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherWeaponService = matcher;
            }

            return _matcherWeaponService;
        }
    }
}
