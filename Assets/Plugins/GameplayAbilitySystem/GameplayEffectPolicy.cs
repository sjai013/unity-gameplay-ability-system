using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects
{
    [Serializable]
    public class GameplayEffectPolicy : IGameplayEffectPolicy
    {
        [SerializeField]
        private EDurationPolicy _durationPolicy;

        [SerializeField]
        private float _durationMagnitude;

        [SerializeField]
        private List<GameplayEffectModifier> _modifiers = new List<GameplayEffectModifier>();

        public EDurationPolicy DurationPolicy => _durationPolicy;
        public float DurationMagnitude => _durationMagnitude;
        public List<GameplayEffectModifier> Modifiers => _modifiers;
    }

}
