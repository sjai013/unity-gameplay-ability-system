using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GameplayAbilitySystem.Abilities.Fire;
using GameplayAbilitySystem.Abilities.Heal;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class GenericAbilitySystem : JobComponentSystem {
    public delegate void ApplyGameplayEffectsDelegate(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
    public delegate void ApplyAbilityCostsDelegate(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
    public delegate bool CheckResourceAvailableDelegate(ref Entity Caster, ref AttributesComponent attributesComponent);
    public delegate void ApplyCooldownEffectDelegate(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);

    // void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent)
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    NativeHashMap<int, FunctionPointer<CheckResourceAvailableDelegate>> checkResourceFunctionArray;
    NativeHashMap<int, FunctionPointer<ApplyAbilityCostsDelegate>> applyAbilityCostsHashMap;
    NativeHashMap<int, FunctionPointer<ApplyCooldownEffectDelegate>> applyCooldownEffectHashMap;
    NativeMultiHashMap<int, EGameplayEffect> abilityCooldownEffectsMap;
    NativeHashMap<EntityGameplayEffect, GrantedAbilityCooldownComponent> effectRemainingForCasterMap;
    NativeHashMap<CasterAbilityTuple, GrantedAbilityCooldownComponent> casterAbilityDurationMap;
    EntityQuery m_Cooldowns;
    EntityQuery m_ActiveAbilities;
    List<IAbilityBehaviour> abilities = new List<IAbilityBehaviour>();
    protected override void OnCreate() {

        base.OnCreate();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        // Collect list of all types that implement IAbility and store reference to methods 
        var abilityTypes = World.Active.EntityManager.GetAssignableComponentTypes(typeof(IAbilityBehaviour)).ToArray();
        checkResourceFunctionArray = new NativeHashMap<int, FunctionPointer<CheckResourceAvailableDelegate>>(abilityTypes.Length, Allocator.Persistent);
        applyAbilityCostsHashMap = new NativeHashMap<int, FunctionPointer<ApplyAbilityCostsDelegate>>(abilityTypes.Length, Allocator.Persistent);
        applyCooldownEffectHashMap = new NativeHashMap<int, FunctionPointer<ApplyCooldownEffectDelegate>>(abilityTypes.Length, Allocator.Persistent);
        abilityCooldownEffectsMap = new NativeMultiHashMap<int, EGameplayEffect>(abilityTypes.Length, Allocator.Persistent);
        // Iterate over all objects that implement the IAbility interface
        for (var i = 0; i < abilityTypes.Length; i++) {
            IAbilityBehaviour ability = (IAbilityBehaviour)Activator.CreateInstance(abilityTypes[i]);
            abilities.Add(ability);
            // Create the FunctionPointer hashmaps
            checkResourceFunctionArray.TryAdd((int)ability.AbilityType, new FunctionPointer<CheckResourceAvailableDelegate>(Marshal.GetFunctionPointerForDelegate((CheckResourceAvailableDelegate)(ability.CheckResourceAvailable))));
            applyAbilityCostsHashMap.TryAdd((int)ability.AbilityType, new FunctionPointer<ApplyAbilityCostsDelegate>(Marshal.GetFunctionPointerForDelegate((ApplyAbilityCostsDelegate)(ability.ApplyAbilityCosts))));
            applyCooldownEffectHashMap.TryAdd((int)ability.AbilityType, new FunctionPointer<ApplyCooldownEffectDelegate>(Marshal.GetFunctionPointerForDelegate((ApplyCooldownEffectDelegate)(ability.ApplyCooldownEffect))));
            var cooldownEffects = ability.CooldownEffects;
            for (var j = 0; j < cooldownEffects.Length; j++) {
                abilityCooldownEffectsMap.Add((int)ability.AbilityType, cooldownEffects[j]);
            }
        }

        // Create EntityQueries for jobs
        m_Cooldowns = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<CooldownEffectComponent>(), ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<AttributeModificationComponent>() },
        });

        m_ActiveAbilities = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<AbilityComponent>(), ComponentType.ReadOnly<AbilityStateComponent>(), ComponentType.ReadOnly<AbilitySourceTarget>(), ComponentType.ReadWrite<AbilityCooldownComponent>() },
        });
    }


    [BurstCompile]
    public struct GatherAttributesJob : IJobForEachWithEntity<AbilitySourceTarget> {
        [WriteOnly] public NativeArray<AttributesComponent> attributesComponentArray;
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
        public void Execute(Entity entity, int index, ref AbilitySourceTarget abilitySourceTarget) {
            if (attributesComponent.Exists(abilitySourceTarget.Source)) {
                var sourceAttrs = attributesComponent[abilitySourceTarget.Source];
                attributesComponentArray[index] = sourceAttrs;
            }
        }
    }

    [BurstCompile]
    public struct GatherAbilityAvailabilityJob : IJobForEachWithEntity<AbilityComponent, AbilitySourceTarget, AbilityCooldownComponent> {
        [ReadOnly] public NativeArray<AttributesComponent> attributesComponentArray;
        [ReadOnly] public NativeHashMap<int, FunctionPointer<CheckResourceAvailableDelegate>> checkResourceAvailableArray;
        [WriteOnly] public NativeArray<bool> abilityAvailableArray;
        public void Execute(Entity entity, int index, [ReadOnly] ref AbilityComponent abilityComponent, [ReadOnly] ref AbilitySourceTarget abilitySourceTarget, [ReadOnly] ref AbilityCooldownComponent abilityCooldown) {
            var resourceAvailable = false;
            var resourceAvailableFP = checkResourceAvailableArray[(int)abilityComponent.Ability];
            var sourceAttrs = attributesComponentArray[index];
            resourceAvailable = resourceAvailableFP.Invoke(ref abilitySourceTarget.Source, ref sourceAttrs);
            abilityAvailableArray[index] = resourceAvailable && !abilityCooldown.CooldownActivated;
        }
    }

    public struct GatherGameplayEffectsJob : IJobForEach<CooldownEffectComponent, GameplayEffectDurationComponent, AttributeModificationComponent> {
        // /// <summary>
        // /// <Entity, NativeHashMap<int,float>> -> <Caster, <EGameplayAbility, CooldownRemaining>>
        // /// </summary>
        [NativeDisableParallelForRestriction] public NativeHashMap<EntityGameplayEffect, GrantedAbilityCooldownComponent> effectRemainingForCasterMap;

        public void Execute([ReadOnly] ref CooldownEffectComponent cdEffect, [ReadOnly] ref GameplayEffectDurationComponent geDuration, [ReadOnly] ref AttributeModificationComponent attrMod) {

            var caster = cdEffect.Caster;
            var effect = geDuration.Effect;
            var remaining = new GrantedAbilityCooldownComponent
            {
                Duration = geDuration.Duration,
                TimeRemaining = geDuration.TimeRemaining
            };

            var key = new EntityGameplayEffect
            {
                Caster = caster,
                Effect = effect
            };
            // Try to get value of existing effect for this caster
            if (effectRemainingForCasterMap.TryGetValue(key, out var existingRemaining)) {
                // Effect exists, does this effect have larger cd than existing?
                if (remaining.TimeRemaining > existingRemaining.TimeRemaining) {
                    // Replace duration remaining with the new value
                    effectRemainingForCasterMap[key] = remaining;
                }
            } else {
                // We don't have this effect for this caster, so add it to map
                effectRemainingForCasterMap.TryAdd(key, remaining);
            }

        }
    }


    public struct GatherCooldownsJob : IJobForEach<AbilitySourceTarget, AbilityComponent, AbilityStateComponent> {
        [ReadOnly] public NativeHashMap<EntityGameplayEffect, GrantedAbilityCooldownComponent> effectRemainingForCasterMap;
        [ReadOnly] public NativeMultiHashMap<int, EGameplayEffect> abilityCooldownEffectsMap;

        [NativeDisableParallelForRestriction] public NativeHashMap<CasterAbilityTuple, GrantedAbilityCooldownComponent> casterAbilityDurationMap;


        public void Execute(ref AbilitySourceTarget sourceTarget, ref AbilityComponent ability, ref AbilityStateComponent state) {
            var longestDuration = new GrantedAbilityCooldownComponent
            {
                Duration = 0,
                TimeRemaining = 0
            };

            var duration = longestDuration;
            foreach (var effect in abilityCooldownEffectsMap.GetValuesForKey((int)ability.Ability)) {
                var key = new EntityGameplayEffect
                {
                    Caster = sourceTarget.Source,
                    Effect = effect
                };
                effectRemainingForCasterMap.TryGetValue(key, out duration);
                if (duration.TimeRemaining > longestDuration.TimeRemaining) {
                    longestDuration = duration;
                }
            }

            var casterAbilityKey = new CasterAbilityTuple { Ability = ability.Ability, Host = sourceTarget.Source };
            // Unable to add caster/ability (because it already exists)
            // Check duration, and use the longest duration
            if (!(casterAbilityDurationMap.TryAdd(casterAbilityKey, longestDuration))) {
                var existingDuration = casterAbilityDurationMap[casterAbilityKey];
                if (longestDuration.TimeRemaining > existingDuration.TimeRemaining) {
                    casterAbilityDurationMap[casterAbilityKey] = longestDuration;
                }
            }

        }
    }



    public struct ApplyAbilityEffectJob : IJobForEachWithEntity<AbilityComponent, AbilityStateComponent, AbilitySourceTarget> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public NativeHashMap<int, FunctionPointer<ApplyAbilityCostsDelegate>> applyAbilityCostsHashMap;
        [ReadOnly] public NativeArray<AttributesComponent> attributesComponentMap;
        public void Execute(Entity entity, int index, ref AbilityComponent abilityComponent, ref AbilityStateComponent abilityStateComponent, ref AbilitySourceTarget abilitySourceTarget) {
            if (abilityStateComponent.State != EAbilityState.TryActivate) return;
            if (applyAbilityCostsHashMap.TryGetValue((int)abilityComponent.Ability, out var applyAbilityCostFP)) {
                applyAbilityCostFP.Invoke(index, EntityCommandBuffer, abilitySourceTarget.Source, abilitySourceTarget.Target, attributesComponentMap[index]);
            }
        }
    }

    public struct ApplyCooldownEffectJob : IJobForEachWithEntity<AbilityComponent, AbilityStateComponent, AbilitySourceTarget> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public NativeHashMap<int, FunctionPointer<ApplyCooldownEffectDelegate>> applyCooldownEffectHashMap;
        public float WorldTime;
        public void Execute(Entity entity, int index, [ReadOnly] ref AbilityComponent abilityComponent, ref AbilityStateComponent abilityStateComponent, [ReadOnly] ref AbilitySourceTarget abilitySourceTarget) {
            if (abilityStateComponent.State != EAbilityState.TryActivate) return;
            if (applyCooldownEffectHashMap.TryGetValue((int)abilityComponent.Ability, out var applyCooldownEffectFP)) {
                applyCooldownEffectFP.Invoke(index, EntityCommandBuffer, abilitySourceTarget.Source, WorldTime);
            }
        }
    }

    public struct UpdateAbilityActivatedJob : IJobForEachWithEntity<AbilityStateComponent> {
        public void Execute(Entity entity, int index, ref AbilityStateComponent abilityStateComponent) {
            if (abilityStateComponent.State == EAbilityState.TryActivate) {
                abilityStateComponent.State = EAbilityState.Activate;
            }
        }
    }


    [BurstCompile]
    public struct UpdateAbilityAvailabilityJob : IJobForEachWithEntity<AbilityStateComponent> {
        public NativeArray<bool> abilityAvailableArray;

        public void Execute(Entity entity, int index, [WriteOnly] ref AbilityStateComponent abilityStateComponent) {
            if (!abilityAvailableArray[index]) abilityStateComponent.State = EAbilityState.Failed;
        }
    }

    public struct UpdateCooldownsJob : IJobForEach<AbilityComponent, AbilityStateComponent, AbilitySourceTarget, AbilityCooldownComponent> {

        [ReadOnly] public NativeHashMap<CasterAbilityTuple, GrantedAbilityCooldownComponent> casterAbilityDurationMap;

        public void Execute([ReadOnly] ref AbilityComponent abilityComponent,
                            [ReadOnly] ref AbilityStateComponent abilityState,
                            [ReadOnly] ref AbilitySourceTarget abilitySourceTarget,
                            ref AbilityCooldownComponent cooldown
                            ) {

            // Get ability cooldown
            var casterAbilityCooldownKey = new CasterAbilityTuple
            {
                Ability = abilityComponent.Ability,
                Host = abilitySourceTarget.Source
            };

            casterAbilityDurationMap.TryGetValue(casterAbilityCooldownKey, out var duration);
            cooldown.Duration = duration.Duration;
            cooldown.TimeRemaining = duration.TimeRemaining;
            cooldown.CooldownActivated = cooldown.Duration > 0;
        }
    }

    public struct AbilityCleanupJob : IJobForEachWithEntity<AbilityCooldownComponent, AbilityStateComponent> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref AbilityCooldownComponent abilityCooldown, [ReadOnly] ref AbilityStateComponent state) {
            if ((abilityCooldown.TimeRemaining <= 0 && state.State == EAbilityState.Completed) || state.State == EAbilityState.Failed) {
                EntityCommandBuffer.DestroyEntity(index, entity);
            }
        }
    }



    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var m_CooldownCount = m_Cooldowns.CalculateEntityCount();
        var m_ActiveAbilitiesCount = m_ActiveAbilities.CalculateEntityCount();
        effectRemainingForCasterMap = new NativeHashMap<EntityGameplayEffect, GrantedAbilityCooldownComponent>(m_CooldownCount, Allocator.TempJob);
        casterAbilityDurationMap = new NativeHashMap<CasterAbilityTuple, GrantedAbilityCooldownComponent>(m_ActiveAbilitiesCount, Allocator.TempJob);

        // NativeMultiHashMap<Entity, NativeHashMap<int, Entity>> grantedAbilities = new NativeMultiHashMap<Entity, NativeHashMap<int, Entity>>();
        var attributesComponentArray = new NativeArray<AttributesComponent>(m_ActiveAbilitiesCount, Allocator.TempJob);
        var abilityAvailableArray = new NativeArray<bool>(m_ActiveAbilitiesCount, Allocator.TempJob);

        // inputDeps = new GatherGameplayEffectsJob
        // {
        //     effectRemainingForCasterMap = effectRemainingForCasterMap
        // }.Schedule(m_Cooldowns, inputDeps);

        // inputDeps = new GatherCooldownsJob
        // {
        //     effectRemainingForCasterMap = effectRemainingForCasterMap,
        //     casterAbilityDurationMap = casterAbilityDurationMap,
        //     abilityCooldownEffectsMap = abilityCooldownEffectsMap
        // }.Schedule(m_ActiveAbilities, inputDeps);

        // inputDeps = new UpdateCooldownJob
        // {
        //     casterAbilityDurationMap = casterAbilityDurationMap
        // }.Schedule(m_ActiveAbilities, inputDeps);

        // inputDeps = new GatherAttributesJob
        // {
        //     attributesComponentArray = attributesComponentArray,
        //     attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true)
        // }.Schedule(m_ActiveAbilities, inputDeps);

        // inputDeps = new GatherAbilityAvailabilityJob
        // {
        //     attributesComponentArray = attributesComponentArray,
        //     checkResourceAvailableArray = checkResourceFunctionArray,
        //     abilityAvailableArray = abilityAvailableArray
        // }.Schedule(m_ActiveAbilities, inputDeps);


        // inputDeps = new UpdateAbilityAvailabilityJob
        // {
        //     abilityAvailableArray = abilityAvailableArray
        // }.Schedule(m_ActiveAbilities, inputDeps);

        // inputDeps = new ApplyAbilityEffectJob
        // {
        //     EntityCommandBuffer = commandBuffer,
        //     applyAbilityCostsHashMap = applyAbilityCostsHashMap,
        //     attributesComponentMap = attributesComponentArray
        // }.Schedule(m_ActiveAbilities, inputDeps);

        // inputDeps = new ApplyCooldownEffectJob
        // {
        //     EntityCommandBuffer = commandBuffer,
        //     WorldTime = Time.time,
        //     applyCooldownEffectHashMap = applyCooldownEffectHashMap
        // }.Schedule(m_ActiveAbilities, inputDeps);

        // inputDeps = new UpdateAbilityActivatedJob
        // {
        // }.Schedule(m_ActiveAbilities, inputDeps);


        // inputDeps = new AbilityCleanupJob
        // {
        //     EntityCommandBuffer = commandBuffer
        // }.Schedule(m_ActiveAbilities, inputDeps);


        // Initialise cooldown data
        effectRemainingForCasterMap.Dispose(inputDeps);
        casterAbilityDurationMap.Dispose(inputDeps);
        inputDeps.Complete();
        attributesComponentArray.Dispose();
        abilityAvailableArray.Dispose();
        return inputDeps;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        checkResourceFunctionArray.Dispose();
        applyAbilityCostsHashMap.Dispose();
        applyCooldownEffectHashMap.Dispose();
        abilityCooldownEffectsMap.Dispose();
        // if (effectRemainingForCasterMap.IsCreated) effectRemainingForCasterMap.Dispose();
        // if (casterAbilityDurationMap.IsCreated) casterAbilityDurationMap.Dispose();
    }

}
