using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameplayAbilitySystem.Attributes.Components {
    [Serializable]
    public struct AttributesOwnerSharedComponent : ISharedComponentData {
        public Entity Value;
        public static implicit operator Entity(AttributesOwnerSharedComponent e) { return e.Value; }
        public static implicit operator AttributesOwnerSharedComponent(Entity e) { return new AttributesOwnerSharedComponent { Value = e }; }
    }
}
