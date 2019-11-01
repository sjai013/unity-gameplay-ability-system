using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
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

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        List<ComponentType> attributeTypes = new List<ComponentType>();

        // Get reference to character attribute component on script, and list of attributes
        attributeTypes = new List<ComponentType>();
        if (this.TryGetComponent<CharacterAttributesAuthoringComponent>(out var component)) {
            if (component.Attributes != null && component.Attributes.Attributes != null) {
                attributeTypes = component.Attributes.ComponentArchetype;
            }
        }

        // Add tag component to indicate that this entity represents an actor with attributes
        attributeTypes.Add(typeof(ActorWithAttributes));
        var attributeArchetype = dstManager.CreateArchetype(attributeTypes.ToArray());
        // Create a new entity for this actor
        var attributeEntity = dstManager.CreateEntity(attributeArchetype);
        dstManager.SetComponentData(attributeEntity, new ActorWithAttributes
        {
            TransformEntity = entity
        });
        dstManager.SetName(attributeEntity, this.gameObject.name + " - Attributes");
    }
}

