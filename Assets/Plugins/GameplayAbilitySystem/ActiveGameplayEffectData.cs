using UnityEngine;
using System;

namespace GameplayAbilitySystem.GameplayEffects
{
    /// <summary>
    /// This class is used to keep track of active <see cref="GameplayEffect"/>.  
    /// </summary>
    [Serializable]
    public class ActiveGameplayEffectData
    {
        public ActiveGameplayEffectData(GameplayEffect effect, float cooldownTimeElapsed = 0f)
        {
            this._gameplayEffect = effect;
            this._cooldownTimeElapsed = cooldownTimeElapsed;
        }

        /// <summary>
        /// The actual <see cref="GameplayEffect"/>. 
        /// </summary>
        /// <value></value>
        public GameplayEffect Effect { get => _gameplayEffect; }


        /// <summary>
        /// The cooldown time that has already elapsed for this gameplay effect
        /// </summary>
        /// <value>Cooldown time elapsed</value>
        public float CooldownTimeElapsed { get => _cooldownTimeElapsed; set => _cooldownTimeElapsed = value; }

        [SerializeField]
        private int _stacks;

        [SerializeField]
        private GameplayEffect _gameplayEffect;

        [SerializeField]
        private float _cooldownTimeElapsed;

        public void CheckOngoingTagRequirements()
        {

        }
    }
}

