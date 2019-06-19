using System.Threading;
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

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public class ActiveGameplayEffectsContainer {
        private IGameplayAbilitySystem AbilitySystem;
        public ActiveGameplayEffectsContainer(IGameplayAbilitySystem AbilitySystem) {
            this.AbilitySystem = AbilitySystem;
        }

        /// <summary>
        /// This is used to keep track of all the "temporary" attribute modifiers,
        /// so we can calculate them all as f(Base, Added, Multiplied, Divided) = (Base + Added) * (Multiplied/Divided)
        /// </summary>
        /// <value></value>
        public ActiveEffectAttributeAggregator ActiveEffectAttributeAggregator { get; } = new ActiveEffectAttributeAggregator();

        [SerializeField]
        private List<ActiveGameplayEffectData> _activeCooldowns = new List<ActiveGameplayEffectData>();
        public List<ActiveGameplayEffectData> ActiveCooldowns { get => _activeCooldowns; }

        public ActiveGameplayEffectsEvent ActiveGameplayEffectAdded = new ActiveGameplayEffectsEvent();

        public async Task<ActiveGameplayEffectData> ApplyGameEffect(ActiveGameplayEffectData EffectData) {
            // Durational effect.  Add granted modifiers to active list

            switch (EffectData.Effect.StackingPolicy.StackingType) {
                // Add effect as a separate instance. 
                // We still need to respect stacking limit, so check to see how many stacks we are allowed (0 = infinite)
                case EStackingType.None:
                    if (EffectData.Effect.StackingPolicy.StackLimit >= 0
                        && this.ActiveEffectAttributeAggregator.GetActiveEffects().Where(x => x.Effect == EffectData.Effect).Count() >= EffectData.Effect.StackingPolicy.StackLimit) {
                        // End oldest effect before applying this one, so we don't exceed the maximum
                        var closestEffectToExpiry = this.ActiveEffectAttributeAggregator.GetActiveEffects().OrderBy(x => x.StartWorldTime).First();
                        closestEffectToExpiry.EndEffect();
                    }
                    AddActiveGameplayEffect(EffectData);
                    ActiveGameplayEffectAdded?.Invoke(AbilitySystem, EffectData);
                    break;


            }

            // We only remove the effect if it is "Duration" (i.e. not "Infinite")
            if (EffectData.Effect.GameplayEffectPolicy.DurationPolicy != Enums.EDurationPolicy.HasDuration) return EffectData;
            // Register callbacks for removal of effects when duration expires
            var removalTime = EffectData.Effect.GameplayEffectPolicy.DurationMagnitude * 1000.0f;
            ScheduleRemoveActiveGameplayEffect(EffectData);

            return EffectData;
        }

        public async void ApplyCooldownEffect(ActiveGameplayEffectData EffectData) {
            this.ActiveCooldowns.Add(EffectData);
            await UniTask.Delay((int)(EffectData.Effect.GameplayEffectPolicy.DurationMagnitude * 1000.0f));
            this.ActiveCooldowns.Remove(EffectData);
        }

        public bool IsCooldownEffectPresent(GameplayEffect Effect) {
            return (this.ActiveCooldowns.Any(x => x.Effect == Effect));
        }

        private void OnActiveGameplayEffectAdded(ActiveGameplayEffectData EffectData) {
            ActiveGameplayEffectAdded?.Invoke(AbilitySystem, EffectData);
        }

        private void ModifyActiveGameplayEffect(ActiveGameplayEffectData EffectData, Action<GameplayEffectModifier> action) {
            foreach (var modifier in EffectData.Effect.GameplayEffectPolicy.Modifiers) {
                action(modifier);
            }
        }


        private void AddActiveGameplayEffect(ActiveGameplayEffectData EffectData) {
            ModifyActiveGameplayEffect(EffectData, modifier => {
                modifier.AttemptCalculateMagnitude(out var EvaluatedMagnitude);

                // Check if we already have an entry for this gameplay effect attribute modifier
                var attributeAggregatorMap = ActiveEffectAttributeAggregator.AddOrGet(EffectData);

                // If aggregator for this attribute doesn't exist, add it.
                if (!attributeAggregatorMap.TryGetValue(modifier.Attribute, out var aggregator)) {
                    aggregator = new Aggregator(modifier.Attribute);
                    // aggregator.Dirtied.AddListener(UpdateAttribute);
                    attributeAggregatorMap.Add(modifier.Attribute, aggregator);
                }

                aggregator.AddAggregatorMod(EvaluatedMagnitude, modifier.ModifierOperation);

                // Recalculate new value by recomputing all aggregators
                var aggregators = AbilitySystem.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.GetAggregatorsForAttribute(modifier.Attribute);
                AbilitySystem.ActiveGameplayEffectsContainer.UpdateAttribute(aggregators, modifier.Attribute);
            });


            // Add cooldown effect as well.  Application of cooldown effect
            // is different to other game effects, because we don't take
            // attribute modifiers into account
            OnActiveGameplayEffectAdded(EffectData);
        }


        /// <summary>
        /// This is currently updated every frame.  Perhaps there are ways to make this more efficient?
        /// The main advantage of checking every frame is we can manipulate the WorldStartTime to
        /// effectively "refresh" the effect or end it at will.
        /// </summary>
        /// <param name="EffectData"></param>
        /// <returns></returns>
        private async Task WaitForEffectExpiryTime(ActiveGameplayEffectData EffectData) {
            bool durationExpired = false;
            while (!durationExpired) {
                await UniTask.DelayFrame(0);
                // Check whether required time has expired
                durationExpired = EffectData.CooldownTimeRemaining <= 0 ? true : false;

            }
        }

        private async void ScheduleRemoveActiveGameplayEffect(ActiveGameplayEffectData EffectData) {
            await WaitForEffectExpiryTime(EffectData);

            // There could be multiple stacked effects, due to multiple casts
            // Remove one instance of this effect from the active list
            ModifyActiveGameplayEffect(EffectData, modifier => {

                AbilitySystem.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.RemoveEffect(EffectData);


                // Find all remaining aggregators of the same type and recompute values
                var aggregators = AbilitySystem.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.GetAggregatorsForAttribute(modifier.Attribute);

                // If there are no aggregators, set base = current
                if (aggregators.Count() == 0) {
                    var current = AbilitySystem.GetNumericAttributeBase(modifier.Attribute);
                    if (current < 0) AbilitySystem.SetNumericAttributeBase(modifier.Attribute, 0f);
                    AbilitySystem.SetNumericAttributeCurrent(modifier.Attribute, current);
                } else {
                    UpdateAttribute(aggregators, modifier.Attribute);
                }
            });

        }

        public void UpdateAttribute(IEnumerable<Aggregator> Aggregator, AttributeType AttributeType) {
            var baseAttributeValue = AbilitySystem.GetNumericAttributeBase(AttributeType);
            var newCurrentAttributeValue = Aggregator.Evaluate(baseAttributeValue);
            AbilitySystem.SetNumericAttributeCurrent(AttributeType, newCurrentAttributeValue);

        }

    }
    public class ActiveGameplayEffectsEvent : UnityEvent<IGameplayAbilitySystem, ActiveGameplayEffectData> {

    }
}

