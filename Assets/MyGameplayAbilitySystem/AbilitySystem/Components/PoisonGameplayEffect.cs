using GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;

public struct PoisonGameplayEffect : IGameplayEffectComponent, IComponentData
{
    public float DamagePerTick;
}