using System;
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
    public delegate bool CheckResourceAvailableDelegate(Entity Caster, AttributesComponent attributesComponent);
    public delegate void ApplyCooldownEffectDelegate(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);

    // void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent)
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    NativeHashMap<int, FunctionPointer<ApplyGameplayEffectsDelegate>> gameplayEffectHashMap;
    NativeHashMap<int, FunctionPointer<CheckResourceAvailableDelegate>> checkResourceAvailableHashMap;
    NativeHashMap<int, FunctionPointer<ApplyAbilityCostsDelegate>> applyAbilityCostsHashMap;
    NativeHashMap<int, FunctionPointer<ApplyCooldownEffectDelegate>> applyCooldownEffectHashMap;
    NativeMultiHashMap<int, EGameplayEffect> abilityCooldownEffectsMap;
    NativeHashMap<EntityGameplayEffect, GrantedAbilityCooldownComponent> effectRemainingForCasterMap;
    NativeHashMap<CasterAbilityTuple, GrantedAbilityCooldownComponent> casterAbilityDurationMap;

    NativeList<int> allAbilitiesList;

    private EntityQuery m_Cooldowns;
    private EntityQuery m_AbilityCooldown;
    private EntityQuery m_ActiveAbilities;

    protected override void OnCreate() {

        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        // cooldownQuery = GetEntityQuery(cooldownQueryDesc);
        base.OnCreate();

        // Collect list of all types that implement IAbility and store reference to methods 
        var abilityTypes = World.Active.EntityManager.GetAssignableComponentTypes(typeof(IAbility)).ToArray();
        gameplayEffectHashMap = new NativeHashMap<int, FunctionPointer<ApplyGameplayEffectsDelegate>>(abilityTypes.Length, Allocator.Persistent);
        checkResourceAvailableHashMap = new NativeHashMap<int, FunctionPointer<CheckResourceAvailableDelegate>>(abilityTypes.Length, Allocator.Persistent);
        applyAbilityCostsHashMap = new NativeHashMap<int, FunctionPointer<ApplyAbilityCostsDelegate>>(abilityTypes.Length, Allocator.Persistent);
        applyCooldownEffectHashMap = new NativeHashMap<int, FunctionPointer<ApplyCooldownEffectDelegate>>(abilityTypes.Length, Allocator.Persistent);
        allAbilitiesList = new NativeList<int>(abilityTypes.Length, Allocator.Persistent);
        abilityCooldownEffectsMap = new NativeMultiHashMap<int, EGameplayEffect>(abilityTypes.Length * 2, Allocator.Persistent);


        for (var i = 0; i < abilityTypes.Length; i++) {
            IAbility ability = (IAbility)Activator.CreateInstance(abilityTypes[i]);
            gameplayEffectHashMap.TryAdd((int)ability.AbilityType, new FunctionPointer<ApplyGameplayEffectsDelegate>(Marshal.GetFunctionPointerForDelegate((ApplyGameplayEffectsDelegate)(ability.ApplyGameplayEffects))));
            checkResourceAvailableHashMap.TryAdd((int)ability.AbilityType, new FunctionPointer<CheckResourceAvailableDelegate>(Marshal.GetFunctionPointerForDelegate((CheckResourceAvailableDelegate)(ability.CheckResourceAvailable))));
            applyAbilityCostsHashMap.TryAdd((int)ability.AbilityType, new FunctionPointer<ApplyAbilityCostsDelegate>(Marshal.GetFunctionPointerForDelegate((ApplyAbilityCostsDelegate)(ability.ApplyAbilityCosts))));
            applyCooldownEffectHashMap.TryAdd((int)ability.AbilityType, new FunctionPointer<ApplyCooldownEffectDelegate>(Marshal.GetFunctionPointerForDelegate((ApplyCooldownEffectDelegate)(ability.ApplyCooldownEffect))));
            var cooldownEffects = ability.CooldownEffects;
            for (var j = 0; j < cooldownEffects.Length; j++) {
                abilityCooldownEffectsMap.Add((int)ability.AbilityType, cooldownEffects[j]);
            }
            allAbilitiesList.Add((int)ability.AbilityType);
        }

        m_Cooldowns = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<CooldownEffectComponent>(), ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<AttributeModificationComponent>() },
        });

        m_ActiveAbilities = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<AbilityComponent>(), ComponentType.ReadOnly<AbilityStateComponent>(), ComponentType.ReadOnly<AbilitySourceTarget>() },
        });

        m_AbilityCooldown = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<AbilityComponent>(), ComponentType.ReadOnly<AbilityStateComponent>(), ComponentType.ReadOnly<AbilitySourceTarget>(), ComponentType.ReadWrite<AbilityCooldownComponent>() },
        });
        

    }


    public struct AbilityJob : IJobForEachWithEntity<AbilityComponent, AbilityStateComponent, AbilitySourceTarget> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
        [ReadOnly] public NativeHashMap<int, FunctionPointer<ApplyGameplayEffectsDelegate>> gameplayEffectHashMap;
        [ReadOnly] public NativeHashMap<int, FunctionPointer<CheckResourceAvailableDelegate>> checkResourceAvailableHashMap;
        [ReadOnly] public NativeHashMap<int, FunctionPointer<ApplyAbilityCostsDelegate>> applyAbilityCostsHashMap;
        [ReadOnly] public NativeHashMap<int, FunctionPointer<ApplyCooldownEffectDelegate>> applyCooldownEffectHashMap;
        [ReadOnly] public NativeHashMap<CasterAbilityTuple, GrantedAbilityCooldownComponent> casterAbilityDurationMap;

        // [ReadOnly] public NativeArray<CooldownTimeCaster> CooldownArray;

        public float WorldTime;
        public void Execute(Entity entity,
                            int index,
                            ref AbilityComponent abilityComponent,
                            ref AbilityStateComponent abilityState,
                            ref AbilitySourceTarget abilitySourceTarget
                            ) {

            if (abilityState.State != EAbilityState.TryActivate) return;

            // // Get ability cooldown
            // var casterAbilityCooldownKey = new CasterAbilityTuple
            // {
            //     Ability = abilityComponent.Ability,
            //     Host = abilitySourceTarget.Source
            // };

            // casterAbilityDurationMap.TryGetValue(casterAbilityCooldownKey, out var duration);
            // cooldown.Duration = duration;
            var sourceAttrs = attributesComponent[abilitySourceTarget.Source];
            if (attributesComponent.Exists(abilitySourceTarget.Source)) {
                EntityCommandBuffer.AddComponent<AbilityCooldownComponent>(index, entity, new AbilityCooldownComponent
                {
                    Duration = 0,
                    TimeRemaining = 0
                });

                var resourceAvailableFP = checkResourceAvailableHashMap[(int)abilityComponent.Ability];
                var resourcesAvailable = resourceAvailableFP.Invoke(abilitySourceTarget.Source, sourceAttrs);

                if (!resourcesAvailable) {
                    AbilityUnsuccessful(index, ref abilityState);
                    return;
                }

                if (!AbilityNotOnCooldown(index, entity, abilitySourceTarget.Source, abilityComponent.Ability)) {
                    AbilityUnsuccessful(index, ref abilityState);
                    return;
                }

                var applyAbilityCostFP = applyAbilityCostsHashMap[(int)abilityComponent.Ability];
                applyAbilityCostFP.Invoke(index, EntityCommandBuffer, abilitySourceTarget.Source, abilitySourceTarget.Target, sourceAttrs);

                var applyCooldownEffectFP = applyCooldownEffectHashMap[(int)abilityComponent.Ability];
                applyCooldownEffectFP.Invoke(index, EntityCommandBuffer, abilitySourceTarget.Source, WorldTime);

            }

            AbilitySuccessful(index, ref abilityState);
        }


        //     private bool AbilityNotOnCooldown(int index, Entity entity, Entity source) {
        //         // Find entity in CooldownArray
        //         for (var i = 0; i < CooldownArray.Length; i++) {
        //             if (CooldownArray[i].Caster.Equals(source)
        //                 && CooldownArray[i].TimeRemaining > 0f) {
        //                 return false;
        //             }
        //         }
        //         return true;
        //     }

        // if (attributesComponent.Exists(abilitySourceTarget.Source)) {
        //     // Check if ability is on cooldown for player
        //     if (!AbilityNotOnCooldown(index, entity, abilitySourceTarget.Source)) {
        //         AbilityUnsuccessful(index, ability, ref abilityState);
        //         return;
        //     }
        // }
        // AbilitySuccessful(index, ability, ref abilityState);


        private bool ResourceAvailable(int index, Entity entity, Entity source, AttributesComponent sourceAttrs, IAbility ability) {
            var resourcesAvailable = ability.CheckResourceAvailable(source, sourceAttrs);
            if (!resourcesAvailable) {
                return false;
            }
            return true;
        }

        private bool AbilityNotOnCooldown(int index, Entity entity, Entity source, EAbility Ability) {
            var casterAbilityKey = new CasterAbilityTuple
            {
                Ability = Ability,
                Host = source
            };

            if (casterAbilityDurationMap.TryGetValue(casterAbilityKey, out var cooldown)) {
                return !(cooldown.Duration > 0 && cooldown.TimeRemaining > 0);
            }

            return false;
        }

        private void AbilitySuccessful(int index, ref AbilityStateComponent abilityState) {
            abilityState.State = EAbilityState.Activate;
        }

        private void AbilityUnsuccessful(int index, ref AbilityStateComponent abilityState) {
            abilityState.State = EAbilityState.Failed;
        }


    }


    public struct EffectAggregatorJob : IJobForEach<CooldownEffectComponent, GameplayEffectDurationComponent, AttributeModificationComponent> {
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

    [BurstCompile]
    public struct AbilityCooldownAggregatorJob : IJobForEach<AbilitySourceTarget, AbilityComponent, AbilityStateComponent> {
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


    public struct AbilityCooldownUpdateJob : IJobForEach<AbilityComponent, AbilityStateComponent, AbilitySourceTarget, AbilityCooldownComponent> {

        [ReadOnly] public NativeHashMap<CasterAbilityTuple, GrantedAbilityCooldownComponent> casterAbilityDurationMap;

        public void Execute([ReadOnly] ref AbilityComponent abilityComponent,
                            [ReadOnly] ref AbilityStateComponent abilityState,
                            [ReadOnly] ref AbilitySourceTarget abilitySourceTarget,
                            [WriteOnly] ref AbilityCooldownComponent cooldown
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
        }
    }

    public struct AbilityCleanup : IJobForEachWithEntity<AbilityCooldownComponent, AbilityStateComponent> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref AbilityCooldownComponent abilityCooldown, [ReadOnly] ref AbilityStateComponent state) {
            if ((abilityCooldown.TimeRemaining <= 0 && state.State == EAbilityState.Completed) || state.State == EAbilityState.Failed) {
                EntityCommandBuffer.DestroyEntity(index, entity);
            }
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        gameplayEffectHashMap.Dispose();
        checkResourceAvailableHashMap.Dispose();
        applyAbilityCostsHashMap.Dispose();
        applyCooldownEffectHashMap.Dispose();
        allAbilitiesList.Dispose();
        abilityCooldownEffectsMap.Dispose();
        if (effectRemainingForCasterMap.IsCreated) effectRemainingForCasterMap.Dispose();
        if (casterAbilityDurationMap.IsCreated) casterAbilityDurationMap.Dispose();

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var m_CooldownCount = m_Cooldowns.CalculateEntityCount();
        var m_ActiveAbilitiesCount = m_ActiveAbilities.CalculateEntityCount();
        effectRemainingForCasterMap = new NativeHashMap<EntityGameplayEffect, GrantedAbilityCooldownComponent>(m_CooldownCount, Allocator.TempJob);
        casterAbilityDurationMap = new NativeHashMap<CasterAbilityTuple, GrantedAbilityCooldownComponent>(m_ActiveAbilitiesCount, Allocator.TempJob);

        // NativeMultiHashMap<Entity, NativeHashMap<int, Entity>> grantedAbilities = new NativeMultiHashMap<Entity, NativeHashMap<int, Entity>>();

        // inputDeps = new ActiveAbilityAggregatorJob
        // {

        // }.Schedule(m_Abilities, inputDeps);

        inputDeps = new EffectAggregatorJob
        {
            effectRemainingForCasterMap = effectRemainingForCasterMap
        }.Schedule(m_Cooldowns, inputDeps);

        inputDeps = new AbilityCooldownAggregatorJob
        {
            effectRemainingForCasterMap = effectRemainingForCasterMap,
            casterAbilityDurationMap = casterAbilityDurationMap,
            abilityCooldownEffectsMap = abilityCooldownEffectsMap
        }.Schedule(m_ActiveAbilities, inputDeps);

        inputDeps = new AbilityCooldownUpdateJob
        {
            casterAbilityDurationMap = casterAbilityDurationMap
        }.Schedule(m_AbilityCooldown, inputDeps);

        inputDeps = new AbilityJob
        {
            gameplayEffectHashMap = gameplayEffectHashMap,
            checkResourceAvailableHashMap = checkResourceAvailableHashMap,
            applyAbilityCostsHashMap = applyAbilityCostsHashMap,
            applyCooldownEffectHashMap = applyCooldownEffectHashMap,
            attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true),
            EntityCommandBuffer = commandBuffer,
            WorldTime = Time.time,
            casterAbilityDurationMap = casterAbilityDurationMap
        }.Schedule(m_ActiveAbilities, inputDeps);

        inputDeps = new AbilityCleanup
        {
            EntityCommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(m_AbilityCooldown, inputDeps);
        
        // Initialise cooldown data
        effectRemainingForCasterMap.Dispose(inputDeps);
        casterAbilityDurationMap.Dispose(inputDeps);
        return inputDeps;
    }

    protected override void OnStopRunning() {
        if (effectRemainingForCasterMap.IsCreated) effectRemainingForCasterMap.Dispose();
        if (casterAbilityDurationMap.IsCreated) casterAbilityDurationMap.Dispose();
    }

}

public struct CasterAbilityTuple : IComponentData, IEquatable<CasterAbilityTuple> {
    public Entity Host;
    public EAbility Ability;

    public bool Equals(CasterAbilityTuple other) {
        return other.Host == Host && other.Ability == Ability;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + Host.GetHashCode();
            hash = hash * 31 + (int)Ability.GetHashCode();
            return hash;
        }
    }
}

public struct GrantedAbilityCooldownComponent : IComponentData {
    public float TimeRemaining;
    public float Duration;
}



public struct EffectTimeRemaining : IEquatable<EffectTimeRemaining> {
    public EGameplayEffect Effect;
    public float Remaining;

    public override int GetHashCode() {
        return Effect.GetHashCode();
    }

    public override bool Equals(object obj) {
        return Equals((EffectTimeRemaining)obj);

    }

    public bool Equals(EffectTimeRemaining other) {
        return other.Effect == Effect;
    }

}

public struct EntityGameplayEffect : IComponentData, IEquatable<EntityGameplayEffect> {
    public Entity Caster;
    public EGameplayEffect Effect;

    public bool Equals(EntityGameplayEffect other) {
        return other.Caster == Caster && other.Effect == Effect;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + Caster.GetHashCode();
            hash = hash * 31 + (int)Effect.GetHashCode();
            return hash;
        }
    }
}