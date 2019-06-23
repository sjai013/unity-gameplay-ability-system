using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Statics;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitySystem {
    /// <inheritdoc />
    [Serializable]
    public class GameplayEffectModifier : IGameplayEffectModifier {
        [SerializeField]
        private AttributeType _attributeType = null;

        [SerializeField]
        private EModifierOperationType _modifierOperationType = default;
        [Space(10)]

        [SerializeField]
        private EMagnitudeCalculationTypes _magnitudeCalculationType = default;

        [SerializeField]
        private float _scaledMagnitude = 0f;
        [Space(10)]

        [SerializeField]
        private GameplayEffectModifierTagCollection _sourceTags = null;

        [SerializeField]
        private GameplayEffectModifierTagCollection _targetTags = null;

        /// <inheritdoc />
        public AttributeType Attribute => _attributeType;
        /// <inheritdoc />
        public EModifierOperationType ModifierOperation => _modifierOperationType;
        /// <inheritdoc />
        public float ScaledMagnitude => _scaledMagnitude;
        /// <inheritdoc />
        public EMagnitudeCalculationTypes MagnitudeCalculationType => _magnitudeCalculationType;
        /// <inheritdoc />
        public GameplayEffectModifierTagCollection SourceTags => _sourceTags;
        /// <inheritdoc />
        public GameplayEffectModifierTagCollection TargetTags => _targetTags;

        /// <inheritdoc />
        public bool AttemptCalculateMagnitude(out float EvaluatedMagnitude) {
            //TODO: PROPER IMPLEMENTATION
            EvaluatedMagnitude = this.ScaledMagnitude;
            return true;
        }

        public GameplayEffectModifier InitialiseEmpty() {
            this._attributeType = null;
            return this;
        }

    }
}
