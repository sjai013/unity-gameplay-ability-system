using Unity.Entities;

namespace GameplayAbilitySystem.Attributes.Components {

    /// <summary>
    /// Attribute types should implement this interface.
    /// 
    /// See <see cref="GameplayAbilitySystem.Attributes.Components.AttributeModifier{TOper, TAttribute}"/>
    /// for details on specifying how the attribute will affect attribute values.
    /// </summary>
    public interface IAttributeComponent {
        float BaseValue { get; set; }
        float CurrentValue { get; set; }
    }

    /// <summary>
    /// Operators are used to control how different modifiers interact.
    /// New classes can implement this interface, and then specify the implementation
    /// in a custom AttributeSystem
    /// </summary>
    public interface IAttributeOperator { }

    /// <summary>
    /// Attribute modifiers are tagged with the <see cref="AttributeComponentTag{TAttributeComponent}"/> component.
    /// The presence of this tag is essential for the attribute modifier to be counted.
    /// </summary>
    public struct AttributeComponentTag<TAttributeComponent> : IComponentData
    where TAttributeComponent : struct, IAttributeComponent, IComponentData { }

}

namespace GameplayAbilitySystem.Attributes.Components.Operators {

    /// <summary>
    /// The Add operator is used to indicate that these modifiers should be added to the attribute
    /// </summary>
    public struct Add : IAttributeOperator, IComponentData { }
    /// <summary>
    /// The Multiply operator is used to indicate that these modifiers should be multiplied to the base attribute
    /// </summary>
    public struct Multiply : IAttributeOperator, IComponentData { }
    /// <summary>
    /// The Divide operator is used to indicate that these modifiers should be divided from the base attribute. 
    /// </summary>
    public struct Divide : IAttributeOperator, IComponentData { }
}