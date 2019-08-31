using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Heal {
    public class HealAbilityActivationSystem : AbilityActivationSystem<HealAbilityComponent> {
        public GameObject Prefab;

        protected override void OnUpdate() {
            // This is a simple system, where there is only one projectile to worry about.
            // We could create a new entity to capture the position of the projectile, but to keep it simple
            // we use the existing entity

            Entities.WithAll<HealAbilityComponent, AbilityStateComponent, AbilitySourceTarget>().ForEach((Entity entity, ref HealAbilityComponent ability, ref AbilityStateComponent abilityState, ref AbilitySourceTarget sourceTarget) => {
                if (abilityState.State != EAbilityState.Activate) return;
                var transforms = GetComponentDataFromEntity<LocalToWorld>(true);
                var sourceTransform = transforms[sourceTarget.Source];
                var sourceAbilitySystem = EntityManager.GetComponentObject<AbilitySystemComponent>(sourceTarget.Source);
                var targetAbilitySystem = EntityManager.GetComponentObject<AbilitySystemComponent>(sourceTarget.Target);
                ActivateAbility(sourceAbilitySystem, targetAbilitySystem, entity, ability);
                abilityState.State = EAbilityState.Completed;
            });
        }

        private void ActivateAbility(AbilitySystemComponent Source, AbilitySystemComponent Target, Entity AbilityEntity, HealAbilityComponent ability) {
            var abilitySystemActor = Source.GetActor();


            //(_, var gameplayEventData) = await AbilitySystem.OnGameplayEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
            var gameplayEventData = new GameplayAbilitySystem.Events.GameplayEventData()
            {
                Instigator = Source,
                Target = Target
            };

            GameObject instantiatedProjectile = null;

            instantiatedProjectile = Object.Instantiate(Prefab);
            instantiatedProjectile.transform.position = abilitySystemActor.transform.position + new Vector3(0, 1.5f, 0) + abilitySystemActor.transform.forward * 1.2f;

            // Animation complete.  Spawn and send projectile at target
            // if (instantiatedProjectile != null) {
            //     SeekTargetAndDestroy(Source, Target, instantiatedProjectile, ability, AbilityEntity);
            // }


        }

    }
}