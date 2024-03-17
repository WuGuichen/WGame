//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class SensorEntity {

    public LifeTimeComponent lifeTime { get { return (LifeTimeComponent)GetComponent(SensorComponentsLookup.LifeTime); } }
    public bool hasLifeTime { get { return HasComponent(SensorComponentsLookup.LifeTime); } }

    public void AddLifeTime(float newValue) {
        var index = SensorComponentsLookup.LifeTime;
        var component = (LifeTimeComponent)CreateComponent(index, typeof(LifeTimeComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceLifeTime(float newValue) {
        var index = SensorComponentsLookup.LifeTime;
        var component = (LifeTimeComponent)CreateComponent(index, typeof(LifeTimeComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveLifeTime() {
        RemoveComponent(SensorComponentsLookup.LifeTime);
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
public sealed partial class SensorMatcher {

    static Entitas.IMatcher<SensorEntity> _matcherLifeTime;

    public static Entitas.IMatcher<SensorEntity> LifeTime {
        get {
            if (_matcherLifeTime == null) {
                var matcher = (Entitas.Matcher<SensorEntity>)Entitas.Matcher<SensorEntity>.AllOf(SensorComponentsLookup.LifeTime);
                matcher.componentNames = SensorComponentsLookup.componentNames;
                _matcherLifeTime = matcher;
            }

            return _matcherLifeTime;
        }
    }
}
