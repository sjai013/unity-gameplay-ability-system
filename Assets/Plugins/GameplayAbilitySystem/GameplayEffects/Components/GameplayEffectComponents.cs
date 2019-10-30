using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
namespace GameplayAbilitySystem.GameplayEffects.Systems {

    [Serializable]
    public struct GameplayEffectTargetComponent : IComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectTargetComponent e) { return e.Value; }
        public static implicit operator GameplayEffectTargetComponent(Entity e) { return new GameplayEffectTargetComponent { Value = e }; }
    }

    [Serializable]
    public struct GameplayEffectSourceComponent : IComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectSourceComponent e) { return e.Value; }
        public static implicit operator GameplayEffectSourceComponent(Entity e) { return new GameplayEffectSourceComponent { Value = e }; }
    }

    public interface IGameplayEffectTag { }

    [Serializable]
    public struct GameplayEffectAttributeEntity : IComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectAttributeEntity e) { return e.Value; }
        public static implicit operator GameplayEffectAttributeEntity(Entity e) { return new GameplayEffectAttributeEntity { Value = e }; }
    }

    [Serializable]
    public struct GameplayEffectDurationComponent : IComponentData {
        public float WorldStartTime;
        public float RemainingTime;
    }
}