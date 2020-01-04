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
using System.Collections;
using System.Runtime.InteropServices;
using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.Abilities.Systems;
using GameplayAbilitySystem.Abilities.Systems.Generic;
using GameplayAbilitySystem.AbilitySystem.Enums;
using GameplayAbilitySystem.ExtensionMethods;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using MyGameplayAbilitySystem.GameplayEffects.Components;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Mathematics;
using Unity.Collections;
using GameplayAbilitySystem.Common.Components;

namespace MyGameplayAbilitySystem.Abilities.DefaultAttack {

    public class DefaultAttackAbilitySystem {
        [UpdateInGroup(typeof(AbilityUpdateSystemGroup))]
        public class AbilityCooldownSystem : JobComponentSystem {
            EntityQuery m_GEQuery;
            protected override JobHandle OnUpdate(JobHandle inputDeps) {
                var Cooldown1DurationComponents = GetComponentDataFromEntity<GameplayEffectDurationComponent>();
                var Cooldown1TagComponents = GetComponentDataFromEntity<GlobalCooldownGameplayEffectComponent>();
                var AbilityTagComponents = GetComponentDataFromEntity<DefaultAttackAbilityTag>();
                var AbilityCooldownComponents = GetComponentDataFromEntity<AbilityCooldownComponent>();
                // NativeHashMap<Entity, GameplayEffectDurationComponent> AbilityCooldownDurations = new NativeHashMap<Entity, GameplayEffectDurationComponent>(m_GEQuery.CalculateEntityCount(), Allocator.TempJob);
                inputDeps = Entities.ForEach((Entity entity, int entityInQueryIndex, in DynamicBuffer<GameplayEffectBufferElement> gameplayEffectBuffer, in DynamicBuffer<GrantedAbilityBufferElement> grantedAbilityBuffer) => {
                    // The outer foreach loops through all actors
                    var abilityCooldownDuration = new GameplayEffectDurationComponent();
                    for (var i = 0; i < gameplayEffectBuffer.Length; i++) {
                        // The inner loop is iterating through all active GE on a single actor
                        var gameplayEffectEntity = gameplayEffectBuffer[i].Value;
                        // Check if entity has any of the cooldown components
                        if (Cooldown1DurationComponents.HasComponent(gameplayEffectEntity) && Cooldown1TagComponents.HasComponent(gameplayEffectEntity)) {
                            var durationComponent = Cooldown1DurationComponents[gameplayEffectEntity];

                            bool thisEffectIsLongest = durationComponent.Value > abilityCooldownDuration.Value;
                            abilityCooldownDuration.Value.RemainingTime = math.select(abilityCooldownDuration.Value.RemainingTime, durationComponent.Value.RemainingTime, thisEffectIsLongest);
                            abilityCooldownDuration.Value.NominalDuration = math.select(abilityCooldownDuration.Value.NominalDuration, durationComponent.Value.NominalDuration, thisEffectIsLongest);
                            abilityCooldownDuration.Value.WorldStartTime = math.select(abilityCooldownDuration.Value.WorldStartTime, durationComponent.Value.WorldStartTime, thisEffectIsLongest);
                        }
                    }
                    for (var i = 0; i < grantedAbilityBuffer.Length; i++) {
                        if (AbilityTagComponents.HasComponent(grantedAbilityBuffer[i])) {
                            AbilityCooldownComponents[grantedAbilityBuffer[i]] = new AbilityCooldownComponent
                            {
                                Value = abilityCooldownDuration
                            };
                            break;
                        }
                    }
                })
                .WithStoreEntityQueryInField(ref m_GEQuery)
                .WithReadOnly(Cooldown1DurationComponents)
                .WithReadOnly(Cooldown1TagComponents)
                .WithReadOnly(AbilityTagComponents)
                .WithNativeDisableParallelForRestriction(AbilityCooldownComponents)
                .Schedule(inputDeps);
                return inputDeps;
            }
        }

        public class AbilityAvailabilitySystem : AbilityAvailabilitySystem<DefaultAttackAbilityTag> {
            // private EntityQuery m_Query;
            // protected override void OnCreate() {
            //     this.m_Query = GetEntityQuery(ComponentType.ReadOnly<DefaultAttackAbilityActive>(), ComponentType.ReadWrite<AbilityStateComponent>());
            // }

            [RequireComponentTag(typeof(DefaultAttackAbilityActive))]
            struct SystemJob : IJobForEach<AbilityStateComponent> {
                public void Execute(ref AbilityStateComponent abilityState) {
                    abilityState |= (int)AbilityStates.ACTIVE;
                }
            }
            protected override JobHandle UpdateAbilityAvailability(JobHandle inputDeps) {
                // Check for existence of AbilityActive tag
                inputDeps = inputDeps.ScheduleJob(new SystemJob(), this);
                return inputDeps;
            }
        }

        public class AssignAbilityIdentifierSystem : GenericAssignAbilityIdentifierSystem<DefaultAttackAbilityTag> { }

    }

}
