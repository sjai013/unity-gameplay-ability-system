using UnityEngine;
using System;
using GameplayAbilitySystem.Interfaces;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;

namespace GameplayAbilitySystem.GameplayEffects {
    /// <summary>
    /// This class is used to keep track of active <see cref="GameplayEffect"/>.  
    /// </summary>
    [Serializable]
    public class ActiveGameplayEffectData {
        public ActiveGameplayEffectData(GameplayEffect effect, IGameplayAbilitySystem instigator, IGameplayAbilitySystem target) {
            this._gameplayEffect = effect;
            this._startWorldTime = Time.time;
            this.Instigator = instigator;
            this.Target = target;
            if (!this.Effect.Period.ExecuteOnApplication) {
                this._timeOfLastPeriodicApplication = Time.time;
            }
        }

        bool _bForceRemoveEffect = false;

        public bool bForceRemoveEffect => _bForceRemoveEffect;

        /// <summary>
        /// The actual <see cref="GameplayEffect"/>. 
        /// </summary>
        /// <value></value>
        [SerializeField]
        public GameplayEffect Effect { get => _gameplayEffect; }


        /// <summary>
        /// The cooldown time that has already elapsed for this gameplay effect
        /// </summary>
        /// <value>Cooldown time elapsed</value>
        public float CooldownTimeElapsed { get => Time.time - _startWorldTime; }

        /// <summary>
        /// The total cooldown time for this gameplay effect
        /// </summary>
        /// <value>Cooldown time total</value>
        public float CooldownTimeTotal { get => Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.HasDuration ? Effect.GameplayEffectPolicy.DurationMagnitude : 0; }

        /// <summary>
        /// The cooldown time that is remaining for this gameplay effect
        /// </summary>
        /// <value>Cooldown time remaining</value>
        public float CooldownTimeRemaining { get => Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.HasDuration ? CooldownTimeTotal - CooldownTimeElapsed : 0; }

        private float _timeOfLastPeriodicApplication = 0;

        public float TimeSincePreviousPeriodicApplication { get => Time.time - _timeOfLastPeriodicApplication; }
        public float TimeUntilNextPeriodicApplication { get => _timeOfLastPeriodicApplication + Effect.Period.Period - Time.time; }

        private Dictionary<AttributeType, Aggregator> PeriodicEffectModificationsToDate = new Dictionary<AttributeType, Aggregator>();

        public IGameplayAbilitySystem Instigator { get; private set; }
        public IGameplayAbilitySystem Target { get; private set; }

        [SerializeField]
        private int _stacks;

        [SerializeField]
        private GameplayEffect _gameplayEffect;

        [SerializeField]
        private float _startWorldTime;

        public float StartWorldTime { get => _startWorldTime; }
        public void CheckOngoingTagRequirements() {

        }


        /// <summary>
        /// Reset duration of this effect.
        /// Optionally, we can provide an offset to compensate for
        /// the fact that the reset did not happen at exactly 0
        /// and over time this could cause time drift
        /// </summary>
        /// <param name="offset">Overflow time</param>
        public void ResetDuration(float offset = 0) {
            this._startWorldTime = Time.time;
        }

        public void EndEffect() {
            this._startWorldTime = Time.time - CooldownTimeTotal;
        }

        public void ForceEndEffect() {
            EndEffect();
            _bForceRemoveEffect = true;
        }

        /// <summary>
        /// Reset time at which last periodic application occured.
        /// Optionally, we can provide an offset to compensate for
        /// the fact that the reset did not happen at exactly 0
        /// and over time this could cause time drift
        /// </summary>
        /// <param name="offset">Overflow time</param>
        public void ResetPeriodicTime(float offset = 0) {
            this._timeOfLastPeriodicApplication = Time.time - offset;
        }

        public void AddPeriodicEffectAttributeModifiers() {
            // Check out ActiveGameplayEffectContainer.AddActiveGameplayEffect to see how to populate the ActiveEffectAttributeAggregator object
            foreach (var modifier in Effect.GameplayEffectPolicy.Modifiers) {
                modifier.AttemptCalculateMagnitude(out var EvaluatedMagnitude);

                // If aggregator for this attribute doesn't exist, add it.
                if (!PeriodicEffectModificationsToDate.TryGetValue(modifier.Attribute, out var aggregator)) {
                    aggregator = new Aggregator(modifier.Attribute);
                    // aggregator.Dirtied.AddListener(UpdateAttribute);
                    PeriodicEffectModificationsToDate.Add(modifier.Attribute, aggregator);
                }

                aggregator.AddAggregatorMod(EvaluatedMagnitude, modifier.ModifierOperation);

                // Recalculate new value by recomputing all aggregators
                var aggregators = Target.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator
                                    .GetAggregatorsForAttribute(modifier.Attribute);
                Target.ActiveGameplayEffectsContainer.UpdateAttribute(aggregators, modifier.Attribute);
            }
        }

        public Aggregator GetPeriodicAggregatorForAttribute(AttributeType Attribute) {
            PeriodicEffectModificationsToDate.TryGetValue(Attribute, out var aggregator);
            return aggregator;
        }
    }
}

