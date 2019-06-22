using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UniRx.Async;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.AbilityActivations {
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability Logic/Ability")]
    public class RangeAttack : AbstractAbilityActivation {

        public GameObject Projectile;
        public Vector3 ProjectilePositionOffset;

        public GameplayEffect TargetGameplayEffect;
        public AnimationEvent FireProjectile;
        public GameplayTag WaitForEventTag;
        public string AnimationTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void ActivateAbility(IGameplayAbilitySystem AbilitySystem, IGameplayAbility Ability) {
            var abilitySystemActor = AbilitySystem.GetActor();
            var animationEventSystemComponent = abilitySystemActor.GetComponent<AnimationEventSystem>();
            var animatorComponent = abilitySystemActor.GetComponent<Animator>();

            // Make sure we have enough resources.  End ability if we don't

            (_, var gameplayEventData) = await AbilitySystem.OnGameplayEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
            animatorComponent.SetTrigger(AnimationTriggerName);
            List<GameObject> objectsSpawned = new List<GameObject>();

            GameObject instantiatedProjectile = null;

            if (Projectile != null) {
                instantiatedProjectile = Instantiate(Projectile);
                instantiatedProjectile.transform.position = abilitySystemActor.transform.position + this.ProjectilePositionOffset + abilitySystemActor.transform.forward * 1.2f;
            }

            await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == FireProjectile);



            // Animation complete.  Spawn and send projectile at target
            if (instantiatedProjectile != null) {
                await instantiatedProjectile.GetComponent<Projectile>().SeekTarget(gameplayEventData.Target.TargettingLocation.gameObject, gameplayEventData.Target.gameObject);
            }

            _ = AbilitySystem.ApplyGameEffectToTarget(TargetGameplayEffect, gameplayEventData.Target);


            DestroyImmediate(instantiatedProjectile);


            var beh = animatorComponent.GetBehaviour<AnimationBehaviourEventSystem>();
            await beh.StateEnter.WaitForEvent((animator, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));

            // Commit ability cost
            // TODO: ApplyCost();

            // Wait for some specific gameplay event
            // Not applicable for base activate

            // Commit ability cooldown
            //TODO: ApplyCooldown();

            // Apply game effect(s)

            // End Ability
            Ability.EndAbility(AbilitySystem);
        }

    }
}
