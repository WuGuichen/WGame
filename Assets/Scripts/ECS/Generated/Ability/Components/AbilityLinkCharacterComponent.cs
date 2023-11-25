//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class AbilityEntity {

    public LinkCharacterComponent linkCharacter { get { return (LinkCharacterComponent)GetComponent(AbilityComponentsLookup.LinkCharacter); } }
    public bool hasLinkCharacter { get { return HasComponent(AbilityComponentsLookup.LinkCharacter); } }

    public void AddLinkCharacter(GameEntity newCharacter) {
        var index = AbilityComponentsLookup.LinkCharacter;
        var component = (LinkCharacterComponent)CreateComponent(index, typeof(LinkCharacterComponent));
        component.Character = newCharacter;
        AddComponent(index, component);
    }

    public void ReplaceLinkCharacter(GameEntity newCharacter) {
        var index = AbilityComponentsLookup.LinkCharacter;
        var component = (LinkCharacterComponent)CreateComponent(index, typeof(LinkCharacterComponent));
        component.Character = newCharacter;
        ReplaceComponent(index, component);
    }

    public void RemoveLinkCharacter() {
        RemoveComponent(AbilityComponentsLookup.LinkCharacter);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class AbilityEntity : ILinkCharacterEntity { }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class AbilityMatcher {

    static Entitas.IMatcher<AbilityEntity> _matcherLinkCharacter;

    public static Entitas.IMatcher<AbilityEntity> LinkCharacter {
        get {
            if (_matcherLinkCharacter == null) {
                var matcher = (Entitas.Matcher<AbilityEntity>)Entitas.Matcher<AbilityEntity>.AllOf(AbilityComponentsLookup.LinkCharacter);
                matcher.componentNames = AbilityComponentsLookup.componentNames;
                _matcherLinkCharacter = matcher;
            }

            return _matcherLinkCharacter;
        }
    }
}
