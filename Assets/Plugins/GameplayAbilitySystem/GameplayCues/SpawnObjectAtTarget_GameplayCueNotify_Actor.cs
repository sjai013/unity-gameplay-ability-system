using System;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayCues
{
    public class SpawnObjectAtTarget_GameplayCueNotify_Actor : AbstractGameplayCueNotify_Actor
    {
        public GameObject ObjectToSpawn;
        public AbstractGameplayCueImplementation GameplayCueImplementation;

        public override void Execute(GameObject Target, EGameplayCueEventTypes EventType, GameplayCueParameters Parameters)
        {
            GameplayCueImplementation.HandleGameplayCue(Target, EventType, Parameters);
        }
    }
}
