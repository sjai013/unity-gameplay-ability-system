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
            var existingStacks = -1;
            var maxStacks = EffectData.Effect.StackingPolicy.StackLimit;
            IEnumerable<ActiveGameplayEffectData> matchingStackedActiveEffects = GetMatchingEffectsForActiveEffect(EffectData);

            switch (EffectData.Effect.StackingPolicy.StackDurationRefreshPolicy) {
                case EStackRefreshPolicy.RefreshOnSuccessfulApplication: // We refresh all instances of this game effect
                    foreach (var effect in matchingStackedActiveEffects) {
                        effect.ResetDuration();
                    }
                    break;
                case EStackRefreshPolicy.NeverRefresh: // Don't do anything.  Effect will expire naturally
                    break;

            }


            existingStacks = matchingStackedActiveEffects?.Count() ?? -1;
            if (existingStacks < maxStacks) { // We can still add more stacks.
                AddActiveGameplayEffect(EffectData);
                ActiveGameplayEffectAdded?.Invoke(AbilitySystem, EffectData);
                // We only remove the effect if it is "Duration" (i.e. not "Infinite")
                if (EffectData.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.HasDuration) {
                    // Register callbacks for removal of effects when duration expires
                    var removalTime = EffectData.Effect.GameplayEffectPolicy.DurationMagnitude * 1000.0f;
                    ScheduleRemoveActiveGameplayEffect(EffectData);
                }

            }


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

                IEnumerable<ActiveGameplayEffectData> matchingEffects;
                if (durationExpired) { // This effect is due for expiry
                    switch (EffectData.Effect.StackingPolicy.StackExpirationPolicy) {
                        case EStackExpirationPolicy.ClearEntireStack: // Remove all effects which match
                            matchingEffects = GetMatchingEffectsForActiveEffect(EffectData);
                            foreach (var effect in matchingEffects) {
                                effect.EndEffect();
                            }
                            break;
                        case EStackExpirationPolicy.RemoveSingleStackAndRefreshDuration:
                            // Remove this effect, and reset all other durations to max
                            matchingEffects = GetMatchingEffectsForActiveEffect(EffectData);
                            foreach (var effect in matchingEffects) {
                                effect.ResetDuration();
                            }
                            // This effect was going to expire anyway, but we put this here to be explicit to future code readers
                            EffectData.EndEffect();
                            break;
                        case EStackExpirationPolicy.RefreshDuration:
                            // Refreshing duration on expiry basically means the effect can never expire
                            matchingEffects = GetMatchingEffectsForActiveEffect(EffectData);
                            foreach (var effect in matchingEffects) {
                                effect.ResetDuration();
                                durationExpired = false;
                            }
                            break;

                    }
                }


            }
        }

        private async void ScheduleRemoveActiveGameplayEffect(ActiveGameplayEffectData EffectData, bool RespectEffectExpiryTime = true) {
            if (RespectEffectExpiryTime) {
                await WaitForEffectExpiryTime(EffectData);
            }

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

        public IEnumerable<ActiveGameplayEffectData> GetMatchingEffectsForActiveEffect(ActiveGameplayEffectData EffectData) {
            IEnumerable<ActiveGameplayEffectData> matchingStackedActiveEffects = null;

            switch (EffectData.Effect.StackingPolicy.StackingType) {
                // Stacking Type None:
                // Add effect as a separate instance. 
                case EStackingType.None:
                    break;

                case EStackingType.AggregatedBySource:
                    matchingStackedActiveEffects = this.ActiveEffectAttributeAggregator
                                        .GetActiveEffects()
                                        .Where(x => x.Instigator == EffectData.Instigator && x.Effect == EffectData.Effect);
                    break;

                case EStackingType.AggregatedByTarget:
                    matchingStackedActiveEffects = this.ActiveEffectAttributeAggregator
                                        .GetActiveEffects()
                                        .Where(x => x.Effect == EffectData.Effect);
                    break;
            }

            return matchingStackedActiveEffects;
        }

    }
    public class ActiveGameplayEffectsEvent : UnityEvent<IGameplayAbilitySystem, ActiveGameplayEffectData> {

    }
}

