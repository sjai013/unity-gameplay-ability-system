using System;
using Unity.Entities;
namespace GameplayAbilitySystem.GameplayEffects.Components {
    [Serializable]
    public struct GameplayEffectAttributeEntityComponent : IComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectAttributeEntityComponent e) { return e.Value; }
        public static implicit operator GameplayEffectAttributeEntityComponent(Entity e) { return new GameplayEffectAttributeEntityComponent { Value = e }; }
    }
}