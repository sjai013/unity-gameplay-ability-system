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

using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.Abilities.Systems;
using GameplayAbilitySystem.Abilities.Systems.Generic;
using GameplayAbilitySystem.AbilitySystem.Enums;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.GameplayEffects.Components;
using MyGameplayAbilitySystem.GameplayEffects.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MyGameplayAbilitySystem.Abilities.Fire1 {
    public class Fire1AbilitySystem {

        [UpdateInGroup(typeof(AbilityUpdateSystemGroup))]
        public class AbilityCooldownSystem : JobComponentSystem {
            EntityQuery m_GEQuery;
            protected override JobHandle OnUpdate(JobHandle inputDeps) {
                var CooldownDurationComponents = GetComponentDataFromEntity<GameplayEffectDurationComponent>();
                var Cooldown1TagComponents = GetComponentDataFromEntity<GlobalCooldownGameplayEffectComponent>();
                var Cooldown2TagComponents = GetComponentDataFromEntity<Fire1CooldownGameplayEffectComponent>();
                var AbilityTagComponents = GetComponentDataFromEntity<Fire1AbilityTag>();
                var AbilityCooldownComponents = GetComponentDataFromEntity<AbilityCooldownComponent>();

                // Set the cooldown of the ability for each actor
                inputDeps = Entities.ForEach((Entity entity, int entityInQueryIndex, in DynamicBuffer<GameplayEffectBufferElement> gameplayEffectBuffer, in DynamicBuffer<GrantedAbilityBufferElement> grantedAbilityBuffer) => {
                    // The outer foreach loops through all actors
                    var abilityCooldownDuration = new GameplayEffectDurationComponent();
                    for (var i = 0; i < gameplayEffectBuffer.Length; i++) {
                        // The inner loop is iterating through all active GE on a single actor
                        var gameplayEffectEntity = gameplayEffectBuffer[i].Value;
                        // Check if entity has any of the cooldown components
                        int cooldownDurationComponentThatHas = 0;
                        cooldownDurationComponentThatHas = math.select(1, cooldownDurationComponentThatHas, Cooldown1TagComponents.HasComponent(gameplayEffectEntity));
                        cooldownDurationComponentThatHas = math.select(2, cooldownDurationComponentThatHas, Cooldown2TagComponents.HasComponent(gameplayEffectEntity));
                        if (cooldownDurationComponentThatHas > 0) {
                            if (!CooldownDurationComponents.HasComponent(gameplayEffectEntity)) {
                                continue;
                            }
                            var durationComponent = CooldownDurationComponents[gameplayEffectEntity];

                            // Cooldown selection logic:
                            // 1. Effect with the longest remaining duration is always the ability's cooldown remaining duration
                            // 2. If two effects have the same remaining duration, then the effect with the longest period is the ability's cooldown duration
                            bool thisEffectIsLongest = durationComponent > abilityCooldownDuration;
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
                .WithReadOnly(CooldownDurationComponents)
                .WithReadOnly(Cooldown1TagComponents)
                .WithReadOnly(Cooldown2TagComponents)
                .WithReadOnly(AbilityTagComponents)
                .WithNativeDisableParallelForRestriction(AbilityCooldownComponents)
                .Schedule(inputDeps);
                return inputDeps;
            }
        }

        public class AbilityAvailabilitySystem : AbilityAvailabilitySystem<Fire1AbilityTag> {
            [RequireComponentTag(typeof(Fire1AbilityActive))]
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
        public class AssignAbilityIdentifierSystem : GenericAssignAbilityIdentifierSystem<Fire1AbilityTag> { }
    }

}
