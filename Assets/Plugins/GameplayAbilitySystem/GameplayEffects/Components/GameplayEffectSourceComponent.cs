using System;
using Unity.Entities;
namespace GameplayAbilitySystem.GameplayEffects.Components {
    [Serializable]
    public struct GameplayEffectSourceComponent : IComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectSourceComponent e) { return e.Value; }
        public static implicit operator GameplayEffectSourceComponent(Entity e) { return new GameplayEffectSourceComponent { Value = e }; }
    }
}