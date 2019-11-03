using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ActorAbilitySystemAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {
    // Add fields to your component here. Remember that:
    //
    // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
    //   running.
    // 
    // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
    //   must be one of the supported types.
    //
    // For example,
    //    public float scale;
    public CharacterAttributesScriptableObject Attributes;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        List<ComponentType> attributeTypes = new List<ComponentType>();

        // Get reference to character attribute component on script, and list of attributes
        attributeTypes = new List<ComponentType>();
        if (Attributes != null && Attributes.Attributes != null) {
            attributeTypes = Attributes.ComponentArchetype;
        }

        // Add tag component to indicate that this entity represents an actor with attributes
        attributeTypes.Add(typeof(AbilitySystemActor));
        attributeTypes.Add(typeof(TestAbilityTag));
        var attributeArchetype = dstManager.CreateArchetype(attributeTypes.ToArray());
        // Create a new entity for this actor
        var abilitySystemEntity = dstManager.CreateEntity(attributeArchetype);
        dstManager.SetComponentData(abilitySystemEntity, new AbilitySystemActor
        {
            TransformEntity = entity
        });
        dstManager.SetName(abilitySystemEntity, this.gameObject.name + " - GameplayAbilitySystem");
    }

    /// <summary>
    /// For testing cooldowns
    /// </summary>
    /// <param name="dstManager"></param>
    /// <param name="abilitySystemEntity"></param>
    private void TestAbilitySystemCooldown(EntityManager dstManager, Entity abilitySystemEntity) {

        var cooldownArchetype = dstManager.CreateArchetype(
            typeof(GameplayEffectDurationComponent),
            typeof(GameplayEffectTargetComponent),
            typeof(GlobalCooldownGameplayEffectComponent));

        var cooldownEntity = dstManager.CreateEntity(cooldownArchetype);
        dstManager.SetComponentData<GameplayEffectTargetComponent>(cooldownEntity, abilitySystemEntity);
        dstManager.SetComponentData<GameplayEffectDurationComponent>(cooldownEntity, new GameplayEffectDurationComponent
        {
            RemainingTime = 10,
            WorldStartTime = 1
        });

        var cooldownEntity2 = dstManager.CreateEntity(cooldownArchetype);
        dstManager.SetComponentData<GameplayEffectTargetComponent>(cooldownEntity2, abilitySystemEntity);
        dstManager.SetComponentData<GameplayEffectDurationComponent>(cooldownEntity2, new GameplayEffectDurationComponent
        {
            RemainingTime = 5,
            WorldStartTime = 1
        });
    }
}

