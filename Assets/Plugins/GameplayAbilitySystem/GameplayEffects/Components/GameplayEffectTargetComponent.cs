using System;
using Unity.Entities;
namespace GameplayAbilitySystem.GameplayEffects.Components {
    [Serializable]
    public struct GameplayEffectTargetComponent : IComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectTargetComponent e) { return e.Value; }
        public static implicit operator GameplayEffectTargetComponent(Entity e) { return new GameplayEffectTargetComponent { Value = e }; }
    }
}