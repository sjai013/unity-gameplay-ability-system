using System;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayCues {
    public class GameplayCueParameters {
        public float NormalisedMagnitude = 0f;
        public float RawMagnitude = 0f;
        public Vector3 Location = Vector3.zero;
        public Vector3 Normal = Vector3.zero;
        public int GameplayEffectLevel = 1;
        public int AbilityLevel = 1;

        public GameplayCueParameters(IGameplayAbilitySystem Instigator, GameObject EffectCauser, GameObject SourceObject) {
            this.Instigator = Instigator;
            this.EffectCauser = EffectCauser;
            this.SourceObject = SourceObject;
        }

        /// <summary>
        /// Returns the component that instigated the gameplay cue
        /// </summary>
        /// <returns>Instigating component</returns>
        public IGameplayAbilitySystem Instigator { get; }

        /// <summary>
        /// The actual component that caused the effect, e.g. projectile
        /// 
        /// </summary>
        /// <value></value>
        public GameObject EffectCauser { get; }

        /// <summary>
        /// The object that created the effect
        /// </summary>
        /// <value></value>
        public GameObject SourceObject { get; }
    }

    /// <summary>
    /// GameplayCue Event Types.
    /// <para></para>
    /// <para>WhileActive/OnActive is called for Infinite effects</para>
    /// <para></para>
    /// <para>Executed is called for Instant effects/on each tick</para>
    /// <para></para>
    /// <para>WhileActive/OnActive/Removed is called for Duration effects</para>
    /// </summary>
    public enum EGameplayCueEventTypes {
        OnActive, // Called when GameplayCue is first activated
        WhileActive, // Called *while* GameplayCue is active
        Executed, // Called when a GameplayCue is executed (e.g. instant/periodic/tick)
        Removed // Called when a GameplayCue is removed
    }

}
