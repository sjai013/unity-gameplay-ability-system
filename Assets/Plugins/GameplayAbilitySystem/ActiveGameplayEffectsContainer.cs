using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.Attributes;
using System;
using UnityEngine.Events;
using UniRx.Async;
using System.Threading.Tasks;
using UnityEngine;

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

        [SerializeField]
        private List<ActiveGameplayEffectData> _activeCooldowns = new List<ActiveGameplayEffectData>();
        public List<ActiveGameplayEffectData> ActiveCooldowns { get => _activeCooldowns; }

        public List<ActiveGameplayEffectData> ActiveGameplayEffects = new List<ActiveGameplayEffectData>();

        
        public ActiveGameplayEffectsEvent ActiveGameplayEffectAdded = new ActiveGameplayEffectsEvent();

        public async Task<ActiveGameplayEffectData> ApplyGameEffect(ActiveGameplayEffectData EffectData)
        {
            // Durational effect.  Add granted modifiers to active list
            AddActiveGameplayEffect(EffectData);
            ActiveGameplayEffectAdded?.Invoke(AbilitySystem, EffectData);

            // Register callbacks for removal of effects when duration expires
            await UniTask.Delay((int)(EffectData.Effect.GameplayEffectPolicy.DurationMagnitude * 1000.0f));
            RemoveActiveGameplayEffect(EffectData);

            return null;
        }

        public async void ApplyCooldownEffect(ActiveGameplayEffectData EffectData)
        {
            this.ActiveCooldowns.Add(EffectData);
            await UniTask.Delay((int)(EffectData.Effect.GameplayEffectPolicy.DurationMagnitude * 1000.0f));
            this.ActiveCooldowns.Remove(EffectData);
        }

        public bool IsCooldownEffectPresent(GameplayEffect Effect)
        {
            return (this.ActiveCooldowns.Any(x => x.Effect == Effect));
        }

        private void OnActiveGameplayEffectAdded(ActiveGameplayEffectData EffectData)
        {
            ActiveGameplayEffectAdded?.Invoke(AbilitySystem, EffectData);
        }

        private void ModifyActiveGameplayEffect(ActiveGameplayEffectData EffectData, Action<GameplayEffectModifier> action)
        {
            foreach (var modifier in EffectData.Effect.GameplayEffectPolicy.Modifiers)
            {
                action(modifier);
            }
        }



        private void RemoveActiveGameplayEffect(ActiveGameplayEffectData EffectData)
        {
            // There could be multiple stacked effects, due to multiple casts
            // Remove one instance of this effect from the active list
            ModifyActiveGameplayEffect(EffectData, modifier =>
            {
                // Find in the active list, and remove
                var attributeAggregatorMap = AbilitySystem.ActiveGameplayEffectsContainer.AttributeAggregatorMap;
                // If aggregator for this attribute doesn't exist, don't do anything.  
                // It may have never been added, or has already been removed
                if (attributeAggregatorMap.TryGetValue(modifier.Attribute, out var aggregator))
                {
                    var aggregatorToRemove = aggregator.Mods[modifier.ModifierOperation].FirstOrDefault(x =>
                    {
                        x.ProviderEffect.TryGetTarget(out var Effect);
                        return Effect == EffectData.Effect;
                    });

                    if (aggregatorToRemove != null)
                    {
                        aggregator.Mods[modifier.ModifierOperation].Remove(aggregatorToRemove);
                    }

                    if (aggregator.Mods[modifier.ModifierOperation].Count == 0)
                    {
                        aggregator.Mods.Remove(modifier.ModifierOperation);
                    }
                }
                aggregator.MarkDirty();
            });

        }

        private void AddActiveGameplayEffect(ActiveGameplayEffectData EffectData)
        {
            ModifyActiveGameplayEffect(EffectData, modifier =>
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
            });


            // Add cooldown effect as well.  Application of cooldown effect
            // is different to other game effects, because we don't take
            // attribute modifiers into account
            OnActiveGameplayEffectAdded(EffectData);
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

