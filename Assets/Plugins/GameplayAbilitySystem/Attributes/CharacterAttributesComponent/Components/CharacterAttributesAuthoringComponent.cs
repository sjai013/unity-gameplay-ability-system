using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Components {
    [DisallowMultipleComponent]
    public class CharacterAttributesAuthoringComponent : MonoBehaviour {
        public CharacterAttributesScriptableObject Attributes;
    }

    public struct ActorWithAttributes : IComponentData {
        public Entity TransformEntity;
    }
}

