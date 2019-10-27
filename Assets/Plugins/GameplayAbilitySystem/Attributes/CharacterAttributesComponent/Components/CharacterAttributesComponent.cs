using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Components {
    [DisallowMultipleComponent]
    public class CharacterAttributesComponent : MonoBehaviour {
        [SerializeField]
        [HideInInspector]
        public List<string> Attributes = new List<string>();

        public List<ComponentType> ComponentArchetype => ConvertAttributesToTypes();
        private List<ComponentType> ConvertAttributesToTypes() {
            List<ComponentType> types = new List<ComponentType>(Attributes.Count);
            for (var i = 0; i < Attributes.Count; i++) {
                types.Add(Type.GetType(Attributes[i]));
            }
            return types;
        }
    }
    public struct ActorWithAttributes : IComponentData {
        public Entity TransformEntity;
    }

}

