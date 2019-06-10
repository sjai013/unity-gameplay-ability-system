using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.Statics;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Enums;
using UnityEngine;
using GameplayAbilitySystem.GameplayCues;

namespace GameplayAbilitySystem.GameplayEffects
{
    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Gameplay Effect")]
    public class GameplayEffect : ScriptableObject
    {
        [SerializeField]
        GameplayEffectPolicy _gameplayEffectPolicy = new GameplayEffectPolicy();

        [SerializeField]
        GameplayEffectTags _gameplayEffectTags = new GameplayEffectTags();

        [SerializeField]
        List<AbstractGameplayCueImplementation> GameplayCues = new List<AbstractGameplayCueImplementation>();

        public GameplayEffectTags GameplayEffectTags { get => _gameplayEffectTags; }
        public GameplayEffectPolicy GameplayEffectPolicy { get => _gameplayEffectPolicy; }

        public bool ExecuteEffect(IGameplayAbilitySystem TargetAbilitySystem)
        {
            bool allExecuted = false;
            var attributeSet = TargetAbilitySystem.GetActor().GetComponent<AttributeSet>();
            for (var i = 0; i < this._gameplayEffectPolicy.Modifiers.Count; i++)
            {
                // TODO: Stacking logic, including effect refreshing on stack application etc.

                // TODO: Modify current attribute value

                var modifier = this._gameplayEffectPolicy.Modifiers[i];
                var attribute = attributeSet.Attributes.Find(x => x.AttributeType == modifier.Attribute);

                if (attribute == null) continue;

                var evalData = new GameplayModifierEvaluatedData()
                {
                    Attribute = attribute,
                    ModOperation = modifier.ModifierOperation,
                    Magnitude = GetModifierMagnitude(modifier)
                };

                // TODO: We should probably gather all attribute changes (base and current) into a separate structure
                // and apply all at once
                allExecuted |= _ExecuteModification(TargetAbilitySystem, attributeSet, modifier, evalData);
            }

            // TODO: These are applied regardless of whether the effect was applied or not
            // and regardless of whether we are "refreshing" a stack.

            // // Apply the persisted attribute modifiers to the current value
            // if (TargetAbilitySystem.PersistedAttributeModifiers.ContainsKey(this))
            // {
            //     var effectPersistedModifiers = TargetAbilitySystem.PersistedAttributeModifiers[this];

            //     if (effectPersistedModifiers != null)
            //     {
            //         for (int i = 0; i < effectPersistedModifiers.Count; i++)
            //         {
            //             TargetAbilitySystem.AdjustNumericAttributeCurrent(effectPersistedModifiers[i].AttributeType, effectPersistedModifiers[i].Modifier);
            //         }
            //     }
            // }


            // Apply gamecues
            for (var i = 0; i < GameplayCues.Count; i++)
            {
                var cue = GameplayCues[i];
                cue.HandleGameplayCue(TargetAbilitySystem.GetActor().gameObject, EGameplayCueEventTypes.Executed, new GameplayCueParameters(null, null, null));
            }

            return allExecuted;
        }

        public bool EffectExpired(float durationElapsed)
        {
            var effectExpired = false;
            switch (this.GameplayEffectPolicy.DurationPolicy)
            {
                case EDurationPolicy.Infinite:
                    effectExpired = false;
                    break;
                case EDurationPolicy.HasDuration:
                    if (durationElapsed >= this.GameplayEffectPolicy.DurationMagnitude)
                    {
                        effectExpired = true;
                    }
                    break;
                case EDurationPolicy.Instant:
                    effectExpired = true;
                    break;
            }

            return effectExpired;
        }

        public List<GameplayTag> GetOwningTags()
        {
            var tags = new List<GameplayTag>(_gameplayEffectTags.GrantedTags.Added.Count
                                            + _gameplayEffectTags.AssetTags.Added.Count);

            tags.AddRange(_gameplayEffectTags.GrantedTags.Added);
            tags.AddRange(_gameplayEffectTags.AssetTags.Added);

            return tags;

        }

        private bool _ExecuteModification(IGameplayAbilitySystem TargetAbilitySystem, IAttributeSet AttributeSet, GameplayEffectModifier Modifier, GameplayModifierEvaluatedData EvalData)
        {
            var executed = false;
            float oldAttributeValue = EvalData.Attribute.BaseValue;

            // If this is an instant attribute, we do things slightly different than if it's not
            // PreGameplayEffectExecute is only called if this is an instant effect
            if (this.GameplayEffectPolicy.DurationPolicy == EDurationPolicy.Instant)
            {
                // Apply changes to base value immediately
                if (!AttributeSet.PreGameplayEffectExecute(this, EvalData)) return executed;
                ApplyModificationToAttributeBase(TargetAbilitySystem, AttributeSet, Modifier, EvalData);
                AttributeSet.PostGameplayEffectExecute(this, EvalData);
            }
            else
            {



            }

            executed = true;
            return executed;

        }

        private float ApplyModificationToAttributeBase(IGameplayAbilitySystem TargetAbilitySystem, IAttributeSet AttributeSet, GameplayEffectModifier Modifier, GameplayModifierEvaluatedData EvalData)
        {
            var currentBase = EvalData.Attribute.BaseValue;
            var newBase = currentBase + AbilitySystemStatics.CalculateModificationValue(currentBase, Modifier.ModifierOperation, Modifier.ScaledMagnitude);
            SetAttributeBaseValue(TargetAbilitySystem, AttributeSet, EvalData.Attribute, Modifier, newBase);
            return newBase;
        }


        private float SetAttributeBaseValue(IGameplayAbilitySystem TargetAbilitySystem, IAttributeSet AttributeSet, IAttribute Attribute, GameplayEffectModifier Modifier, float NewBaseValue)
        {
            AttributeSet.PreAttributeBaseChange(Attribute, ref NewBaseValue);
            float currentBase = Attribute.BaseValue;

            if (currentBase != NewBaseValue)
            {
                var AttributeChangeData = new AttributeChangeData()
                {
                    NewValue = NewBaseValue,
                    OldValue = currentBase,
                    Modifier = Modifier,
                    Effect = this,
                    Target = TargetAbilitySystem
                };
                Attribute.BaseValue = NewBaseValue;
                AttributeSet.AttributeBaseValueChanged?.Invoke(AttributeChangeData);

                if (TargetAbilitySystem.ActiveGameplayEffectsContainer.AttributeAggregatorMap.TryGetValue(Attribute.AttributeType, out var Aggregator))
                {
                    Aggregator.MarkDirty();
                }
            }
            return NewBaseValue;
        }

        private void SetAttributeCurrentValue(IGameplayAbilitySystem TargetAbilitySystem, IAttributeSet AttributeSet, IAttribute Attribute, GameplayEffectModifier Modifier, float NewValue)
        {
            AttributeSet.PreAttributeChange(Attribute, ref NewValue);
            float currentValue = Attribute.CurrentValue;

            if (currentValue != NewValue)
            {
                var AttributeChangeData = new AttributeChangeData()
                {
                    NewValue = NewValue,
                    OldValue = currentValue,
                    Modifier = Modifier,
                    Effect = this,
                    Target = TargetAbilitySystem
                };
                Attribute.BaseValue = NewValue;
                AttributeSet.AttributeCurrentValueChanged?.Invoke(AttributeChangeData);
            }

            Attribute.SetAttributeCurrentValue(AttributeSet, ref NewValue);
        }

        public float GetModifierMagnitude(GameplayEffectModifier modifier)
        {
            modifier.AttemptCalculateMagnitude(out var evaluatedMagnitude);
            return evaluatedMagnitude;
        }

        /// <summary> Calculate the magnitudes of all modifiers on this <see cref="GameplayEffec"/> </summary>
        /// <returns>  Return List of Attribute-Magnitude tuples  </returns>
        public List<(AttributeType Attribute, float Magnitude)> CalculateModifierMagnitudes()
        {
            var modifierList = new List<(AttributeType Attribute, float Magnitude)>();
            foreach (var modifier in this.GameplayEffectPolicy.Modifiers)
            {
                float magnitude = 0;
                modifier.AttemptCalculateMagnitude(out magnitude);
                modifierList.Add((modifier.Attribute, magnitude));
            }

            return modifierList;
        }

    }

    public struct GameplayModifierEvaluatedData
    {
        public IAttribute Attribute;
        public EModifierOperationType ModOperation;
        public float Magnitude;

    }
}

