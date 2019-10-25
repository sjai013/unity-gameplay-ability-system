using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Components {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class CharacterAttributesComponent : MonoBehaviour, IConvertGameObjectToEntity {
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

        [SerializeField]
        [HideInInspector]
        public List<string> Attributes = new List<string>();


        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {

            // Add components defined by Attributes using reflection to get object.  Initialise with default 0 values.
            for (var i = 0; i < Attributes.Count; i++) {
                var type = Type.GetType(Attributes[i]);
                dstManager.AddComponent(entity, type);
            }
            dstManager.AddComponent(entity, typeof(ActorWithAttributesTag));
        }
    }

    struct ActorWithAttributesTag : IComponentData { }
}

