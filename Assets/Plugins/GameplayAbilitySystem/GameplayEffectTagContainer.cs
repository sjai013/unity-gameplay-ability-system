using System.Collections.Generic;

namespace GameplayAbilitySystem.GameplayEffects {
    public abstract class GameplayEffectTagContainer {
        public abstract bool HasAny(IEnumerable<GameplayTag> Tags);
        public abstract bool HasAll(IEnumerable<GameplayTag> Tags);
    }
}