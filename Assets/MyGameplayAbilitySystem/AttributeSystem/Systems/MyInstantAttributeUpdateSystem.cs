using GameplayAbilitySystem.AttributeSystem.Components;
using GameplayAbilitySystem.AttributeSystem.Systems;
using Unity.Entities;

namespace MyGameplayAbilitySystem
{
    [UpdateBefore(typeof(MyDurationalAttributeUpdateSystem))]
    public class MyInstantAttributeUpdateSystem
        : AttributeModifierCollectionSystem<MyInstantAttributeModifierValues, MyInstantGameplayAttributeModifier, InstantAttributeModifierComponentTag>
    {
    }
}
