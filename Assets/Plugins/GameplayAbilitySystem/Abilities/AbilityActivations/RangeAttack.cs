using System;
using System.Collections.Generic;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.AbilityActivations
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability Logic/Ability")]
    public class RangeAttack : AbstractAbilityActivation
    {
        public GameplayEffect TargetGameplayEffect;
        public AnimationEvent FireProjectile;
        public GameplayTag WaitForEventTag;
        public string AnimationTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void ActivateAbility(IGameplayAbilitySystem AbilitySystem, IGameplayAbility Ability)
        {
            var abilitySystemActor = AbilitySystem.GetActor();
            var animationEventSystemComponent = abilitySystemActor.GetComponent<AnimationEventSystem>();
            var animatorComponent = abilitySystemActor.GetComponent<Animator>();

            // Make sure we have enough resources.  End ability if we don't

            (_, var gameplayEventData) = await AbilitySystem.OnGameplayEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
            animatorComponent.SetTrigger(AnimationTriggerName);
            List<GameObject> objectsSpawned = new List<GameObject>();

            // projectile.transform.position = abilitySystemActor.transform.position + abilitySystemActor.transform.forward * 1.2f + new Vector3(0, 1.5f, 0);
            await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == FireProjectile);
            AbilitySystem.ApplyGameEffectToTarget(TargetGameplayEffect, gameplayEventData.Target);

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
