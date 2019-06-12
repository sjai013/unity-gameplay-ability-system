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
        public List<AbstractGameplayCueImplementation> GameplayCues = new List<AbstractGameplayCueImplementation>();

        public GameplayEffectTags GameplayEffectTags { get => _gameplayEffectTags; }
        public GameplayEffectPolicy GameplayEffectPolicy { get => _gameplayEffectPolicy; }

        public void ExecuteEffect(IGameplayAbilitySystem TargetAbilitySystem)
        {

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

