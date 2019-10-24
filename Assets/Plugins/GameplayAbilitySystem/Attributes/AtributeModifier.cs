using Unity.Entities;

namespace GameplayAbilitySystem.Attributes.Components {
    /// <summary>
    /// Attribute modification struct.  By adding this struct
    /// to an entity, you are declaring that the attribute
    /// <see cref="TAttribute" /> should be modified.  
    /// The implementation of how the operator <see cref="TOper" />
    /// affects the final value is left to the system that will
    /// operate on this to decide.
    /// 
    /// You should declare the different combinations in the
    /// component that houses the <see cref="TAttribute" /> component
    /// using (<see cref="Unity.Entities.RegisterGenericComponentTypeAttribute" />).
    /// 
    /// E.g:
    /// <code>
    ///  [assembly: RegisterGenericComponentType(&lt;AtributeModifierOperators.Add, HealthAttributeComponent&gt;))]
    /// </code>
    /// </summary>
    /// <typeparam name="TOper">The element type of the array</typeparam>
    /// <typeparam name="TAttribute">The element type of the array</typeparam>
    public struct AttributeModifier<TOper, TAttribute> : IComponentData
    where TOper : struct, IAttributeOperator
    where TAttribute : struct, IAttributeComponent {
        public float Value;
        public static implicit operator float(AttributeModifier<TOper, TAttribute> e) { return e.Value; }
        public static implicit operator AttributeModifier<TOper, TAttribute>(float e) { return new AttributeModifier<TOper, TAttribute> { Value = e }; }
    }
}