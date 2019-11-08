/*
 * Created on Mon Nov 04 2019
 *
 * The MIT License (MIT)
 * Copyright (c) 2019 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Systems {
    public enum AbilityStates {
        DISABLED, READY
    };

    public struct NullComponent : IComponentData { }

    public abstract class GenericAbilitySystem<T> : AbilitySystem<T>
    where T : struct, IAbilityTagComponent, IComponentData {

        protected EntityQuery CooldownEffectsQuery;
        protected EntityQuery GrantedAbilityQuery;

        /// <summary>
        /// The ability owns these effects.
        /// </summary>
        /// <value></value>
        protected virtual ComponentType[] AbilityOwningEffects { get; }

        /// <summary>
        /// The ability has these cooldowns.
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] CooldownEffects { get; }

        /// <summary>
        /// This ability cancels currently *executing* abilities which own these effects.
        /// E.g. cancelling a chanelling spell
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] CancelAbilitiesWithOwningEffects { get; }

        /// <summary>
        /// Prevents execution of abilities that have these effects
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] BlockAbilitiesWithEffects { get; }

        /// <summary>
        /// Provides the actor executing this ability with these effects. 
        /// These effects are automatically removed once the ability has
        /// finished executing.
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] SourceActivationOwnedEffects { get; }

        /// <summary>
        /// The actor needs to have all these tags applied to begin executing
        /// the ability.  
        /// 
        /// E.g. Allowing casting of Fire 2 only if we have already cast Fire 1
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] SourceActivationRequiredTags { get; }

        /// <summary>
        /// The actor must not have any of these tags to begin executing the ability.
        /// 
        /// E.g. Can't cast magic if the actor is silenced.
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] SourceActivationBlockedTags { get; }

        /// <summary>
        /// The target needs to have all these tags applied to begin executing
        /// the ability.  
        /// 
        /// E.g. Allowing casting Remedy on a silenced target, only if the target is poisoned.
        /// 
        /// For AOE effects, this means that the spell will still cast, but will have no effect on
        /// targets that do not have all these tags.
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] TargetRequiredTags { get; }

        /// <summary>
        /// The target must not have any of these tags to begin executing the ability.
        /// 
        /// E.g. Can't cast magic if the actor is silenced.
        /// 
        /// For AOE effects, this means that the spell will still cast, but will have no effect
        /// on targets that have any of these tags.
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] TargetBlockedTags { get; }

        protected override void OnCreate() {
            InitialiseQueries();
        }

        protected void InitialiseQueries() {

            EntityQueryDesc _cooldownQueryDesc = new EntityQueryDesc
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<GameplayEffectTargetComponent>() },
                Any = CooldownEffects.Length == 0 ? new ComponentType[] { typeof(NullComponent) } : CooldownEffects,
            };


            CooldownEffectsQuery = GetEntityQuery(_cooldownQueryDesc);
            GrantedAbilityQuery = GetEntityQuery(ComponentType.ReadOnly<AbilitySystemActorTransformComponent>(), ComponentType.ReadOnly<AbilityOwnerComponent>(), ComponentType.ReadWrite<T>());
        }

        protected override JobHandle UpdateAbilityState(JobHandle inputDeps) {
            // Check the granted ability entity for this ability.  Usually, if cooldown <= 0, ability is not available.
            inputDeps = new CheckAbilityAvailableJob().Schedule(GrantedAbilityQuery, inputDeps);
            // Any other logic that determines whether the ability can be activated
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

        /// <summary>
        /// Gather all cooldown effects for this ability
        /// </summary>
        [BurstCompile]
        protected struct GatherCooldownGameplayEffectsJob : IJobForEach<GameplayEffectTargetComponent, GameplayEffectDurationComponent> {
            public NativeMultiHashMap<Entity, GameplayEffectDurationComponent>.ParallelWriter GameplayEffectDurations;
            public void Execute([ReadOnly] ref GameplayEffectTargetComponent targetComponent, [ReadOnly] ref GameplayEffectDurationComponent durationComponent) {
                GameplayEffectDurations.Add(targetComponent, durationComponent);
            }
        }

        /// <summary>
        /// Get the longest cooldown for the ability for each entity
        /// </summary>
        [BurstCompile]
        [RequireComponentTag(typeof(AbilitySystemActorTransformComponent))]
        protected struct GatherLongestCooldownPerEntity : IJobForEach<T, AbilityOwnerComponent> {
            [ReadOnly] public NativeMultiHashMap<Entity, GameplayEffectDurationComponent> GameplayEffectDurationComponent;
            public void Execute([ReadOnly]ref T abilityTagComponent, [ReadOnly] ref AbilityOwnerComponent ownerComponent) {
                abilityTagComponent.DurationComponent = GetMaxFromNMHP(ownerComponent, GameplayEffectDurationComponent);
            }

            private GameplayEffectDurationComponent GetMaxFromNMHP(Entity entity, NativeMultiHashMap<Entity, GameplayEffectDurationComponent> values) {
                values.TryGetFirstValue(entity, out var longestCooldownComponent, out var multiplierIt);
                while (values.TryGetNextValue(out var tempLongestCooldownComponent, ref multiplierIt)) {
                    var tDiff = tempLongestCooldownComponent.RemainingTime - longestCooldownComponent.RemainingTime;
                    var newPercentRemaining = tempLongestCooldownComponent.RemainingTime / tempLongestCooldownComponent.NominalDuration;
                    var oldPercentRemaining = longestCooldownComponent.RemainingTime / longestCooldownComponent.NominalDuration;

                    // If the duration currently being evaluated has more time remaining than the previous one,
                    // use this as the cooldown.
                    // If the durations are the same, then use the one which has the longer nominal time.
                    // E.g. if we have two abilities, one with a nominal duration of 10s and 2s respectively,
                    // but both have 1s remaining, then the "main" cooldown should be the 10s cooldown.
                    if (tDiff > 0) {
                        longestCooldownComponent = tempLongestCooldownComponent;
                    } else if (tDiff == 0 && tempLongestCooldownComponent.NominalDuration > longestCooldownComponent.NominalDuration) {
                        longestCooldownComponent = tempLongestCooldownComponent;
                    }
                }
                return longestCooldownComponent;
            }
        }

        /// <summary>
        /// If an entity has no active cooldowns active for an ability, we would never get the chance
        /// to update the cooldown to 0.  This inserts a default cooldown of '0', so we can
        /// use that as the default for each ability.
        /// </summary>
        [BurstCompile]
        protected struct CooldownAbilityIsZeroIfAbsentJob : IJobForEach<T, AbilityOwnerComponent> {
            public NativeMultiHashMap<Entity, GameplayEffectDurationComponent>.ParallelWriter GameplayEffectDurations;
            public void Execute([ReadOnly] ref T abilityTagComponent, [ReadOnly] ref AbilityOwnerComponent ownerComponent) {
                GameplayEffectDurations.Add(ownerComponent, GameplayEffectDurationComponent.Initialise(0, 0));
            }
        }

        protected struct CheckAbilityAvailableJob : IJobForEach<T> {
            public void Execute(ref T abilityTagComponent) {
                if (abilityTagComponent.DurationComponent.RemainingTime <= 0) {
                    abilityTagComponent.AbilityState = (int)AbilityStates.READY;
                } else {
                    abilityTagComponent.AbilityState = (int)AbilityStates.DISABLED;
                }
            }
        }
    }
}
