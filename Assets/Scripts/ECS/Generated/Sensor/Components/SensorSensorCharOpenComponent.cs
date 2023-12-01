//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class SensorEntity {

    static readonly SensorCharOpen sensorCharOpenComponent = new SensorCharOpen();

    public bool isSensorCharOpen {
        get { return HasComponent(SensorComponentsLookup.SensorCharOpen); }
        set {
            if (value != isSensorCharOpen) {
                var index = SensorComponentsLookup.SensorCharOpen;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : sensorCharOpenComponent;

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
public sealed partial class SensorMatcher {

    static Entitas.IMatcher<SensorEntity> _matcherSensorCharOpen;

    public static Entitas.IMatcher<SensorEntity> SensorCharOpen {
        get {
            if (_matcherSensorCharOpen == null) {
                var matcher = (Entitas.Matcher<SensorEntity>)Entitas.Matcher<SensorEntity>.AllOf(SensorComponentsLookup.SensorCharOpen);
                matcher.componentNames = SensorComponentsLookup.componentNames;
                _matcherSensorCharOpen = matcher;
            }

            return _matcherSensorCharOpen;
        }
    }
}
