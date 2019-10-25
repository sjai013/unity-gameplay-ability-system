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
    public interface IAttributeOperator { }
    public struct AttributeComponentTag<TAttributeComponent> : IComponentData
    where TAttributeComponent : struct, IAttributeComponent, IComponentData { }

}

namespace GameplayAbilitySystem.Attributes.Components.Operators {
    public struct Add : IAttributeOperator, IComponentData { }
    public struct Multiply : IAttributeOperator, IComponentData { }
    public struct Divide : IAttributeOperator, IComponentData { }
}