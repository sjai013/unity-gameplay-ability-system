using Unity.Collections;

namespace GameplayAbilitySystem.AttributeSystem.Components
{
    public interface IAttributeExecute<TAttribute, TInstantAttributeModifier, TDurationalAttributeModifier>
    where TAttribute : struct, IAttributeData
    where TInstantAttributeModifier : struct, IAttributeModifier
    where TDurationalAttributeModifier : struct, IAttributeModifier
    {
        void CalculateInstant(NativeArray<TAttribute> arg0, NativeArray<TInstantAttributeModifier> arg1);
        void CalculateDurational(NativeArray<TAttribute> arg0, NativeArray<TDurationalAttributeModifier> arg1);
    }
}
