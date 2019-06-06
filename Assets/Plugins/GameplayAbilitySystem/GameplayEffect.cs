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
                var modifier = this._gameplayEffectPolicy.Modifiers[i];
                var attribute = attributeSet.Attributes.Find(x => x.AttributeType == modifier.Attribute);

                if (attribute == null) continue;

                var evalData = new GameplayModifierEvaluatedData()
                {
                    Attribute = attribute,
                    ModOperation = modifier.ModifierOperation,
                    Magnitude = CalculateModifierMagnitude(modifier)
                };

                allExecuted |= _ExecuteModification(TargetAbilitySystem, attributeSet, modifier, evalData);
            }

            // Apply gamecues
            for (var i = 0; i < GameplayCues.Count; i++)
            {
                var cue = GameplayCues[i];
                cue.HandleGameplayCue(TargetAbilitySystem.GetActor().gameObject, EGameplayCueEventTypes.Executed, new GameplayCueParameters(null, null, null));
            }

            return allExecuted;
        }

        public bool EffectExpired(float CooldownTimeElapsed)
        {
            var effectExpired = false;
            switch (this.GameplayEffectPolicy.DurationPolicy)
            {
                case EDurationPolicy.Infinite:
                    effectExpired = false;
                    break;
                case EDurationPolicy.HasDuration:
                    if (CooldownTimeElapsed >= this.GameplayEffectPolicy.DurationMagnitude)
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
            if (!AttributeSet.PreGameplayEffectExecute(this, EvalData)) return executed;
            float oldAttributeValue = EvalData.Attribute.BaseValue;
            ApplyModificationToAttribute(TargetAbilitySystem, AttributeSet, Modifier, EvalData);
            AttributeSet.PostGameplayEffectExecute(this, EvalData);
            executed = true;
            return executed;

        }

        private void ApplyModificationToAttribute(IGameplayAbilitySystem TargetAbilitySystem, IAttributeSet AttributeSet, GameplayEffectModifier Modifier, GameplayModifierEvaluatedData EvalData)
        {
            var currentBase = EvalData.Attribute.BaseValue;
            var newBase = AbilitySystemStatics.CalculateModifiedBaseAttribute(currentBase, Modifier.ModifierOperation, Modifier.ScaledMagnitude);
            SetAttributeBaseValue(TargetAbilitySystem, AttributeSet, EvalData.Attribute, Modifier, newBase);

        }

        private void SetAttributeBaseValue(IGameplayAbilitySystem TargetAbilitySystem, IAttributeSet AttributeSet, IAttribute Attribute, GameplayEffectModifier Modifier, float NewBaseValue)
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
                AttributeSet.AttributeValueChanged?.Invoke(AttributeChangeData);

                Attribute.SetNumericValueChecked(AttributeSet, ref NewBaseValue);
            }
        }

        private float CalculateModifierMagnitude(GameplayEffectModifier modifier)
        {
            return modifier.ScaledMagnitude;
        }

    }
    public struct GameplayModifierEvaluatedData
    {
        public IAttribute Attribute;
        public EModifierOperationType ModOperation;
        public float Magnitude;

    }
}

