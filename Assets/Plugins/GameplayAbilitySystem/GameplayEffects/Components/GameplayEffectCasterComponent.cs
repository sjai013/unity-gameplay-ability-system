using Unity.Entities;

namespace GameplayAbilitySystem.GameplayEffects.Components {
    public struct GameplayEffectCasterComponent : IComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectCasterComponent e) { return e.Value; }
        public static implicit operator GameplayEffectCasterComponent(Entity e) { return new GameplayEffectCasterComponent { Value = e }; }
    }
 
}


