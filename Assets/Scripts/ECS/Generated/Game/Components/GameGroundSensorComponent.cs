//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public GroundSensor groundSensor { get { return (GroundSensor)GetComponent(GameComponentsLookup.GroundSensor); } }
    public bool hasGroundSensor { get { return HasComponent(GameComponentsLookup.GroundSensor); } }

    public void AddGroundSensor(bool newIntersect) {
        var index = GameComponentsLookup.GroundSensor;
        var component = (GroundSensor)CreateComponent(index, typeof(GroundSensor));
        component.intersect = newIntersect;
        AddComponent(index, component);
    }

    public void ReplaceGroundSensor(bool newIntersect) {
        var index = GameComponentsLookup.GroundSensor;
        var component = (GroundSensor)CreateComponent(index, typeof(GroundSensor));
        component.intersect = newIntersect;
        ReplaceComponent(index, component);
    }

    public void RemoveGroundSensor() {
        RemoveComponent(GameComponentsLookup.GroundSensor);
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

    static Entitas.IMatcher<GameEntity> _matcherGroundSensor;

    public static Entitas.IMatcher<GameEntity> GroundSensor {
        get {
            if (_matcherGroundSensor == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GroundSensor);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherGroundSensor = matcher;
            }

            return _matcherGroundSensor;
        }
    }
}
