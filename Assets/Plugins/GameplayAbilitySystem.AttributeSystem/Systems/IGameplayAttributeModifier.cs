using Unity.Entities;

namespace GameplayAbilitySystem.AttributeSystem.Systems
{
    public interface IGameplayAttributeModifier<TAttributeModifier>
    where TAttributeModifier : struct, IAttributeModifier, IComponentData
    {
        void UpdateAttribute(ref TAttributeModifier attributeModifier);
    }
}
