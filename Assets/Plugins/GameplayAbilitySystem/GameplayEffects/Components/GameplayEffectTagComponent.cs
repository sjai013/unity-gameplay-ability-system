using Unity.Entities;

namespace GameplayAbilitySystem.GameplayEffects.Components {
    public struct GameplayEffectTagComponent : IComponentData {
        public int Value;
        public static implicit operator int(GameplayEffectTagComponent e) { return e.Value; }
        public static implicit operator GameplayEffectTagComponent(int e) { return new GameplayEffectTagComponent { Value = e }; }
    }

}


