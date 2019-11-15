using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Fire {
    public class FireAbilityActivationSystem : AbilityActivationSystem<FireAbilityComponent> {
        public GameObject Prefab;

        protected override void OnUpdate() {
            // This is a simple system, where there is only one projectile to worry about.
            // We could create a new entity to capture the position of the projectile, but to keep it simple
            // we use the existing entity

            Entities.WithAll<FireAbilityComponent, _AbilityStateComponent, AbilitySourceTargetComponent>().ForEach((Entity entity, ref FireAbilityComponent ability, ref _AbilityStateComponent abilityState, ref AbilitySourceTargetComponent sourceTarget) => {
                if (abilityState.State != EAbilityState.Activate) return;
                var transforms = GetComponentDataFromEntity<LocalToWorld>(true);
                var sourceTransform = transforms[sourceTarget.Source];
                var sourceAbilitySystem = EntityManager.GetComponentObject<AbilitySystemComponent>(sourceTarget.Source);
                var targetAbilitySystem = EntityManager.GetComponentObject<AbilitySystemComponent>(sourceTarget.Target);
                ActivateAbility(sourceAbilitySystem, targetAbilitySystem, entity, ability);
                abilityState.State = EAbilityState.Completed;
            });
        }

        private void ActivateAbility(AbilitySystemComponent Source, AbilitySystemComponent Target, Entity AbilityEntity, FireAbilityComponent ability) {
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
            if (instantiatedProjectile != null) {
                SeekTargetAndDestroy(Source, Target, instantiatedProjectile, ability, AbilityEntity);
            }


        }

        private async void SeekTargetAndDestroy(AbilitySystemComponent Source, AbilitySystemComponent Target, GameObject projectile, FireAbilityComponent Ability, Entity AbilityEntity) {
            await projectile.GetComponent<Projectile>().SeekTarget(Target.TargettingLocation.gameObject, Target.gameObject);
            var attributesComponent = GetComponentDataFromEntity<AttributesComponent>(false);
            Ability.ApplyGameplayEffects(World.Active.EntityManager, Source.entity, Target.entity, attributesComponent[Target.entity], Time.time);
            Object.DestroyImmediate(projectile);

        }
    }
}