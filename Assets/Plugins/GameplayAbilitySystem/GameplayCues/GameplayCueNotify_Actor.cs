using System;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayCues {
    public abstract class AbstractGameplayCueNotify_Actor {
        public abstract void Execute(GameObject Target, EGameplayCueEventTypes EventType, GameplayCueParameters Parameters);
    }
}
