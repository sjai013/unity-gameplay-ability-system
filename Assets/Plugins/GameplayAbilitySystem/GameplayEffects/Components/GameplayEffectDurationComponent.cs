using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
namespace GameplayAbilitySystem.GameplayEffects.Components {

    [Serializable]
    public struct GameplayEffectDurationComponent : IComponentData {
        public float WorldStartTime;
        public float RemainingTime;
    }
}