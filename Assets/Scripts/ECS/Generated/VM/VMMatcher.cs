//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ContextMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class VMMatcher {

    public static Entitas.IAllOfMatcher<VMEntity> AllOf(params int[] indices) {
        return Entitas.Matcher<VMEntity>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<VMEntity> AllOf(params Entitas.IMatcher<VMEntity>[] matchers) {
          return Entitas.Matcher<VMEntity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<VMEntity> AnyOf(params int[] indices) {
          return Entitas.Matcher<VMEntity>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<VMEntity> AnyOf(params Entitas.IMatcher<VMEntity>[] matchers) {
          return Entitas.Matcher<VMEntity>.AnyOf(matchers);
    }
}
