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

using System.Collections;
using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.Abilities.Systems;
using GameplayAbilitySystem.Abilities.Systems.Generic;
using GameplayAbilitySystem.AbilitySystem.Enums;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Common.Editor;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.GameplayEffects.Components;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace MyGameplayAbilitySystem.Abilities {

    [AbilitySystemDisplayName("Default Attack Ability")]
    public struct DefaultAttackAbilityTag : IAbilityTagComponent, IComponentData {
        private const string AnimationStartTriggerName = "DoSwingAttack";

        public int AbilityIdentifier => 1;

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

        public IEnumerator CheckAbilityHit(EntityManager EntityManager, Entity sourceEntity, Entity targetEntity) {
            yield return null;
        }

        public IEnumerator DoAbility(object Payload) {
            if (Payload is Payload payload) {
                payload.ActorAbilitySystem.StartCoroutine(AbilityActionLogic(payload));
            } else {
                Debug.LogWarningFormat("The payload passed to {0} does not match the expected payload format.", this.GetType());
            }

            yield return null;
        }

        private IEnumerator AbilityActionLogic(Payload payload) {
            var entityManager = payload.EntityManager;
            var transform = payload.ActorTransform;
            var actorAbilitySystem = payload.ActorAbilitySystem;
            var grantedAbilityEntity = payload.GrantedAbilityEntity;
            // Check ability state
            var abilityStateComponent = entityManager.GetComponentData<AbilityStateComponent>(grantedAbilityEntity);

            if (abilityStateComponent.Value != 0) yield break;
            var animator = actorAbilitySystem.GetComponent<Animator>();
            BeginActivateAbility(entityManager, grantedAbilityEntity);
            CreateSourceAttributeModifiers(entityManager, actorAbilitySystem.AbilityOwnerEntity);
            var animatorLayerName = "Weapon";
            var animatorStateName = animatorLayerName + ".Swing";
            var animatorStateFullHash = Animator.StringToHash(animatorStateName);
            var animatorLayerIndex = animator.GetLayerIndex(animatorLayerName);

            // Get animator state info
            var weaponLayerAnimatorStateInfo = GetAnimatorStateInfo(animator, animatorLayerIndex, animatorStateName);

            animator.SetTrigger(AnimationStartTriggerName);
            // Wait to reach the "Swing" state
            while (!weaponLayerAnimatorStateInfo.IsName(animatorStateName)) {
                yield return null;
                weaponLayerAnimatorStateInfo = GetAnimatorStateInfo(animator, animatorLayerIndex, animatorStateName);
            }

            // In the swing state, do raycast and hit damage
            // Raycast forward, and if we hit something, reduce it's HP.
            RaycastHit hit;
            Vector3 rayOrigin = actorAbilitySystem.CastPoint.transform.position;
            var forwardVector = actorAbilitySystem.transform.forward;
            // Does the ray intersect any objects
            if (!Physics.Raycast(rayOrigin, forwardVector, out hit, Mathf.Infinity)) {
                yield break;
            }
            bool wasHit = false;
            Entity targetEntity = default(Entity);
            if (hit.distance < 1f) {
                Debug.DrawRay(rayOrigin, forwardVector * hit.distance, Color.black, 1f);
                Debug.Log(hit.transform.gameObject);
                if (hit.transform.TryGetComponent<HurtboxMonoComponent>(out var hurtboxComponent)) {
                    targetEntity = hurtboxComponent.ActorAbilitySystem.AbilityOwnerEntity;
                    Debug.Log(hurtboxComponent.ActorAbilitySystem.gameObject);
                    wasHit = true;
                }
            }

            Debug.Log(targetEntity);
            // Once we are more than 50% through the animation, trigger the damage
            while (weaponLayerAnimatorStateInfo.normalizedTime < 0.5f) {
                yield return null;
                weaponLayerAnimatorStateInfo = GetAnimatorStateInfo(animator, animatorLayerIndex, animatorStateName);
            }

            CreateTargetAttributeModifiers(entityManager, targetEntity);
            while (weaponLayerAnimatorStateInfo.IsName("Weapon.Swing")) {
                yield return null;
                weaponLayerAnimatorStateInfo = GetAnimatorStateInfo(animator, animatorLayerIndex, animatorStateName);
            }
            // Once we are no longer in the swing animation, commit the ability
            CreateCooldownEntities(entityManager, actorAbilitySystem.AbilityOwnerEntity);
            EndActivateAbility(entityManager, grantedAbilityEntity);
            yield return null;
        }

        AnimatorStateInfo GetAnimatorStateInfo(Animator animator, int layerIndex, string animatorStateName) {
            var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            var animatorStateInfoNext = animator.GetNextAnimatorStateInfo(layerIndex);
            if (animatorStateInfoNext.IsName(animatorStateName)) animatorStateInfo = animatorStateInfoNext;
            return animatorStateInfo;
        }
        public class Activation {

        }

        public struct Payload {
            public EntityManager EntityManager;
            public Transform ActorTransform;
            public ActorAbilitySystem ActorAbilitySystem;
            public Entity GrantedAbilityEntity;
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

        public class AssignAbilityIdentifierSystem : GenericAssignAbilityIdentifierSystem<DefaultAttackAbilityTag> { }

    }

}
