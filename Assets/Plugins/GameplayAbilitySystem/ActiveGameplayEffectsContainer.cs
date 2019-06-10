using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.Attributes;
using System;
using UnityEngine.Events;

namespace GameplayAbilitySystem.GameplayEffects
{
    [Serializable]
    public class ActiveGameplayEffectsContainer
    {
        private IGameplayAbilitySystem AbilitySystem;
        public ActiveGameplayEffectsContainer(IGameplayAbilitySystem AbilitySystem)
        {
            this.AbilitySystem = AbilitySystem;
        }

        /// <summary>
        /// This is used to keep track of all the "temporary" attribute modifiers,
        /// so we can calculate them all as f(Base, Added, Multiplied, Divided) = (Base + Added) * (Multiplied/Divided)
        /// </summary>
        /// <value></value>
        public Dictionary<AttributeType, Aggregator> AttributeAggregatorMap { get; } = new Dictionary<AttributeType, Aggregator>();

        public List<ActiveGameplayEffectData> ActiveGameplayEffects = new List<ActiveGameplayEffectData>();

        public ActiveGameplayEffectsEvent ActiveGameplayEffectAdded = new ActiveGameplayEffectsEvent();

        public ActiveGameplayEffectData ApplyGameEffect(ActiveGameplayEffectData EffectData)
        {
            // Durational effect.  Add granted modifiers to active list
            AddActiveGameplayEffectGrantedTagsAndModifiers(EffectData);
            // Register callbacks for removal of effects when duration expires
            ActiveGameplayEffectAdded?.Invoke(AbilitySystem, EffectData);
            return null;
        }

        public ActiveGameplayEffectData ApplyCooldownEffect(ActiveGameplayEffectData EffectData)
        {
            return null;
        }

        private void OnActiveGameplayEffectAdded(ActiveGameplayEffectData EffectData)
        {
            ActiveGameplayEffectAdded?.Invoke(AbilitySystem, EffectData);
        }

        private void AddActiveGameplayEffectGrantedTagsAndModifiers(ActiveGameplayEffectData EffectData)
        {
            foreach (var modifier in EffectData.Effect.GameplayEffectPolicy.Modifiers)
            {
                modifier.AttemptCalculateMagnitude(out var EvaluatedMagnitude);

                var attributeAggregatorMap = AbilitySystem.ActiveGameplayEffectsContainer.AttributeAggregatorMap;
                // If aggregator for this attribute doesn't exist, add it.
                if (!attributeAggregatorMap.TryGetValue(modifier.Attribute, out var aggregator))
                {
                    aggregator = new Aggregator(modifier.Attribute);
                    aggregator.Dirtied.AddListener(UpdateAttribute);
                    attributeAggregatorMap.Add(modifier.Attribute, aggregator);
                }
                aggregator.AddAggregatorMod(EvaluatedMagnitude, modifier.ModifierOperation, EffectData.Effect);
                aggregator.MarkDirty();
            }
        }

        private void UpdateAttribute(Aggregator Aggregator, AttributeType AttributeType)
        {
            var baseAttributeValue = AbilitySystem.GetNumericAttributeBase(AttributeType);
            var newCurrentAttributeValue = Aggregator.Evaluate(baseAttributeValue);
            AbilitySystem.SetNumericAttributeCurrent(AttributeType, newCurrentAttributeValue);

        }

    }
    public class ActiveGameplayEffectsEvent : UnityEvent<IGameplayAbilitySystem, ActiveGameplayEffectData>
    {

    }
}

