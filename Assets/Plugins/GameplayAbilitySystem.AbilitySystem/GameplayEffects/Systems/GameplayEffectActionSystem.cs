using System;
using GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components;
using GameplayAbilitySystem.AttributeSystem.Components;
using Unity.Entities;

namespace GameplayAbilitySystem.AbilitySystem.GameplayEffects.Systems
{
    public abstract class GameplayEffectActionSystem<TGameplayEffect> : SystemBase
    where TGameplayEffect : struct, IComponentData, IGameplayEffectComponent
    {

    }

}