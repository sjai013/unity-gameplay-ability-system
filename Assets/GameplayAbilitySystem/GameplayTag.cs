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
        /// A developer friendly comment
        /// </summary>
        public string Comment;

        [SerializeField]
        private int UniqueId;

        public void SetUniqueId(int id) {
            this.UniqueId = id;
        }

        public override int GetHashCode() {
            return this.UniqueId;
        }

    }
}
