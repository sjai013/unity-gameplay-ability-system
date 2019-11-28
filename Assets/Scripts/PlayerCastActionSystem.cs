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
            if (Input.GetKeyUp("1")) {
                // Default attack - trigger cooldown and resource cost
                (new DefaultAttackAbilityTag()).CreateCooldownEntities(EntityManager, actorAbilitySystem.AbilityOwnerEntity);
                (new DefaultAttackAbilityTag()).CreateSourceAttributeModifiers(EntityManager, actorAbilitySystem.AbilityOwnerEntity);

                // Raycast forward, and if we hit something, reduce it's HP.
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (!Physics.Raycast(translation.Value + new float3(0f, 1f, 1f), forwardVector, out hit, Mathf.Infinity)) {
                    return;
                }
                Debug.DrawRay(translation.Value + new float3(0f, 1f, 1f), forwardVector * hit.distance * 1000, Color.black, 1f);
                if (hit.distance < 0.5f) {
                    Debug.Log("Did Hit " + hit.distance);
                    var targetEntity = hit.transform.gameObject.GetComponentInParent<ActorAbilitySystem>().AbilityOwnerEntity;
                    Debug.Log(targetEntity);
                }
            }
        });
    }
}