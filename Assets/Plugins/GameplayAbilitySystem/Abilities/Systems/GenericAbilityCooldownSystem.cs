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
using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Systems {
    public struct NullComponent : IComponentData { }

    public abstract class GenericAbilityCooldownSystem<T> : AbilityCooldownSystem<T>
    where T : struct, IAbilityTagComponent, IComponentData {

        protected EntityQuery CooldownEffectsQuery;
        protected EntityQuery GrantedAbilityQuery;

        /// <summary>
        /// The ability has these cooldowns.
        /// </summary>
        /// <value></value>
        protected abstract ComponentType[] CooldownEffects { get; }

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
            GrantedAbilityQuery = GetEntityQuery(ComponentType.ReadOnly<AbilitySystemActorTransformComponent>(), ComponentType.ReadOnly<AbilityOwnerComponent>(), ComponentType.ReadOnly<T>(), ComponentType.ReadWrite<AbilityCooldownComponent>(), ComponentType.ReadWrite<AbilityStateComponent>());
        }



        protected override JobHandle CooldownJobs(JobHandle inputDeps) {
            NativeMultiHashMap<Entity, AbilityCooldownComponent> Cooldowns = new NativeMultiHashMap<Entity, AbilityCooldownComponent>(CooldownEffectsQuery.CalculateEntityCount() * 2 + GrantedAbilityQuery.CalculateEntityCount(), Allocator.TempJob);

            // Collect all effects which act as cooldowns for this ability
            var job1 = new GatherCooldownGameplayEffectsJob { AbilityCooldowns = Cooldowns.AsParallelWriter() };
            var job2 = new GatherLongestCooldownPerEntity { AbilityCooldowns = Cooldowns };
            inputDeps = inputDeps
                        .ScheduleJob(job1, CooldownEffectsQuery)
                        .ScheduleJob(job2, GrantedAbilityQuery)
                        ;

            Cooldowns.Dispose(inputDeps);
            return inputDeps;
        }

        /// <summary>
        /// Gather all cooldown effects for this ability
        /// </summary>
        [BurstCompile]
        protected struct GatherCooldownGameplayEffectsJob : IJobForEach<GameplayEffectTargetComponent, GameplayEffectDurationComponent> {
            public NativeMultiHashMap<Entity, AbilityCooldownComponent>.ParallelWriter AbilityCooldowns;
            public void Execute([ReadOnly] ref GameplayEffectTargetComponent targetComponent, [ReadOnly] ref GameplayEffectDurationComponent durationComponent) {
                AbilityCooldowns.Add(targetComponent, durationComponent.Value);
            }
        }

        /// <summary>
        /// Get the longest cooldown for the ability for each entity
        /// </summary>
        [BurstCompile]
        [RequireComponentTag(typeof(AbilitySystemActorTransformComponent))]
        protected struct GatherLongestCooldownPerEntity : IJobForEach<T, AbilityCooldownComponent, AbilityOwnerComponent> {
            [ReadOnly] public NativeMultiHashMap<Entity, AbilityCooldownComponent> AbilityCooldowns;
            public void Execute([ReadOnly]ref T abilityTagComponent, [WriteOnly] ref AbilityCooldownComponent cooldown, [ReadOnly] ref AbilityOwnerComponent ownerComponent) {
                cooldown.Value = GetMaxFromNMHP(ownerComponent, AbilityCooldowns);
            }

            private AbilityCooldownComponent GetMaxFromNMHP(Entity entity, NativeMultiHashMap<Entity, AbilityCooldownComponent> values) {
                if (!values.TryGetFirstValue(entity, out var longestCooldownComponent, out var multiplierIt)) {
                    // If there are no active cooldowns for this actor, then use a default.
                    longestCooldownComponent = new AbilityCooldownComponent();
                }

                while (values.TryGetNextValue(out var tempLongestCooldownComponent, ref multiplierIt)) {
                    var tDiff = tempLongestCooldownComponent.Value.RemainingTime - longestCooldownComponent.Value.RemainingTime;
                    var newPercentRemaining = tempLongestCooldownComponent.Value.RemainingTime / tempLongestCooldownComponent.Value.NominalDuration;
                    var oldPercentRemaining = longestCooldownComponent.Value.RemainingTime / longestCooldownComponent.Value.NominalDuration;

                    // If the duration currently being evaluated has more time remaining than the previous one,
                    // use this as the cooldown.
                    // If the durations are the same, then use the one which has the longer nominal time.
                    // E.g. if we have two abilities, one with a nominal duration of 10s and 2s respectively,
                    // but both have 1s remaining, then the "main" cooldown should be the 10s cooldown.
                    if (tDiff > 0) {
                        longestCooldownComponent = tempLongestCooldownComponent;
                    } else if (tDiff == 0 && tempLongestCooldownComponent.Value.NominalDuration > longestCooldownComponent.Value.NominalDuration) {
                        longestCooldownComponent = tempLongestCooldownComponent;
                    }
                }
                return longestCooldownComponent;
            }
        }

        // protected struct CheckAbilityAvailableJob : IJobForEach<T> {
        //     public void Execute(ref T abilityTagComponent) {
        //         if (abilityTagComponent.DurationComponent.RemainingTime <= 0) {
        //             abilityTagComponent.AbilityState = (int)AbilityStates.READY;
        //         } else {
        //             abilityTagComponent.AbilityState = (int)AbilityStates.DISABLED;
        //         }
        //     }
        // }
    }
}
