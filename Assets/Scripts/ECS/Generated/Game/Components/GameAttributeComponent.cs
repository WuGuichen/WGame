//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public AttributeComponent attribute { get { return (AttributeComponent)GetComponent(GameComponentsLookup.Attribute); } }
    public bool hasAttribute { get { return HasComponent(GameComponentsLookup.Attribute); } }

    public void AddAttribute(WGame.Attribute.WAttribute newValue) {
        var index = GameComponentsLookup.Attribute;
        var component = (AttributeComponent)CreateComponent(index, typeof(AttributeComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceAttribute(WGame.Attribute.WAttribute newValue) {
        var index = GameComponentsLookup.Attribute;
        var component = (AttributeComponent)CreateComponent(index, typeof(AttributeComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveAttribute() {
        RemoveComponent(GameComponentsLookup.Attribute);
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

    static Entitas.IMatcher<GameEntity> _matcherAttribute;

    public static Entitas.IMatcher<GameEntity> Attribute {
        get {
            if (_matcherAttribute == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Attribute);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAttribute = matcher;
            }

            return _matcherAttribute;
        }
    }
}
