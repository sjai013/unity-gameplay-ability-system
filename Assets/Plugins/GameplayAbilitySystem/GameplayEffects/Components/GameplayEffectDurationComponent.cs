using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
namespace GameplayAbilitySystem.GameplayEffects.Components {

    [Serializable]
    public struct GameplayEffectDurationComponent : IComponentData {
        public float WorldStartTime;
        public float RemainingTime;
        public float NominalDuration;

        public static GameplayEffectDurationComponent Initialise(float duration, float startWorldTime) {
            return new GameplayEffectDurationComponent
            {
                NominalDuration = duration,
                RemainingTime = duration,
                WorldStartTime = startWorldTime
            };
        }
    }
}