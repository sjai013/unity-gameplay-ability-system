using GameplayAbilitySystem.Abilities.Systems;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public struct TestAbilityTag : IAbilityTagComponent, IComponentData {
    public GameplayEffectDurationComponent _durationComponent;
    public GameplayEffectDurationComponent DurationComponent { get => _durationComponent; set => _durationComponent = value; }
}

public class TestAbilitySystem : AbilitySystem<TestAbilityTag> {

    private EntityQuery CooldownEffectsQuery;
    private EntityQuery GrantedAbilityQuery;
    protected override void OnCreate() {
        InitialiseQueries();
    }
    protected void InitialiseQueries() {
        EntityQueryDesc _cooldownQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<GameplayEffectTargetComponent>() },
            Any = new ComponentType[] { ComponentType.ReadOnly<GlobalCooldownGameplayEffectComponent>() }
        };
        CooldownEffectsQuery = GetEntityQuery(_cooldownQueryDesc);
        GrantedAbilityQuery = GetEntityQuery(ComponentType.ReadOnly<AbilitySystemActor>(), ComponentType.ReadWrite<TestAbilityTag>());
    }

    protected override JobHandle CheckAbilityAvailable(JobHandle inputDeps) {
        // Check the granted ability entity for this ability.  Usually, if cooldown <= 0, ability is not available.

        // Any other logic that determines whether the 
        return inputDeps;
    }

    protected override JobHandle CooldownJobs(JobHandle inputDeps) {
        NativeMultiHashMap<Entity, GameplayEffectDurationComponent> Cooldowns = new NativeMultiHashMap<Entity, GameplayEffectDurationComponent>(CooldownEffectsQuery.CalculateEntityCount() * 2 + GrantedAbilityQuery.CalculateEntityCount(), Allocator.TempJob);

        // Collect all effects which act as cooldowns for this ability
        inputDeps = new GatherCooldownGameplayEffectsJob
        {
            GameplayEffectDurations = Cooldowns.AsParallelWriter()
        }.Schedule(CooldownEffectsQuery, inputDeps);

        // Add a default value of '0' for all entities as well
        inputDeps = new CooldownAbilityIsZeroIfAbsentJob
        {
            GameplayEffectDurations = Cooldowns.AsParallelWriter()
        }.Schedule(GrantedAbilityQuery, inputDeps);

        // Get the effect with the longest cooldown remaining
        inputDeps = new GatherLongestCooldownPerEntity
        {
            GameplayEffectDurationComponent = Cooldowns
        }.Schedule(GrantedAbilityQuery, inputDeps);

        Cooldowns.Dispose(inputDeps);
        return inputDeps;
    }

}