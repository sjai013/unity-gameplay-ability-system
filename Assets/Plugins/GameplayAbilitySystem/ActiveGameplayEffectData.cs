using UnityEngine;
using System;
using GameplayAbilitySystem.Interfaces;

namespace GameplayAbilitySystem.GameplayEffects {
    /// <summary>
    /// This class is used to keep track of active <see cref="GameplayEffect"/>.  
    /// </summary>
    [Serializable]
    public class ActiveGameplayEffectData {
        public ActiveGameplayEffectData(GameplayEffect effect, IGameplayAbilitySystem instigator) {
            this._gameplayEffect = effect;
            this._startWorldTime = Time.time;
            this.Instigator = instigator;
        }

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


        public IGameplayAbilitySystem Instigator { get; private set; }

        [SerializeField]
        private int _stacks;

        [SerializeField]
        private GameplayEffect _gameplayEffect;

        [SerializeField]
        private float _startWorldTime;

        public float StartWorldTime { get => _startWorldTime; }
        public void CheckOngoingTagRequirements() {

        }

        public void ResetDuration() {
            this._startWorldTime = Time.time;
        }

        public void EndEffect() {
            this._startWorldTime = Time.time - CooldownTimeTotal;
        }
    }
}

