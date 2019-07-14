using GameplayAbilitySystem;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.GameplayCues;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.Statics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects {

    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Gameplay Effect")]
    public class GameplayEffect : ScriptableObject {

        [SerializeField]
        private GameplayEffectPolicy _gameplayEffectPolicy = new GameplayEffectPolicy();

        [SerializeField]
        private GameplayEffectTags _gameplayEffectTags = new GameplayEffectTags();

        public EffectPeriodicity Period;

        [SerializeField]
        public List<GameplayCue> GameplayCues = new List<GameplayCue>();

        public StackingPolicy StackingPolicy = new StackingPolicy();
        public GameplayEffectTags GameplayEffectTags { get => _gameplayEffectTags; }
        public GameplayEffectPolicy GameplayEffectPolicy { get => _gameplayEffectPolicy; }

        public IEnumerable<(GameplayTag Tag, GameplayEffect Effect)> GrantedEffectTags => GrantedTags.Select(x => (x, this));

        public bool ApplicationTagRequirementMet(IGameplayAbilitySystem AbilitySystem) {
            var requiredTagsPresent = true;
            var ignoredTagsAbsent = true;

            if (GameplayEffectTags.ApplicationTagRequirements.RequirePresence.Count > 0) {
                requiredTagsPresent = AbilitySystem.ActiveTags.Any(x => GameplayEffectTags.ApplicationTagRequirements.RequirePresence.Contains(x));
            }

            if (GameplayEffectTags.ApplicationTagRequirements.RequireAbsence.Count > 0) {
                ignoredTagsAbsent = !AbilitySystem.ActiveTags.Any(x => GameplayEffectTags.ApplicationTagRequirements.RequireAbsence.Contains(x));
            }

            return requiredTagsPresent && ignoredTagsAbsent;
        }

        public List<GameplayTag> GetOwningTags() {
            var tags = new List<GameplayTag>(_gameplayEffectTags.GrantedTags.Added.Count
                                            + _gameplayEffectTags.AssetTags.Added.Count);

            tags.AddRange(_gameplayEffectTags.GrantedTags.Added);
            tags.AddRange(_gameplayEffectTags.AssetTags.Added);

            return tags;
        }

        public List<GameplayTag> GrantedTags => _gameplayEffectTags.GrantedTags.Added;

        public bool ApplicationRequirementsPass(AbilitySystemComponent AbilitySystem) {
            // return _gameplayEffectTags.ApplicationTagRequirements.RequirePresence;

            return true;
        }

        public Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> CalculateModifierEffect(Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> Existing = null) {
            Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> modifierTotals;
            if (Existing == null) {
                modifierTotals = new Dictionary<AttributeType, Dictionary<EModifierOperationType, float>>();
            }
            else {
                modifierTotals = Existing;
            }

            foreach (var modifier in GameplayEffectPolicy.Modifiers) {
                if (!modifierTotals.TryGetValue(modifier.Attribute, out var modifierType)) {
                    // This attribute hasn't been recorded before, so create a blank new record
                    modifierType = new Dictionary<EModifierOperationType, float>();
                    modifierTotals.Add(modifier.Attribute, modifierType);
                }

                if (!modifierType.TryGetValue(modifier.ModifierOperation, out var value)) {
                    value = 0;
                    switch (modifier.ModifierOperation) {
                        case EModifierOperationType.Multiply:
                            value = 1;
                            break;

                        case EModifierOperationType.Divide:
                            value = 1;
                            break;

                        default:
                            value = 0;
                            break;
                    }
                    modifierType.Add(modifier.ModifierOperation, value);
                }

                switch (modifier.ModifierOperation) {
                    case EModifierOperationType.Add:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] += modifier.ScaledMagnitude;
                        break;

                    case EModifierOperationType.Multiply:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                        break;

                    case EModifierOperationType.Divide:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                        break;
                }
            }

            return modifierTotals;
        }

        public Dictionary<AttributeType, AttributeModificationValues> CalculateAttributeModification(IGameplayAbilitySystem AbilitySystem, Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> Modifiers, bool operateOnCurrentValue = false) {
            var attributeModification = new Dictionary<AttributeType, AttributeModificationValues>();

            foreach (var attribute in Modifiers) {
                if (!attribute.Value.TryGetValue(EModifierOperationType.Add, out var addition)) {
                    addition = 0;
                }

                if (!attribute.Value.TryGetValue(EModifierOperationType.Multiply, out var multiplication)) {
                    multiplication = 1;
                }

                if (!attribute.Value.TryGetValue(EModifierOperationType.Divide, out var division)) {
                    division = 1;
                }

                var oldAttributeValue = 0f;
                if (!operateOnCurrentValue) {
                    oldAttributeValue = AbilitySystem.GetNumericAttributeBase(attribute.Key);
                }
                else {
                    oldAttributeValue = AbilitySystem.GetNumericAttributeCurrent(attribute.Key);
                }

                var newAttributeValue = (oldAttributeValue + addition) * (multiplication / division);

                if (!attributeModification.TryGetValue(attribute.Key, out var values)) {
                    values = new AttributeModificationValues();
                    attributeModification.Add(attribute.Key, values);
                }

                values.NewAttribueValue += newAttributeValue;
                values.OldAttributeValue += oldAttributeValue;
            }

            return attributeModification;
        }

        public void ApplyInstantEffect(IGameplayAbilitySystem Target) {
            // Modify base attribute values.  Collect the overall change for each modifier
            var modifierTotals = CalculateModifierEffect();
            var attributeModifications = CalculateAttributeModification(Target, modifierTotals);

            // Finally, For each attribute, apply the new modified values
            foreach (var attribute in attributeModifications) {
                Target.SetNumericAttributeBase(attribute.Key, attribute.Value.NewAttribueValue);

                // mark the corresponding aggregator as dirty so we can recalculate the current values
                var aggregators = Target.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.GetAggregatorsForAttribute(attribute.Key);
                // Target.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.Select(x => x.Value[attribute.Key]).AttributeAggregatorMap.TryGetValue(attribute.Key, out var aggregator);
                if (aggregators.Count() != 0) {
                    Target.ActiveGameplayEffectsContainer.UpdateAttribute(aggregators, attribute.Key);
                }
                else {
                    // No aggregators, so set current value = base value
                    Target.SetNumericAttributeCurrent(attribute.Key, Target.GetNumericAttributeBase(attribute.Key));
                }
            }
        }
    }
}