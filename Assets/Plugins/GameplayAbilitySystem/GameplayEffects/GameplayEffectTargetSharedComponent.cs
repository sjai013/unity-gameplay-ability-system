using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public struct GameplayEffectTargetSharedComponent : ISharedComponentData {
        Entity Value;
    }
}
