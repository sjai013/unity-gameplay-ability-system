using System;
using Unity.Entities;

namespace GameplayAbilitySystem.Attributes.Components {
    [Serializable]
    public struct AttributesOwnerComponent : IComponentData {
        public Entity Value;

        public static implicit operator Entity(AttributesOwnerComponent e) { return e.Value; }
        public static implicit operator AttributesOwnerComponent(Entity e) { return new AttributesOwnerComponent { Value = e }; }
    }
}