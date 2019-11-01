using Unity.Entities;

namespace GameplayAbilitySystem.GameplayEffects.Components {
    public struct GameplayEffectTargetSCDComponent : ISharedComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectTargetSCDComponent e) { return e.Value; }
        public static implicit operator GameplayEffectTargetSCDComponent(Entity e) { return new GameplayEffectTargetSCDComponent { Value = e }; }
    }
 
}


