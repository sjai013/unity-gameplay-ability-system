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

using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Abilities.Systems {

    /// <summary>
    /// Defines the system for handling ability parameters, such as
    /// current cooldown for each actor.
    /// 
    /// <para>
    /// The set of components that are returned for CooldownQueryDesc must contain <see cref="GameplayEffectTargetComponent"/>
    /// and <see cref="GameplayEffectDurationComponent"/>. Other components should include the relevant gameplay effects.
    /// </para>
    /// 
    /// </summary>
    /// <typeparam name="T">The Ability</typeparam>
    public abstract class AbilitySystem<T> : JobComponentSystem
    where T : struct, IAbilityTagComponent, IComponentData {
        protected abstract JobHandle CheckAbilityAvailable(JobHandle inputDeps);
        protected abstract JobHandle CooldownJobs(JobHandle inputDeps);

        /// <summary>
        /// Gather all cooldown effects for this ability
        /// </summary>
        [BurstCompile]
        protected struct GatherCooldownGameplayEffectsJob : IJobForEach<GameplayEffectTargetComponent, GameplayEffectDurationComponent> {
            public NativeMultiHashMap<Entity, GameplayEffectDurationComponent>.ParallelWriter GameplayEffectDurations;
            public void Execute(ref GameplayEffectTargetComponent targetComponent, ref GameplayEffectDurationComponent durationComponent) {
                GameplayEffectDurations.Add(targetComponent, durationComponent);
            }
        }

        /// <summary>
        /// Get the longest cooldown for the ability for each entity
        /// </summary>
        [BurstCompile]
        [RequireComponentTag(typeof(AbilitySystemActor))]
        protected struct GatherLongestCooldownPerEntity : IJobForEachWithEntity<T> {
            [ReadOnly] public NativeMultiHashMap<Entity, GameplayEffectDurationComponent> GameplayEffectDurationComponent;
            public void Execute(Entity entity, int index, [ReadOnly]  ref T durationComponent) {
                durationComponent.DurationComponent = GetMaxFromNMHP(entity, GameplayEffectDurationComponent);
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
        protected struct CooldownAbilityIsZeroIfAbsentJob : IJobForEachWithEntity<T> {
            public NativeMultiHashMap<Entity, GameplayEffectDurationComponent>.ParallelWriter GameplayEffectDurations;
            public void Execute(Entity entity, int index, ref T abilityDurationComponent) {
                GameplayEffectDurations.Add(entity, GameplayEffectDurationComponent.Initialise(0, 0));
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = CooldownJobs(inputDeps);
            inputDeps = CheckAbilityAvailable(inputDeps);
            return inputDeps;
        }

    }
}


public interface IAbilityTagComponent {
    GameplayEffectDurationComponent DurationComponent { get; set; }
}


