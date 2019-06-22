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
using GameplayAbilitySystem.GameplayCues;

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
                    if (matchingStackedActiveEffects == null) break;
                    foreach (var effect in matchingStackedActiveEffects) {
                        effect.ResetDuration();
                    }
                    break;
                case EStackRefreshPolicy.NeverRefresh: // Don't do anything.  Effect will expire naturally
                    break;

            }

            switch (EffectData.Effect.StackingPolicy.StackPeriodResetPolicy) {
                case EStackRefreshPolicy.RefreshOnSuccessfulApplication: // We refresh all instances of this game effect
                    if (matchingStackedActiveEffects == null) break;
                    foreach (var effect in matchingStackedActiveEffects) {
                        effect.ResetPeriodicTime();
                    }
                    break;
                case EStackRefreshPolicy.NeverRefresh: // Don't do anything.  Each stack maintains its own periodic effect
                    break;
            }


            existingStacks = matchingStackedActiveEffects?.Count() ?? -1;
            if (existingStacks < maxStacks) { // We can still add more stacks.
                AddActiveGameplayEffect(EffectData);
                ActiveGameplayEffectAdded?.Invoke(AbilitySystem, EffectData);
                // We only need to do timed checks for durational abilities
                if (EffectData.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.HasDuration
                    || EffectData.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.Infinite) {
                    var removalTime = EffectData.Effect.GameplayEffectPolicy.DurationMagnitude * 1000.0f;
                    CheckGameplayEffectForTimedEffects(EffectData);
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
                // We only apply if the effect has execute on application
                modifier.AttemptCalculateMagnitude(out var EvaluatedMagnitude);

                // Check if we already have an entry for this gameplay effect attribute modifier
                var attributeAggregatorMap = ActiveEffectAttributeAggregator.AddOrGet(EffectData);

                // If aggregator for this attribute doesn't exist, add it.
                if (!attributeAggregatorMap.TryGetValue(modifier.Attribute, out var aggregator)) {
                    aggregator = new Aggregator(modifier.Attribute);
                    // aggregator.Dirtied.AddListener(UpdateAttribute);
                    attributeAggregatorMap.Add(modifier.Attribute, aggregator);
                }

                // If this is a periodic effect, we don't add any attributes here.  They will be
                // added as required on period expiry and stored in a separate structure
                if (EffectData.Effect.Period.Period <= 0) {
                    aggregator.AddAggregatorMod(EvaluatedMagnitude, modifier.ModifierOperation);
                }


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
        /// This function is used to do checks for things that may happen on a timed basis, such as
        /// periodic effects or effect expiry
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

                if (EffectData.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.HasDuration) {
                    // Check whether required time has expired
                    // We only need to do this for effects with a finite duration
                    durationExpired = EffectData.CooldownTimeRemaining <= 0 ? true : false;
                }

                // Periodic effects only occur if the period is > 0
                if (EffectData.Effect.Period.Period > 0) {
                    CheckAndApplyPeriodicEffect(EffectData);
                }


                if (durationExpired) { // This effect is due for expiry
                    ApplyStackExpirationPolicy(EffectData, ref durationExpired);
                }

            }
        }

        private void CheckAndApplyPeriodicEffect(ActiveGameplayEffectData EffectData) {
            var TimeUntilNextPeriodicApplication = EffectData.TimeUntilNextPeriodicApplication;
            if (EffectData.TimeUntilNextPeriodicApplication <= 0) {
                var gameplayCues = EffectData.Effect.GameplayCues;
                foreach (var cue in gameplayCues) {
                    cue.HandleGameplayCue(EffectData.Target.GetActor().gameObject, new GameplayCues.GameplayCueParameters(null, null, null), EGameplayCueEvent.OnExecute);
                }

                EffectData.AddPeriodicEffectAttributeModifiers();
                EffectData.ResetPeriodicTime();
            }
        }
        private void ApplyStackExpirationPolicy(ActiveGameplayEffectData EffectData, ref bool durationExpired) {
            IEnumerable<ActiveGameplayEffectData> matchingEffects;

            switch (EffectData.Effect.StackingPolicy.StackExpirationPolicy) {
                case EStackExpirationPolicy.ClearEntireStack: // Remove all effects which match
                    matchingEffects = GetMatchingEffectsForActiveEffect(EffectData);
                    if (matchingEffects == null) break;
                    foreach (var effect in matchingEffects) {
                        effect.EndEffect();
                    }
                    break;
                case EStackExpirationPolicy.RemoveSingleStackAndRefreshDuration:
                    // Remove this effect, and reset all other durations to max
                    matchingEffects = GetMatchingEffectsForActiveEffect(EffectData);
                    if (matchingEffects == null) break;

                    foreach (var effect in matchingEffects) {
                        // We need to cater for the fact that the cooldown
                        // may have exceeded the actual limit by a little bit
                        // due to framerate.  So, when we reset the other cooldowns
                        // we need to account for this difference
                        var timeOverflow = effect.CooldownTimeRemaining;
                        effect.ResetDuration(timeOverflow);
                    }
                    // This effect was going to expire anyway, but we put this here to be explicit to future code readers
                    EffectData.EndEffect();
                    break;
                case EStackExpirationPolicy.RefreshDuration:
                    // Refreshing duration on expiry basically means the effect can never expire
                    matchingEffects = GetMatchingEffectsForActiveEffect(EffectData);
                    if (matchingEffects == null) break;
                    foreach (var effect in matchingEffects) {
                        effect.ResetDuration();
                        durationExpired = false; // Undo effect expiry.  This effect should never expire
                    }
                    break;
            }
        }
        private async void CheckGameplayEffectForTimedEffects(ActiveGameplayEffectData EffectData) {
            await WaitForEffectExpiryTime(EffectData);
            var gameplayCues = EffectData.Effect.GameplayCues;
            foreach (var cue in gameplayCues) {
                cue.HandleGameplayCue(EffectData.Target.GetActor().gameObject, new GameplayCues.GameplayCueParameters(null, null, null), EGameplayCueEvent.OnRemove);
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

