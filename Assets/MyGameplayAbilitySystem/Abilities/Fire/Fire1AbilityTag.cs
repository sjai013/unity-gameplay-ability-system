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
using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Common.Editor;
using GameplayAbilitySystem.GameplayEffects.Components;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using MyGameplayAbilitySystem.GameplayEffects.Components;
using Unity.Entities;
using UnityEngine;


namespace MyGameplayAbilitySystem.Abilities.Fire1 {
    [AbilitySystemDisplayName("Fire 1")]
    public struct Fire1AbilityTag : IAbilityTagComponent, IComponentData {
        private const string AnimationStartTriggerName = "DoMagic1";
        private const string AnimationEndTriggerName = "MagicCastComplete";
        private const string magicStateName = "Weapon.Magic";
        private const string animatorLayerName = "Weapon";

        public int AbilityIdentifier => 2;

        public object EmptyPayload => new BasicRangeAbilityPayload();


        public void BeginActivateAbility(EntityManager dstManager, Entity grantedAbilityEntity) {
            // Check if entity already has the "Active" component - return if existing
            if (dstManager.HasComponent<Fire1AbilityActive>(grantedAbilityEntity)) return;

            // Add component to entity
            dstManager.AddComponentData<Fire1AbilityActive>(grantedAbilityEntity, new Fire1AbilityActive());
        }

        public void CommitAbility(EntityManager dstManager, Entity actorEntity) {
            CreateCooldownEntities(dstManager, actorEntity);
            CreateSourceAttributeModifiers(dstManager, actorEntity);
        }

        public void CreateCooldownEntities(EntityManager dstManager, Entity actorEntity) {
            // Create a "Global Cooldown" gameplay effect, as would be created when a real ability is cast
            new GlobalCooldownGameplayEffectComponent().Instantiate(dstManager, actorEntity, 1f);

            // Create a "Global Cooldown" gameplay effect, as would be created when a real ability is cast
            new Fire1CooldownGameplayEffectComponent().Instantiate(dstManager, actorEntity, 5f);
        }

        public void CreateSourceAttributeModifiers(EntityManager dstManager, Entity actorEntity) {
            new PermanentAttributeModifierTag().CreateAttributeModifier<ManaAttributeComponent, GameplayAbilitySystem.Attributes.Components.Operators.Add>(dstManager, actorEntity, -5f);
        }

        public void CreateTargetAttributeModifiers(EntityManager dstManager, Entity actorEntity) {
            // Create fire damage effect which perform the damage
            new Fire1DamageGameplayEffectComponent() { Damage = -15f }.Instantiate(dstManager, actorEntity, -1f);
        }

        public IEnumerator DoAbility(object Payload) {
            if (Payload is BasicRangeAbilityPayload payload) {
                payload.ActorAbilitySystem.StartCoroutine(AbilityActionLogic(payload));

            } else {
                Debug.LogWarningFormat("The payload passed to {0} does not match the expected payload format.", this.GetType());
            }

            yield return null;
        }

        private IEnumerator AbilityActionLogic(BasicRangeAbilityPayload payload) {
            var entityManager = payload.EntityManager;
            var transform = payload.ActorTransform;
            var actorAbilitySystem = payload.ActorAbilitySystem;
            var grantedAbilityEntity = payload.GrantedAbilityEntity;

            var abilityStateComponent = entityManager.GetComponentData<AbilityStateComponent>(grantedAbilityEntity);

            if (abilityStateComponent.Value != 0) yield break;
            var animator = actorAbilitySystem.GetComponent<Animator>();


            var animatorStateFullHash = Animator.StringToHash(magicStateName);
            var animatorLayerIndex = animator.GetLayerIndex(animatorLayerName);

            if (!animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).IsName("Idle")) yield break;
            BeginActivateAbility(entityManager, grantedAbilityEntity);
            CreateSourceAttributeModifiers(entityManager, actorAbilitySystem.AbilityOwnerEntity);
            // Get animator state info
            var weaponLayerAnimatorStateInfo = GetAnimatorStateInfo(animator, animatorLayerIndex, magicStateName);

            animator.SetTrigger(AnimationStartTriggerName);
            // Complete casting animation
            var currentAnimatorState = animator.GetCurrentAnimatorStateInfo(animatorLayerIndex);

            while (!currentAnimatorState.IsName(magicStateName) || currentAnimatorState.normalizedTime < 1f) {
                yield return null;
                currentAnimatorState = animator.GetCurrentAnimatorStateInfo(animatorLayerIndex);
            }

            // while (!IsInOrEnteringAnimatorState(animator, animatorLayerIndex, magicStateName)) {
            //     yield return null;
            // }


            // Create a new fireball object
            var fireballPrefab = GameObject.Instantiate(payload.AbilityPrefab);
            // Set position of fireball to player's position
            fireballPrefab.transform.position = payload.ActorAbilitySystem.CastPoint.transform.position;
            fireballPrefab.transform.rotation = payload.ActorAbilitySystem.transform.transform.rotation;
            fireballPrefab.SetActive(true);

            var fireballParams = fireballPrefab.GetComponent<Fire1FireballEffect>();
            yield return payload.ActorAbilitySystem.StartCoroutine(CastSpell(entityManager, fireballParams));
            GameObject.Destroy(fireballPrefab);
            CreateCooldownEntities(entityManager, actorAbilitySystem.AbilityOwnerEntity);
            EndActivateAbility(entityManager, grantedAbilityEntity);
            animator.SetTrigger(AnimationEndTriggerName);


        }

        public IEnumerator CastSpell(EntityManager dstManager, Fire1FireballEffect FireballParams) {

            ActorAbilitySystem hitTarget = null;

            // Begin casting
            // Wait for cloud animation to complete
            FireballParams.Fireball.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.8f);
            FireballParams.Fireball.gameObject.SetActive(true);
            FireballParams.Fireball.transform.localPosition = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(0.1f);
            // Subscribe to fireball hitbox event
            FireballParams.Fireball.TriggerEnterEvent += HitTriggered;


            // Fire projectile straight, up to a particular distance
            float distanceTravelled = 0f;
            float penetrations = 0;
            while (distanceTravelled < FireballParams.maxProjectileDisplacement && penetrations < FireballParams.maxPenetration) {
                // Move projectile forward
                float displacement = Time.deltaTime * FireballParams.projectileSpeed;
                FireballParams.Fireball.transform.position = FireballParams.Fireball.transform.position + (FireballParams.Fireball.transform.forward * displacement);
                distanceTravelled += displacement;
                if (hitTarget != null) {
                    penetrations += 1;
                    // Create damage modifier on hit target
                    CreateTargetAttributeModifiers(dstManager, hitTarget.AbilityOwnerEntity);

                }
                yield return null;
            }

            FireballParams.Fireball.TriggerEnterEvent -= HitTriggered;
            FireballParams.Fireball.gameObject.SetActive(false);


            void HitTriggered(object sender, ColliderEventArgs e) {
                hitTarget = e.other.gameObject.GetComponent<HurtboxMonoComponent>().ActorAbilitySystem;
            }


        }



        public void EndActivateAbility(EntityManager dstManager, Entity grantedAbilityEntity) {
            // Check if entity already has the "Active" component - return if not existing
            if (!dstManager.HasComponent<Fire1AbilityActive>(grantedAbilityEntity)) return;

            // Remove component from entity
            dstManager.RemoveComponent<Fire1AbilityActive>(grantedAbilityEntity);
        }

        AnimatorStateInfo GetAnimatorStateInfo(Animator animator, int layerIndex, string animatorStateName) {
            var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            var animatorStateInfoNext = animator.GetNextAnimatorStateInfo(layerIndex);
            if (animatorStateInfoNext.IsName(animatorStateName)) animatorStateInfo = animatorStateInfoNext;
            return animatorStateInfo;
        }


        bool IsInOrEnteringAnimatorState(Animator animator, int layerIndex, string animatorStateName) {
            return GetAnimatorStateInfo(animator, layerIndex, animatorStateName).IsName(animatorStateName);
        }

    }

}
