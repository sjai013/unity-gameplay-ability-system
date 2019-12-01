using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.AbilitySystem.Components;
using MyGameplayAbilitySystem.Abilities;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerCastActionSystem : ComponentSystem {
    protected override void OnUpdate() {

        Entities.WithAll<PlayerCastingControllableTagComponent>().ForEach((Entity entity, ActorAbilitySystem actorAbilitySystem, ref Translation translation, ref Rotation rotation) => {
            var forwardVector = math.normalize(math.mul(rotation.Value, new float3(0, 0, 1)));
            var AbilityState = -1;
            if (Input.GetKeyUp("1")) {


                // Check if player can use ability (the GrantedAbility entity contains this information)
                Entities
                    .WithAllReadOnly<AbilityOwnerComponent, AbilityStateComponent, DefaultAttackAbilityTag>().ForEach((ref AbilityStateComponent abilityState, ref AbilityOwnerComponent abilityOwner) => {
                        if (abilityOwner == actorAbilitySystem.AbilityOwnerEntity) {
                            AbilityState = abilityState;
                        }
                    });

                if (AbilityState != 0) {
                    // We could also do checks here to see what the ability state is and report an appropriate error on screen
                    // For example, we are using AbilityState = 2 to note that the ability is on cooldown, so we could display
                    // something like "This ability is on cooldown" on the screen.
                    return;
                }
                // Default attack - trigger cooldown and resource cost
                (new DefaultAttackAbilityTag()).CreateCooldownEntities(EntityManager, actorAbilitySystem.AbilityOwnerEntity);
                (new DefaultAttackAbilityTag()).CreateSourceAttributeModifiers(EntityManager, actorAbilitySystem.AbilityOwnerEntity);

                // Raycast forward, and if we hit something, reduce it's HP.
                RaycastHit hit;
                float3 rayOrigin = actorAbilitySystem.CastPoint.transform.position;
                // Does the ray intersect any objects
                if (!Physics.Raycast(rayOrigin, forwardVector, out hit, Mathf.Infinity)) {
                    return;
                }
                if (hit.distance < 1f) {
                    Debug.DrawRay(rayOrigin, forwardVector * hit.distance, Color.black, 1f);
                    if (hit.transform.parent.gameObject.TryGetComponent<ActorAbilitySystem>(out var targetActorAbilitySystemMono)) {
                        var targetEntity = targetActorAbilitySystemMono.AbilityOwnerEntity;
                        (new DefaultAttackAbilityTag()).CreateTargetAttributeModifiers(EntityManager, targetEntity);
                    }
                }
            }
        });
    }
}