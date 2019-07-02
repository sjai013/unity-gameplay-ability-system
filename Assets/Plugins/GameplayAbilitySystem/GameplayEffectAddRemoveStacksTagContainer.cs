using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public class GameplayEffectAddRemoveStacksTagContainer : GameplayEffectTagContainer {
        [SerializeField]
        List<GameplayTagStackComposite> _added = new List<GameplayTagStackComposite>();

        public List<GameplayTagStackComposite> Added => _added;


        public override bool HasAny(IEnumerable<GameplayTag> Tags) {
            return _added.Where(x => !Tags.Any(y => x.Tag == y)).Any();
        }

        public override bool HasAll(IEnumerable<GameplayTag> Tags) {
            var addedTags = _added.Select(x => x.Tag);
            return !Tags.Except(addedTags).Any();
        }

    }

    [Serializable]
    public class GameplayTagStackComposite {
        [Tooltip("GameplayEffects with this tag will be candidates for removal")]
        public GameplayTag Tag;
        
        [Tooltip("Number of stacks of each GameEffect to remove.  0 means remove all stacks.")]
        public int StacksToRemove = 0;
    }
}