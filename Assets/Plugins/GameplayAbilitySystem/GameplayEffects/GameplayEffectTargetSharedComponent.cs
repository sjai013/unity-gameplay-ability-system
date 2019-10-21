using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public struct GameplayEffectTargetSharedComponent : ISharedComponentData {
        Entity Value;
    }


    public struct GameplayEffectTag : IComponentData {
        public int Value;
        public static implicit operator int(GameplayEffectTag e) { return e.Value; }
        public static implicit operator GameplayEffectTag(int e) { return new GameplayEffectTag { Value = e }; }
    }


    public struct GameplayEffectTargetSCD : ISharedComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectTargetSCD e) { return e.Value; }
        public static implicit operator GameplayEffectTargetSCD(Entity e) { return new GameplayEffectTargetSCD { Value = e }; }
    }

    public struct GameplayEffectCaster : IComponentData {
        public Entity Value;
        public static implicit operator Entity(GameplayEffectCaster e) { return e.Value; }
        public static implicit operator GameplayEffectCaster(Entity e) { return new GameplayEffectCaster { Value = e }; }
    }
 
}


