//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class SensorEntity {

    public DetectCharDegreeAngle detectCharDegreeAngle { get { return (DetectCharDegreeAngle)GetComponent(SensorComponentsLookup.DetectCharDegreeAngle); } }
    public bool hasDetectCharDegreeAngle { get { return HasComponent(SensorComponentsLookup.DetectCharDegreeAngle); } }

    public void AddDetectCharDegreeAngle(float newWarning, float newSpotted) {
        var index = SensorComponentsLookup.DetectCharDegreeAngle;
        var component = (DetectCharDegreeAngle)CreateComponent(index, typeof(DetectCharDegreeAngle));
        component.warning = newWarning;
        component.spotted = newSpotted;
        AddComponent(index, component);
    }

    public void ReplaceDetectCharDegreeAngle(float newWarning, float newSpotted) {
        var index = SensorComponentsLookup.DetectCharDegreeAngle;
        var component = (DetectCharDegreeAngle)CreateComponent(index, typeof(DetectCharDegreeAngle));
        component.warning = newWarning;
        component.spotted = newSpotted;
        ReplaceComponent(index, component);
    }

    public void RemoveDetectCharDegreeAngle() {
        RemoveComponent(SensorComponentsLookup.DetectCharDegreeAngle);
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

    static Entitas.IMatcher<SensorEntity> _matcherDetectCharDegreeAngle;

    public static Entitas.IMatcher<SensorEntity> DetectCharDegreeAngle {
        get {
            if (_matcherDetectCharDegreeAngle == null) {
                var matcher = (Entitas.Matcher<SensorEntity>)Entitas.Matcher<SensorEntity>.AllOf(SensorComponentsLookup.DetectCharDegreeAngle);
                matcher.componentNames = SensorComponentsLookup.componentNames;
                _matcherDetectCharDegreeAngle = matcher;
            }

            return _matcherDetectCharDegreeAngle;
        }
    }
}
