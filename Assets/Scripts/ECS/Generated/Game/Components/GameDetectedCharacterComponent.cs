//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public DetectedCharacterComponent detectedCharacter { get { return (DetectedCharacterComponent)GetComponent(GameComponentsLookup.DetectedCharacter); } }
    public bool hasDetectedCharacter { get { return HasComponent(GameComponentsLookup.DetectedCharacter); } }

    public void AddDetectedCharacter(GameEntity newEntity) {
        var index = GameComponentsLookup.DetectedCharacter;
        var component = (DetectedCharacterComponent)CreateComponent(index, typeof(DetectedCharacterComponent));
        component.entity = newEntity;
        AddComponent(index, component);
    }

    public void ReplaceDetectedCharacter(GameEntity newEntity) {
        var index = GameComponentsLookup.DetectedCharacter;
        var component = (DetectedCharacterComponent)CreateComponent(index, typeof(DetectedCharacterComponent));
        component.entity = newEntity;
        ReplaceComponent(index, component);
    }

    public void RemoveDetectedCharacter() {
        RemoveComponent(GameComponentsLookup.DetectedCharacter);
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

    static Entitas.IMatcher<GameEntity> _matcherDetectedCharacter;

    public static Entitas.IMatcher<GameEntity> DetectedCharacter {
        get {
            if (_matcherDetectedCharacter == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.DetectedCharacter);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherDetectedCharacter = matcher;
            }

            return _matcherDetectedCharacter;
        }
    }
}
