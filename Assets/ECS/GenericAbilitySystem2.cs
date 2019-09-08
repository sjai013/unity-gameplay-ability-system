using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class GenericAbilitySystem2 : JobComponentSystem {
    public delegate void ApplyGameplayEffectsDelegate(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
    public delegate void ApplyAbilityCostsDelegate(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
    public delegate bool CheckResourceAvailableDelegate(ref Entity Caster, ref AttributesComponent attributesComponent);
    public delegate void ApplyCooldownEffectDelegate(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);

    // void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent)
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    NativeHashMap<int, FunctionPointer<CheckResourceAvailableDelegate>> checkResourceFunctionArray;
    NativeMultiHashMap<int, EGameplayEffect> abilityCooldownEffectsMap;
    List<NativeArray<EGameplayEffect>> abilityCooldownEffects = new List<NativeArray<EGameplayEffect>>();
    EntityQuery m_CooldownEffects;
    EntityQuery m_ActiveAbilities;
    List<IAbilityBehaviour> abilities = new List<IAbilityBehaviour>();

    NativeArray<JobHandle> abilityJobHandles;
    protected override void OnCreate() {

        base.OnCreate();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        // Collect list of all types that implement IAbility and store reference to methods 
        var abilityTypes = World.Active.EntityManager.GetAssignableComponentTypes(typeof(IAbilityBehaviour)).ToArray();
        checkResourceFunctionArray = new NativeHashMap<int, FunctionPointer<CheckResourceAvailableDelegate>>(abilityTypes.Length, Allocator.Persistent);
        abilityCooldownEffectsMap = new NativeMultiHashMap<int, EGameplayEffect>(abilityTypes.Length, Allocator.Persistent);
        abilityJobHandles = new NativeArray<JobHandle>(abilityTypes.Length, Allocator.Persistent);
        // Iterate over all objects that implement the IAbility interface
        for (var i = 0; i < abilityTypes.Length; i++) {
            IAbilityBehaviour ability = (IAbilityBehaviour)Activator.CreateInstance(abilityTypes[i]);
            abilities.Add(ability);
            // Create the FunctionPointer hashmaps
            checkResourceFunctionArray.TryAdd((int)ability.AbilityType, new FunctionPointer<CheckResourceAvailableDelegate>(Marshal.GetFunctionPointerForDelegate((CheckResourceAvailableDelegate)(ability.CheckResourceAvailable))));
            var cooldownEffects = ability.CooldownEffects;
            var nativeArray = new NativeArray<EGameplayEffect>(cooldownEffects, Allocator.Persistent);
            abilityCooldownEffects.Add(nativeArray);

            for (var j = 0; j < cooldownEffects.Length; j++) {
                abilityCooldownEffectsMap.Add((int)ability.AbilityType, cooldownEffects[j]);
            }
        }

        // Create EntityQueries for jobs
        m_CooldownEffects = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<CooldownEffectComponent>(), ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<AttributeModificationComponent>() },
        });

        m_ActiveAbilities = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<AbilityComponent>(), ComponentType.ReadOnly<AbilityStateComponent>(), ComponentType.ReadOnly<AbilitySourceTarget>(), ComponentType.ReadWrite<AbilityCooldownComponent>() },
        });
    }

    [BurstCompile]
    [RequireComponentTag(typeof(AttributeModificationComponent))]
    public struct GatherCooldownEffectsJob : IJobForEach<CooldownEffectComponent, GameplayEffectDurationComponent> {
        [WriteOnly] public NativeMultiHashMap<int, CooldownForCasterComponent>.ParallelWriter activeCooldownEffects;
        public void Execute([ReadOnly] ref CooldownEffectComponent cdEffect, [ReadOnly] ref GameplayEffectDurationComponent geDuration) {

            var caster = cdEffect.Caster;
            var effect = geDuration.Effect;
            var remaining = new CooldownForCasterComponent
            {
                Duration = geDuration.Duration,
                TimeRemaining = geDuration.TimeRemaining,
                Caster = caster
            };

            activeCooldownEffects.Add((int)effect, remaining);
        }
    }

    public struct FilterCooldownEffectsForAbilityJob : IJob {
        [ReadOnly] public NativeMultiHashMap<int, CooldownForCasterComponent> activeCooldownEffects;
        [ReadOnly] public NativeArray<EGameplayEffect> validCooldownEffects;
        public NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownRemainingForAbility; // int refers to ability enum

        public void Execute() {
            // Get effects matching ability from main NMHP
            for (var i = 0; i < validCooldownEffects.Length; i++) { // for each cooldown effect applicable to this ability
                var effect = validCooldownEffects[i];
                using (var cooldownEffectEnumerator = activeCooldownEffects.GetValuesForKey((int)effect)) {
                    while (cooldownEffectEnumerator.MoveNext()) {
                        var cooldownEffect = cooldownEffectEnumerator.Current;
                        if (!cooldownRemainingForAbility.TryGetValue(cooldownEffect.Caster, out var effectDuration)) {
                            cooldownRemainingForAbility.TryAdd(cooldownEffect.Caster, new GrantedAbilityCooldownComponent
                            {
                                Duration = cooldownEffect.Duration,
                                TimeRemaining = cooldownEffect.TimeRemaining
                            });
                            continue;
                        }

                        effectDuration.Duration = math.select(effectDuration.Duration, cooldownEffect.Duration, effectDuration.TimeRemaining < cooldownEffect.TimeRemaining);
                        effectDuration.TimeRemaining = math.select(effectDuration.TimeRemaining, cooldownEffect.TimeRemaining, effectDuration.TimeRemaining < cooldownEffect.TimeRemaining);
                        cooldownRemainingForAbility[cooldownEffect.Caster] = effectDuration;
                    }

                }
            }
        }
    }

    [BurstCompile]
    [RequireComponentTag(typeof(AbilityComponent), typeof(AbilitySourceTarget))]
    public struct UpdateAbilityAvailableJob : IJobForEach<AbilityStateComponent, AbilityCooldownComponent> {
        public void Execute(ref AbilityStateComponent state, ref AbilityCooldownComponent cooldown) {
            if (state.State != EAbilityState.CheckCooldown) return;
            if (cooldown.TimeRemaining > 0 && cooldown.CooldownActivated) {
                state.State = EAbilityState.Failed;
            } else {
                state.State = EAbilityState.CheckResource;
            }
        }
    }

    [RequireComponentTag(typeof(AbilityComponent), typeof(AbilitySourceTarget))]

    public struct UpdateAbilitiesStatusJob : IJobForEachWithEntity<AbilityStateComponent, AbilityCooldownComponent> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public void Execute(Entity entity, int index, ref AbilityStateComponent state, [ReadOnly] ref AbilityCooldownComponent cooldown) {
            if (state.State == EAbilityState.TryActivate) {
                state.State = EAbilityState.CheckCooldown;
            }

            if (state.State == EAbilityState.Failed) {
                EntityCommandBuffer.DestroyEntity(index, entity);
            }

            if (state.State == EAbilityState.Completed && cooldown.CooldownActivated && cooldown.TimeRemaining <= 0) {
                EntityCommandBuffer.DestroyEntity(index, entity);
            }

        }
    }


    [BurstCompile]
    public struct CooldownGameplayEffects : IJobForEach<CooldownEffectComponent, GameplayEffectDurationComponent> {
        public void Execute(ref CooldownEffectComponent cooldown, ref GameplayEffectDurationComponent duration) {
            throw new NotImplementedException();
        }
    }

    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var m_CooldownEffectsCount = m_CooldownEffects.CalculateEntityCount();
        var m_ActiveAbilitiesCount = m_ActiveAbilities.CalculateEntityCount();
        var activeCooldownEffects = new NativeMultiHashMap<int, CooldownForCasterComponent>(m_CooldownEffectsCount, Allocator.TempJob);

        ComponentDataFromEntity<AttributesComponent> attributeComponents = GetComponentDataFromEntity<AttributesComponent>();

        inputDeps = new GatherCooldownEffectsJob
        {
            activeCooldownEffects = activeCooldownEffects.AsParallelWriter()
        }.Schedule(this, inputDeps);

        inputDeps = new UpdateAbilityAvailableJob
        {

        }.Schedule(m_ActiveAbilities, inputDeps);

        inputDeps = new UpdateAbilitiesStatusJob
        {
            EntityCommandBuffer = commandBuffer
        }.Schedule(m_ActiveAbilities, inputDeps);

        // The rest of the functionality is defined by the individual abilities
        for (var i = 0; i < abilities.Count; i++) {
            // Get all cooldown effects applicable to this ability
            var effectRemainingForAbility = new NativeHashMap<Entity, GrantedAbilityCooldownComponent>(m_CooldownEffectsCount, Allocator.TempJob);
            var abilityAvailable = new NativeHashMap<Entity, bool>(m_CooldownEffectsCount, Allocator.TempJob);
            inputDeps = new FilterCooldownEffectsForAbilityJob
            {
                activeCooldownEffects = activeCooldownEffects,
                validCooldownEffects = abilityCooldownEffects[i],
                cooldownRemainingForAbility = effectRemainingForAbility
            }.Schedule(inputDeps);
            inputDeps = abilities[i].CheckAbilityAvailableJob(this, inputDeps, attributeComponents);
            inputDeps = abilities[i].UpdateCooldownsJob(this, inputDeps, effectRemainingForAbility);
            inputDeps = abilities[i].BeginAbilityCastJob(this, inputDeps, commandBuffer, attributeComponents, Time.time);
            effectRemainingForAbility.Dispose(inputDeps);
        }

        m_EntityCommandBufferSystem.AddJobHandleForProducer(inputDeps);

        // Initialise cooldown data
        activeCooldownEffects.Dispose(inputDeps);
        return inputDeps;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        checkResourceFunctionArray.Dispose();
        abilityCooldownEffectsMap.Dispose();
        // if (effectRemainingForCasterMap.IsCreated) effectRemainingForCasterMap.Dispose();
        // if (casterAbilityDurationMap.IsCreated) casterAbilityDurationMap.Dispose();
    }

}
