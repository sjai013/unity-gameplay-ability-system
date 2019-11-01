using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Components {
    [CreateAssetMenu(fileName = "ActorAttributes", menuName = "Gameplay Ability System/Attributes/Attributes Prototype")]
    public class CharacterAttributesScriptableObject : ScriptableObject {
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
}