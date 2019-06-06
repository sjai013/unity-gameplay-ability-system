using System.Linq;
using System;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.GameplayEffects;
using UnityEngine;
using System.Collections.Generic;

namespace GameplayAbilitySystem
{
    /// <inheritdoc />
    [Serializable]
    public class GameplayCooldown : IGameplayCooldown
    {
        /// <inheritdoc />
        public GameplayEffect CooldownGameplayEffect => _cooldownGameplayEffect;

        /// <inheritdoc />
        [SerializeField]
        private GameplayEffect _cooldownGameplayEffect;

        /// <inheritdoc />
        public IEnumerable<GameplayTag> GetCooldownTags()
        {
            return _cooldownGameplayEffect.GameplayEffectTags.GrantedTags.Added;
        }
    }
}
