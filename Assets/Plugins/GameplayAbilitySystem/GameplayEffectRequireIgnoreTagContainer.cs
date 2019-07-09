using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public class GameplayEffectRequireIgnoreTagContainer : GameplayEffectTagContainer, IRequireIgnoreTags {
        [SerializeField]
        private List<GameplayTag> _requirePresence = new List<GameplayTag>();
        
        [SerializeField]
        private List<GameplayTag> _requireAbsence = new List<GameplayTag>();

        public List<GameplayTag> RequirePresence => _requirePresence;

        public List<GameplayTag> RequireAbsence => _requireAbsence;

        public override bool HasAll(IEnumerable<GameplayTag> Tags) {
            throw new System.NotImplementedException();
        }

        public override bool HasAny(IEnumerable<GameplayTag> Tags) {
            throw new System.NotImplementedException();
        }
    }
}