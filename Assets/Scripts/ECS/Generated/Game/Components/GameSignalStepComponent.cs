//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public SignalStepComponent signalStep { get { return (SignalStepComponent)GetComponent(GameComponentsLookup.SignalStep); } }
    public bool hasSignalStep { get { return HasComponent(GameComponentsLookup.SignalStep); } }

    public void AddSignalStep(float newDuration) {
        var index = GameComponentsLookup.SignalStep;
        var component = (SignalStepComponent)CreateComponent(index, typeof(SignalStepComponent));
        component.duration = newDuration;
        AddComponent(index, component);
    }

    public void ReplaceSignalStep(float newDuration) {
        var index = GameComponentsLookup.SignalStep;
        var component = (SignalStepComponent)CreateComponent(index, typeof(SignalStepComponent));
        component.duration = newDuration;
        ReplaceComponent(index, component);
    }

    public void RemoveSignalStep() {
        RemoveComponent(GameComponentsLookup.SignalStep);
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

    static Entitas.IMatcher<GameEntity> _matcherSignalStep;

    public static Entitas.IMatcher<GameEntity> SignalStep {
        get {
            if (_matcherSignalStep == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.SignalStep);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherSignalStep = matcher;
            }

            return _matcherSignalStep;
        }
    }
}
