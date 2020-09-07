using GameplayAbilitySystem.AttributeSystem.Components;
using GameplayAbilitySystem.AttributeSystem.Systems;
using Unity.Entities;

namespace MyGameplayAbilitySystem
{
    [UpdateBefore(typeof(MyAttributeUpdateSystem))]
    public class MyDurationalAttributeUpdateSystem
        : AttributeModifierCollectionSystem<MyDurationalAttributeModifierValues, MyDurationalGameplayAttributeModifier, DurationalAttributeModifierComponentTag>
    { }
}
