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
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Common.Editor;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MyGameplayAbilitySystem.Abilities {

    [AbilitySystemDisplayName("Default Attack Ability")]
    public struct DefaultAttackAbilityTag : IAbilityTagComponent, IComponentData {
        public void CreateCooldownEntities(EntityManager dstManager, Entity actorEntity) {
            var cooldownArchetype1 = dstManager.CreateArchetype(
                                    typeof(GameplayEffectDurationComponent),
                                    typeof(GameplayEffectTargetComponent),
                                    typeof(GlobalCooldownGameplayEffectComponent));

            var cooldownEntity1 = dstManager.CreateEntity(cooldownArchetype1);
            dstManager.SetComponentData<GameplayEffectTargetComponent>(cooldownEntity1, actorEntity);
            dstManager.SetComponentData<GameplayEffectDurationComponent>(cooldownEntity1, GameplayEffectDurationComponent.Initialise(0.2f, UnityEngine.Time.time));
        }

        public void CreateSourceAttributeModifiers(EntityManager dstManager, Entity actorEntity) {
            var archetype = dstManager.CreateArchetype(
                            typeof(GameplayAbilitySystem.Attributes.Components.Operators.Add),
                            typeof(AttributeComponentTag<ManaAttributeComponent>),
                            typeof(AttributeModifier<GameplayAbilitySystem.Attributes.Components.Operators.Add, ManaAttributeComponent>),
                            typeof(AttributesOwnerComponent)
                        );

            var entity = dstManager.CreateEntity(archetype);
            dstManager.SetComponentData(entity, new AttributeModifier<GameplayAbilitySystem.Attributes.Components.Operators.Add, ManaAttributeComponent>()
            {
                Value = -1
            });

            dstManager.SetComponentData(entity, new AttributesOwnerComponent()
            {
                Value = actorEntity
            });
        }

        public void CommitAbility(EntityManager dstManager, Entity actorEntity) {
            CreateCooldownEntities(dstManager, actorEntity);
            CreateSourceAttributeModifiers(dstManager, actorEntity);
        }
        public void CreateTargetAttributeModifiers(EntityManager dstManager, Entity actorEntity) {
            var archetype = dstManager.CreateArchetype(
                            typeof(GameplayAbilitySystem.Attributes.Components.Operators.Add),
                            typeof(AttributeComponentTag<HealthAttributeComponent>),
                            typeof(AttributeModifier<GameplayAbilitySystem.Attributes.Components.Operators.Add, HealthAttributeComponent>),
                            typeof(AttributesOwnerComponent)
                        );

            var entity = dstManager.CreateEntity(archetype);
            dstManager.SetComponentData(entity, new AttributeModifier<GameplayAbilitySystem.Attributes.Components.Operators.Add, HealthAttributeComponent>()
            {
                Value = -5
            });

            dstManager.SetComponentData(entity, new AttributesOwnerComponent()
            {
                Value = actorEntity
            });
        }

        public void BeginActivateAbility(EntityManager dstManager, Entity grantedAbilityEntity) {
            // Check if entity already has the "Active" component - return if existing
            if (dstManager.HasComponent<DefaultAttackAbilityActive>(grantedAbilityEntity)) return;

            // Add component to entity
            dstManager.AddComponentData<DefaultAttackAbilityActive>(grantedAbilityEntity, new DefaultAttackAbilityActive());
        }

        public void EndActivateAbility(EntityManager dstManager, Entity grantedAbilityEntity) {
            // Check if entity already has the "Active" component - return if not existing
            if (!dstManager.HasComponent<DefaultAttackAbilityActive>(grantedAbilityEntity)) return;

            // Remove component from entity
            dstManager.RemoveComponent<DefaultAttackAbilityActive>(grantedAbilityEntity);
        }
    }

    public class DefaultAttackAbilitySystem {
        public class AbilityCooldownSystem : GenericAbilityCooldownSystem<DefaultAttackAbilityTag> {
            protected override ComponentType[] CooldownEffects =>
                new ComponentType[] {
                    ComponentType.ReadOnly<GlobalCooldownGameplayEffectComponent>()
                };

        }

        public class AbilityAvailabilitySystem : AbilityAvailabilitySystem<DefaultAttackAbilityTag> {
            // private EntityQuery m_Query;
            // protected override void OnCreate() {
            //     this.m_Query = GetEntityQuery(ComponentType.ReadOnly<DefaultAttackAbilityActive>(), ComponentType.ReadWrite<AbilityStateComponent>());
            // }

            [RequireComponentTag(typeof(DefaultAttackAbilityActive))]
            struct Job : IJobForEach<AbilityStateComponent> {
                public void Execute(ref AbilityStateComponent abilityState) {
                    abilityState |= (int)AbilityStates.ACTIVE;
                }
            }
            protected override JobHandle UpdateAbilityAvailability(JobHandle inputDeps) {
                // Check for existence of AbilityActive tag
                inputDeps = inputDeps.ScheduleJob(new Job(), this);
                return inputDeps;
            }
        }

        public class AssignAbilityIdentifierSystem : GenericAssignAbilityIdentifierSystem<DefaultAttackAbilityTag> {
            protected override int AbilityIdentifier => 1;
        }

    }

}
