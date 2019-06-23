using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitySystem {
    /// <summary>
    /// Gameplay tags are used to define how various aspects interact with each other
    /// </summary>
    [CreateAssetMenu(fileName = "Gameplay Tag", menuName = "Ability System/Gameplay Tag")]
    [Serializable]
    public class GameplayTag : ScriptableObject {
        /// <summary>
        /// A static container for keeping track of all <see cref="GameplayTag"/> that are used.
        /// </summary>
        /// <typeparam name="GameplayTag"></typeparam>
        /// <returns></returns>
        public static HashSet<GameplayTag> GameplayTags = new HashSet<GameplayTag>();

        /// <summary>
        /// A developer friendly comment
        /// </summary>
        public string Comment;

        void OnEnable() {
            /// <summary>
            /// When this <see cref="ScriptableObject"/> is initialised, add the instance to the static container <see cref="GameplayTags"/>
            /// </summary>
            /// <returns></returns>
            if (!GameplayTags.Contains(this)) {
                GameplayTags.Add(this);
            }
        }

        void OnDisable() {
            /// <summary>
            /// When this <see cref="ScriptableObject"/> is destroyed, remove the instance from the static container <see cref="GameplayTags"/>
            /// </summary>
            /// <returns></returns>
            GameplayTags.Remove(this);
        }

    }
}
