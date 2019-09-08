using System;
using GameplayAbilitySystem.Abilities.Fire;
using GameplayAbilitySystem.Abilities.Heal;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Marks the ability system to check if the ability can be activated
/// </summary>
struct CheckAbilityConstraintsComponent : IComponentData { }

/// <summary>
/// Marks the ability system to commit the ability and activate it
/// </summary>
struct CommitJobComponent : IComponentData { }

/// <summary>
/// Internal mark to check when the CommitJobComponent is first added
/// </summary>
struct CommitJobSystemComponent : ISystemStateComponentData { }

/// <summary>
/// /// Tag to indicate the entity is currently casting, and therefore not able to cast
/// </summary>
struct CastingAbilityTagComponent : IComponentData { }

/// <summary>
/// Tag to indicate the ability should end
/// </summary>
struct EndAbilityComponent : IComponentData { }

/// <summary>
/// Tag to indicate this entity is a cooldown component
/// </summary>
public struct CooldownEffectComponent : IComponentData {
    public Entity Caster;
}


/// <summary>
/// Provides collection of functionality all game effects need to have
/// </summary>
public interface IGameplayEffect {
    void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
    void ApplyGameplayEffect(EntityManager EntityManager, Entity Source, Entity Target, AttributesComponent attributesComponent);

    DurationPolicyComponent DurationPolicy { get; set; }
}

public struct DurationPolicyComponent {
    public EDurationPolicy DurationPolicy;
    public float Duration;
}
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

/// <summary>
/// Provides collection of functionality that cooldowns need to have
/// </summary>
public interface ICooldown {
    void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);
    EGameplayEffect GameplayEffect { get; }
}

/// <summary>
/// Provides collection of functionality that cost effects need to have
/// </summary>
public interface ICost : IGameplayEffect {
    AttributesComponent ComputeResourceUsage(Entity Caster, AttributesComponent attributes);
}


public interface ICooldownSystemComponentDefinition {
    EntityQueryDesc CooldownQueryDesc { get; }
}


public struct CooldownTimeCaster {
    public Entity Caster;
    public float TimeRemaining;
    public float Duration;
}

public struct AbilityCooldownComponent : IComponentData {
    public float TimeRemaining;
    public float Duration;
    public bool CooldownActivated;
}
public interface ICooldownJob {
    NativeArray<CooldownTimeCaster> CooldownArray { get; set; }
}

public struct AbilityStateComponent : IComponentData {
    public EAbilityState State;
}

/// <summary>
/// Provides collection of functionality all abilities need to have
/// </summary>
public interface IAbilityBehaviour {

    /// <summary>
    /// Application of costs associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Source"></param>
    /// <param name="Target"></param>
    /// <param name="attributesComponent"></param>
    void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);

    /// <summary>
    /// Application of gameplay effects associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Source"></param>
    /// <param name="Target"></param>
    /// <param name="attributesComponent"></param>
    void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
    void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent);

    /// <summary>
    /// Check for resource availability associated with ability
    /// </summary>
    /// <param name="Caster"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    bool CheckResourceAvailable(ref Entity Caster, ref AttributesComponent attributes);

    /// <summary>
    /// Application of cooldown effects associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Caster"></param>
    /// <param name="WorldTime"></param>
    void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);
    EAbility AbilityType { get; }

    EGameplayEffect[] CooldownEffects { get; }

    JobHandle BeginAbilityCastJob(JobComponentSystem system, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, ComponentDataFromEntity<AttributesComponent> attributesComponent, float WorldTime);
    JobHandle UpdateCooldownsJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownsRemainingForAbility);
    JobHandle CheckAbilityAvailableJob(JobComponentSystem system, JobHandle inputDeps, ComponentDataFromEntity<AttributesComponent> attributesComponent);
    JobHandle CheckAbilityGrantedJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, bool> AbilityGranted);
}

[RequireComponentTag(typeof(AbilityComponent))]
public struct GenericBeginAbilityCast<T1> : IJobForEachWithEntity<AbilityStateComponent, AbilitySourceTarget, T1>
where T1 : struct, IComponentData, IAbilityBehaviour {
    public EntityCommandBuffer.Concurrent Ecb;
    public float WorldTime;
    [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
    public void Execute(Entity entity, int index, [ReadOnly] ref AbilityStateComponent abilityStateComponent, [ReadOnly] ref AbilitySourceTarget abilitySourceTarget, [ReadOnly] ref T1 ability) {
        if (abilityStateComponent.State != EAbilityState.PreActivate) return;
        ability.ApplyCooldownEffect(index, Ecb, abilitySourceTarget.Source, WorldTime);
        ability.ApplyAbilityCosts(index, Ecb, abilitySourceTarget.Source, abilitySourceTarget.Target, attributesComponent[abilitySourceTarget.Source]);
        abilityStateComponent.State = EAbilityState.Activate;
    }
}

[RequireComponentTag(typeof(AbilityStateComponent))]
public struct _GenericUpdateAbilityCooldownJob<T1> : IJobForEach<AbilitySourceTarget, AbilityCooldownComponent>
where T1 : struct, IComponentData, IAbilityBehaviour {
    [ReadOnly] public NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownsRemainingForAbility;
    public void Execute([ReadOnly] ref AbilitySourceTarget abilitySourceTarget, ref AbilityCooldownComponent cooldown) {
        cooldownsRemainingForAbility.TryGetValue(abilitySourceTarget.Source, out var duration);
        cooldown.Duration = duration.Duration;
        cooldown.TimeRemaining = duration.TimeRemaining;
        cooldown.CooldownActivated = cooldown.Duration > 0 || cooldown.CooldownActivated;

    }
}

public struct GenericUpdateAbilityCooldownJob<T1> : IJobForEach<GrantedAbilityComponent, GrantedAbilityCooldownComponent, T1>
where T1 : struct, IComponentData, IAbilityBehaviour {
    [ReadOnly] public NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownsRemainingForAbility;
    public void Execute([ReadOnly] ref GrantedAbilityComponent grantedAbility, ref GrantedAbilityCooldownComponent cooldown, [ReadOnly] ref T1 ability) {
        cooldownsRemainingForAbility.TryGetValue(grantedAbility.GrantedTo, out var duration);
        cooldown.Duration = duration.Duration;
        cooldown.TimeRemaining = duration.TimeRemaining;
    }
}

[BurstCompile]
[RequireComponentTag(typeof(AbilityComponent))]
public struct GenericCheckResourceForAbilityJob<T1> : IJobForEach<AbilitySourceTarget, AbilityCooldownComponent, AbilityStateComponent, T1>
where T1 : struct, IComponentData, IAbilityBehaviour {
    [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
    public void Execute(ref AbilitySourceTarget abilitySourceTarget, ref AbilityCooldownComponent cooldown, ref AbilityStateComponent state, [ReadOnly] ref T1 ability) {
        if (state.State != EAbilityState.CheckResource) return;
        var resourceAvailable = false;
        var sourceAttrs = attributesComponent[abilitySourceTarget.Source];
        resourceAvailable = ability.CheckResourceAvailable(ref abilitySourceTarget.Source, ref sourceAttrs);
        if (resourceAvailable) {
            state.State = EAbilityState.PreActivate;
        } else {
            state.State = EAbilityState.Failed;
        }
    }
}

[BurstCompile]
[RequireComponentTag(typeof(GrantedAbilityComponent), typeof(GrantedAbilityCooldownComponent))]
public struct GenericCheckAbilityGrantedJob<T1> : IJobForEachWithEntity<T1>
where T1 : struct, IComponentData, IAbilityBehaviour {

    [WriteOnly] public NativeHashMap<Entity, bool>.ParallelWriter AbilityGranted;

    public void Execute(Entity entity, int index, [ReadOnly] ref T1 Ability) {
        AbilityGranted.TryAdd(entity, true);
    }
}

public struct AbilityComponent : IComponentData, IEquatable<AbilityComponent> {
    public EAbility Ability;

    public bool Equals(AbilityComponent other) {
        if (Ability == other.Ability) return true;
        return false;
    }

    public static implicit operator EAbility(AbilityComponent e) {
        return e;
    }

    public static implicit operator AbilityComponent(EAbility e) {
        return new AbilityComponent
        {
            Ability = e
        };
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + (int)Ability.GetHashCode();
            return hash;
        }
    }
}

public enum EAbilityState { TryActivate, CheckCooldown, CheckResource, PreActivate, Activate, Active, Activated, Completed, Failed }

public enum EAbility {
    FireAbility,
    HealAbility

}

public enum EGameplayEffect {
    NullCooldown,
    GlobalCooldown,
    FireAbilityCooldown,
    HealAbilityCooldown
}

public struct GameplayeffectComponent : IComponentData {
    public EGameplayEffect Effect;
}

public enum EDurationPolicy {
    Instant, Infinite, HasDuration
}

public struct AbilitySourceTarget : IComponentData {
    public Entity Source;
    public Entity Target;

}


public struct GrantedAbilityComponent : IComponentData {
    public Entity GrantedTo;
}