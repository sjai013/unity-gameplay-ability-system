using GameplayAbilitySystem.Abilities.Systems;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

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
        protected abstract EntityQuery CooldownEffectsQuery { get; }
        private EntityQuery _GrantedAbilityQuery;

        protected override void OnCreate() {
            _GrantedAbilityQuery = GetEntityQuery(ComponentType.ReadOnly<AbilitySystemActor>(), ComponentType.ReadWrite<T>());
        }

        [BurstCompile]
        struct GatherCooldownGameplayEffectsJob : IJobForEach<GameplayEffectTargetComponent, GameplayEffectDurationComponent> {
            public NativeMultiHashMap<Entity, GameplayEffectDurationComponent>.ParallelWriter GameplayEffectDurations;
            public void Execute(ref GameplayEffectTargetComponent targetComponent, ref GameplayEffectDurationComponent durationComponent) {
                GameplayEffectDurations.Add(targetComponent, durationComponent);
            }
        }

        [BurstCompile]
        [RequireComponentTag(typeof(AbilitySystemActor))]
        struct GatherLongestCooldownPerEntity : IJobForEachWithEntity<T> {
            [ReadOnly] public NativeMultiHashMap<Entity, GameplayEffectDurationComponent> GameplayEffectDurationComponent;
            public void Execute(Entity entity, int index, [ReadOnly]  ref T durationComponent) {
                durationComponent.DurationComponent = GetMaxFromNMHP(entity, GameplayEffectDurationComponent);
            }

            private GameplayEffectDurationComponent GetMaxFromNMHP(Entity entity, NativeMultiHashMap<Entity, GameplayEffectDurationComponent> values) {
                values.TryGetFirstValue(entity, out var longestCooldownComponent, out var multiplierIt);
                while (values.TryGetNextValue(out var tempLongestCooldownComponent, ref multiplierIt)) {
                    if (tempLongestCooldownComponent.RemainingTime > longestCooldownComponent.RemainingTime) {
                        longestCooldownComponent = tempLongestCooldownComponent;
                    }
                }
                return longestCooldownComponent;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = CooldownJobs(inputDeps);
            return inputDeps;
        }

        private JobHandle CooldownJobs(JobHandle inputDeps) {
            NativeMultiHashMap<Entity, GameplayEffectDurationComponent> Cooldowns = new NativeMultiHashMap<Entity, GameplayEffectDurationComponent>(CooldownEffectsQuery.CalculateEntityCount(), Allocator.TempJob);
            inputDeps = new GatherCooldownGameplayEffectsJob
            {
                GameplayEffectDurations = Cooldowns.AsParallelWriter()
            }.Schedule(CooldownEffectsQuery, inputDeps);

            inputDeps = new GatherLongestCooldownPerEntity
            {
                GameplayEffectDurationComponent = Cooldowns
            }.Schedule(_GrantedAbilityQuery, inputDeps);

            Cooldowns.Dispose(inputDeps);
            return inputDeps;
        }
    }
}


public interface IAbilityTagComponent {
    GameplayEffectDurationComponent DurationComponent { get; set; }
}


