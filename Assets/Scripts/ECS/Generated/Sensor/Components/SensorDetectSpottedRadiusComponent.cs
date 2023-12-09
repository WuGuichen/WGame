//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class SensorEntity {

    public DetectSpottedRadiusComponent detectSpottedRadius { get { return (DetectSpottedRadiusComponent)GetComponent(SensorComponentsLookup.DetectSpottedRadius); } }
    public bool hasDetectSpottedRadius { get { return HasComponent(SensorComponentsLookup.DetectSpottedRadius); } }

    public void AddDetectSpottedRadius(float newValue) {
        var index = SensorComponentsLookup.DetectSpottedRadius;
        var component = (DetectSpottedRadiusComponent)CreateComponent(index, typeof(DetectSpottedRadiusComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceDetectSpottedRadius(float newValue) {
        var index = SensorComponentsLookup.DetectSpottedRadius;
        var component = (DetectSpottedRadiusComponent)CreateComponent(index, typeof(DetectSpottedRadiusComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveDetectSpottedRadius() {
        RemoveComponent(SensorComponentsLookup.DetectSpottedRadius);
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

    static Entitas.IMatcher<SensorEntity> _matcherDetectSpottedRadius;

    public static Entitas.IMatcher<SensorEntity> DetectSpottedRadius {
        get {
            if (_matcherDetectSpottedRadius == null) {
                var matcher = (Entitas.Matcher<SensorEntity>)Entitas.Matcher<SensorEntity>.AllOf(SensorComponentsLookup.DetectSpottedRadius);
                matcher.componentNames = SensorComponentsLookup.componentNames;
                _matcherDetectSpottedRadius = matcher;
            }

            return _matcherDetectSpottedRadius;
        }
    }
}