using System;
using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects
{
    [Serializable]
    public class GameplayEffectTagContainer : IAddedRemovedTags
    {
        [SerializeField]
        List<GameplayTag> _added = new List<GameplayTag>();


        public List<GameplayTag> Added => _added;

    }
}