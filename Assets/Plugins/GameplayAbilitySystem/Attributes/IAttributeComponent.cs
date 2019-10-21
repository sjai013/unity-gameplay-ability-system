using Unity.Entities;

namespace GameplayAbilitySystem.Attributes.Components {

    /// <summary>
    /// Attribute types should implement this (empty) interface.
    /// 
    /// See <see cref="GameplayAbilitySystem.Attributes.Components.AttributeModifier{TOper, TAttribute}"/>
    /// for details on specifying how the attribute will affect attribute values.
    /// </summary>
    public interface IAttributeComponent { }
    public interface IAttributeOperator { }

}

namespace GameplayAbilitySystem.Attributes.Components.Operators {
    public struct Add : IAttributeOperator, IComponentData { }
    public struct Multiply : IAttributeOperator, IComponentData { }
    public struct Divide : IAttributeOperator, IComponentData { }
}