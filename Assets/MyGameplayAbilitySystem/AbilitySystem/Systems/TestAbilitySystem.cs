using GameplayAbilitySystem.Abilities.Systems;
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

    private EntityQuery _cooldownEffectsQuery;

    protected override void InitialiseCooldownQuery() {
        EntityQueryDesc _cooldownQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<GameplayEffectTargetComponent>() },
            Any = new ComponentType[] { ComponentType.ReadOnly<GlobalCooldownGameplayEffectComponent>() }
        };
        _cooldownEffectsQuery = GetEntityQuery(_cooldownQueryDesc);
    }

    protected override JobHandle CheckAbilityAvailable(JobHandle inputDeps) {
        return inputDeps;
    }

    protected override JobHandle CooldownJobs(JobHandle inputDeps) {
        NativeMultiHashMap<Entity, GameplayEffectDurationComponent> Cooldowns = new NativeMultiHashMap<Entity, GameplayEffectDurationComponent>(CooldownEffectsQuery.CalculateEntityCount() * 2 + grantedAbilityQuery.CalculateEntityCount(), Allocator.TempJob);

        // Collect all effects which act as cooldowns for this ability
        inputDeps = new GatherCooldownGameplayEffectsJob
        {
            GameplayEffectDurations = Cooldowns.AsParallelWriter()
        }.Schedule(CooldownEffectsQuery, inputDeps);

        // Add a default value of '0' for all entities as well
        inputDeps = new CooldownAbilityIsZeroIfAbsentJob
        {
            GameplayEffectDurations = Cooldowns.AsParallelWriter()
        }.Schedule(grantedAbilityQuery, inputDeps);

        // Get the effect with the longest cooldown remaining
        inputDeps = new GatherLongestCooldownPerEntity
        {
            GameplayEffectDurationComponent = Cooldowns
        }.Schedule(grantedAbilityQuery, inputDeps);

        Cooldowns.Dispose(inputDeps);
        return inputDeps;
    }

    protected override EntityQuery CooldownEffectsQuery => _cooldownEffectsQuery;
}