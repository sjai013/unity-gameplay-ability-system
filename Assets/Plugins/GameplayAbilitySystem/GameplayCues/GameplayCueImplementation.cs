using System;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayCues {
    public abstract class AbstractGameplayCueImplementation : ScriptableObject {
        public abstract void HandleGameplayCue(GameObject Target, EGameplayCueEventTypes EventType, GameplayCueParameters Parameters);
    }
}
