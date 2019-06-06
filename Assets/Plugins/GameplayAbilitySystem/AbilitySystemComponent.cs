using System.Linq;
using System.Runtime.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Events;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;
using GameplayAbilitySystem.Attributes;

namespace GameplayAbilitySystem
{
    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Ablity System")]
    public class AbilitySystemComponent : MonoBehaviour, IGameplayAbilitySystem
    {
        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityActivated = new GenericAbilityEvent();

        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityCommitted = new GenericAbilityEvent();

        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityEnded = new GenericAbilityEvent();

        [SerializeField]
        private GameplayEvent _onGameplayEvent = new GameplayEvent();

        [SerializeField]
        protected List<ActiveGameplayEffectData> _activeGameplayEffects = new List<ActiveGameplayEffectData>();

        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityActivated => _onGameplayAbilityActivated;

        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityCommitted => _onGameplayAbilityCommitted;

        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityEnded => _onGameplayAbilityEnded;

        /// <inheritdoc />
        public GameplayEvent OnGameplayEvent => _onGameplayEvent;

        /// <inheritdoc />
        protected List<IGameplayAbility> _runningAbilities = new List<IGameplayAbility>();

        /// <inheritdoc />
        public List<ActiveGameplayEffectData> ActiveGameplayEffects => _activeGameplayEffects;

        /// <inheritdoc />
        public List<IGameplayAbility> RunningAbilities => _runningAbilities;

        /// <inheritdoc />
        public Transform GetActor()
        {
            return this.transform;
        }

        void Update()
        {
            UpdateCooldowns();
        }

        protected void UpdateCooldowns()
        {
            // Update gameplay effect times
            for (var i = 0; i < _activeGameplayEffects.Count; i++)
            {
                var effect = _activeGameplayEffects[i];
                effect.CooldownTimeElapsed += Time.deltaTime;
            }

            _activeGameplayEffects.RemoveAll(x => x.Effect.EffectExpired(x.CooldownTimeElapsed));
        }

        /// <inheritdoc />
        public void HandleGameplayEvent(GameplayTag EventTag, GameplayEventData Payload)
        {
            /**
             * TODO: Handle triggered abilities
             * Search component for all abilities that are automatically triggered from a gameplay event
             */

            OnGameplayEvent.Invoke(EventTag, Payload);
        }

        /// <inheritdoc />
        public void NotifyAbilityEnded(GameplayAbility ability)
        {
            _runningAbilities.Remove(ability);
        }

        /// <inheritdoc />
        public bool TryActivateAbility(GameplayAbility Ability)
        {
            if (!this.CanActivateAbility(Ability)) return false;
            if (!Ability.IsAbilityActivatable(this)) return false;
            _runningAbilities.Add(Ability);
            Ability.ActivateAbility(this);
            return true;
        }

        /// <inheritdoc />
        public bool CanActivateAbility(GameplayAbility Ability)
        {
            // Check if an ability is already active on this ASC
            if (_runningAbilities.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public GameplayEffect ApplyGameEffectToTarget(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level = 0)
        {
            if (Effect.ExecuteEffect(Target))
            {
                this.AddGameplayEffectToActiveList(Effect);
            }
            return Effect;
        }

        /// <inheritdoc />
        public void AddGameplayEffectToActiveList(GameplayEffect Effect)
        {
            this._activeGameplayEffects.Add(new ActiveGameplayEffectData(Effect));
        }

        /// <inheritdoc />
        public float GetNumericAttribute(AttributeType AttributeType)
        {
            var attributeSet = this.GetComponent<AttributeSet>();
            return attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType).BaseValue;
        }


    }

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
            this._stacks = 1;
        }

        /// <summary>
        /// The actual <see cref="GameplayEffect"/>. 
        /// </summary>
        /// <value></value>
        public GameplayEffect Effect { get => _gameplayEffect; }

        /// <summary>
        /// The number of stacks of this <see cref="GameplayEffect"/>
        /// </summary>
        /// <value></value>
        public int Stacks { get => _stacks; set => _stacks = value; }
        
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

    }
}
