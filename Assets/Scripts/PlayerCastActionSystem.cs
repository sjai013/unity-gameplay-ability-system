using MyGameplayAbilitySystem.Abilities;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using Unity.Entities;
using UnityEngine;

public class PlayerCastActionSystem : ComponentSystem {
    protected override void OnUpdate() {

        Entities.WithAll<PlayerCastingControllableTagComponent>().ForEach((Entity entity, ActorAbilitySystem actorAbilitySystem) => {
            if (Input.GetKeyUp("1")) {
                // Default attack - trigger cooldown and resource cost
                (new DefaultAttackAbilityTag()).CreateCooldownEntities(EntityManager, actorAbilitySystem.AbilityOwnerEntity);
                (new DefaultAttackAbilityTag()).CreateSourceAttributeModifiers(EntityManager, actorAbilitySystem.AbilityOwnerEntity);
            }
        });
    }
}