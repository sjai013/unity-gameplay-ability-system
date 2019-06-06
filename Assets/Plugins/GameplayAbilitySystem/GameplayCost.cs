using System;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.GameplayEffects;
using UnityEngine;

namespace GameplayAbilitySystem
{
    /// <inheritdoc />
    [Serializable]
    public class GameplayCost : IGameplayCost
    {
        /// <inheritdoc />
        public GameplayEffect CostGameplayEffect => _costGameplayEffect;

        /// <inheritdoc />
        [SerializeField]
        private GameplayEffect _costGameplayEffect;
    }
}
