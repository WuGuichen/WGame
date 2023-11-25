//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public AttackForceFwdComponent attackForceFwd { get { return (AttackForceFwdComponent)GetComponent(GameComponentsLookup.AttackForceFwd); } }
    public bool hasAttackForceFwd { get { return HasComponent(GameComponentsLookup.AttackForceFwd); } }

    public void AddAttackForceFwd(float newValue) {
        var index = GameComponentsLookup.AttackForceFwd;
        var component = (AttackForceFwdComponent)CreateComponent(index, typeof(AttackForceFwdComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceAttackForceFwd(float newValue) {
        var index = GameComponentsLookup.AttackForceFwd;
        var component = (AttackForceFwdComponent)CreateComponent(index, typeof(AttackForceFwdComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveAttackForceFwd() {
        RemoveComponent(GameComponentsLookup.AttackForceFwd);
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

    static Entitas.IMatcher<GameEntity> _matcherAttackForceFwd;

    public static Entitas.IMatcher<GameEntity> AttackForceFwd {
        get {
            if (_matcherAttackForceFwd == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AttackForceFwd);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAttackForceFwd = matcher;
            }

            return _matcherAttackForceFwd;
        }
    }
}
