using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;

namespace GameplayAbilitySystem.GameplayEffects {
    public class GameplayEffectRequireIgnoreTagContainer : GameplayEffectTagContainer, IRequireIgnoreTags {
        private List<GameplayTag> _requirePresence = new List<GameplayTag>();
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