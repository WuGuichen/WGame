//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly SignalStateComponent signalStateComponent = new SignalStateComponent();

    public bool isSignalState {
        get { return HasComponent(GameComponentsLookup.SignalState); }
        set {
            if (value != isSignalState) {
                var index = GameComponentsLookup.SignalState;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : signalStateComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
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

    static Entitas.IMatcher<GameEntity> _matcherSignalState;

    public static Entitas.IMatcher<GameEntity> SignalState {
        get {
            if (_matcherSignalState == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.SignalState);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherSignalState = matcher;
            }

            return _matcherSignalState;
        }
    }
}
