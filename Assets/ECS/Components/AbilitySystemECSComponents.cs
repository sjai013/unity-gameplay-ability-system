using GameplayAbilitySystem.Abilities.Fire;
using GameplayAbilitySystem.Abilities.Heal;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Used for implementing periodic gameplay effects
/// TODO: Create sample periodic gameplay effect component as a prototype 
/// TODO:   -> decrement PeriodRemaining.  
/// TODO:   -> Apply GE when PeriodRemaining 0.  
/// TODO:   -> Destroy Entity when ParentGameplayEffect cooldown = 0;
/// </summary>
public struct PeriodicGameplayEffectComponent : IComponentData {
    public Entity ParentGameplayEffect;
    public float Period;
    public float PeriodRemaining;
    public bool ExecuteOnApplication;
}
