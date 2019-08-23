using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.Events;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UniRx.Async;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityStandardAssets.Cameras;

namespace GameplayAbilitySystem.Abilities.AbilityActivations {
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Range Attack")]
    public class RangeAttack : AbstractAbilityActivation {

        public GameObject Projectile;
        public Vector3 ProjectilePositionOffset;

        public GameplayEffect TargetGameplayEffect;
        public AnimationEvent CastingInitiated;
        public AnimationEvent FireProjectile;

        public AnimationEvent StartComboSeek;
        public AnimationEvent EndComboSeek;
        public GameplayTag WaitForEventTag;
        public string AnimationTriggerName;
        public string ProjectileFireTriggerName;
        public string CompletionAnimatorStateFullHash;

        public GameObject SpawnProjectile(LocalToWorld spawnLocation) {
            return Instantiate(Projectile);

        }

        public override void ActivateAbility(AbilitySystemComponent AbilitySystem, GameplayAbility Ability) {
            //ActivateAbility(AbilitySystem, new GameplayEventData());
            Ability.EndAbility(AbilitySystem);
        }


        private async Task<bool> WaitForComboPress(AnimationEvent StartEvent, AnimationEvent EndEvent) {
            return false;
        }

        private async void SeekTargetAndDestroy(IGameplayAbilitySystem AbilitySystem, GameplayEventData gameplayEventData, GameObject projectile) {
            await projectile.GetComponent<Projectile>().SeekTarget(gameplayEventData.Target.TargettingLocation.gameObject, gameplayEventData.Target.gameObject);
            _ = AbilitySystem.ApplyGameEffectToTarget(TargetGameplayEffect, gameplayEventData.Target);
            DestroyImmediate(projectile);
        }

        public override async void ActivateAbility(AbilitySystemComponent Source, AbilitySystemComponent Target, IAbility Ability) {
            var abilitySystemActor = Source.GetActor();
            var animationEventSystemComponent = abilitySystemActor.GetComponent<AnimationEventSystem>();
            var animatorComponent = abilitySystemActor.GetComponent<Animator>();
            Ability.State = AbilityState.Activated;

            // Make sure we have enough resources.  End ability if we don't

            animatorComponent.SetTrigger(AnimationTriggerName);
            //(_, var gameplayEventData) = await AbilitySystem.OnGameplayEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
            var gameplayEventData = new GameplayEventData()
            {
                Instigator = Source,
                Target = Target
            };

            List<GameObject> objectsSpawned = new List<GameObject>();

            GameObject instantiatedProjectile = null;

            await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == CastingInitiated);

            if (Projectile != null) {
                instantiatedProjectile = Instantiate(Projectile);
                instantiatedProjectile.transform.position = abilitySystemActor.transform.position + this.ProjectilePositionOffset + abilitySystemActor.transform.forward * 1.2f;
            }

            animatorComponent.SetTrigger(ProjectileFireTriggerName);

            await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == FireProjectile);

            // Animation complete.  Spawn and send projectile at target
            if (instantiatedProjectile != null) {
                SeekTargetAndDestroy(Source, gameplayEventData, instantiatedProjectile);
            }


            var beh = animatorComponent.GetBehaviour<AnimationBehaviourEventSystem>();
            await beh.StateEnter.WaitForEvent((animator, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));
        }
    }
}
