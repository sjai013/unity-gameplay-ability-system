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
using UnityEngine.Events;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.GameplayCues;

namespace GameplayAbilitySystem {
    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Ability System")]
    public class AbilitySystemComponent : MonoBehaviour, IGameplayAbilitySystem {
        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityActivated = new GenericAbilityEvent();
        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityActivated => _onGameplayAbilityActivated;

        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityEnded = new GenericAbilityEvent();
        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityEnded => _onGameplayAbilityEnded;

        [SerializeField]
        private GameplayEvent _onGameplayEvent = new GameplayEvent();
        /// <inheritdoc />
        public GameplayEvent OnGameplayEvent => _onGameplayEvent;

        [SerializeField]
        protected ActiveGameplayEffectsContainer _activeGameplayEffectsContainer;
        /// <inheritdoc />
        public ActiveGameplayEffectsContainer ActiveGameplayEffectsContainer => _activeGameplayEffectsContainer;

        [SerializeField]
        protected List<IGameplayAbility> _runningAbilities = new List<IGameplayAbility>();
        /// <inheritdoc />
        public List<IGameplayAbility> RunningAbilities => _runningAbilities;

        [SerializeField]
        protected GenericGameplayEffectEvent _onEffectAdded = new GenericGameplayEffectEvent();
        /// <inheritdoc />
        public GenericGameplayEffectEvent OnEffectAdded => _onEffectAdded;

        [SerializeField]
        protected GenericGameplayEffectEvent _onEffectRemoved = new GenericGameplayEffectEvent();
        /// <inheritdoc />
        public GenericGameplayEffectEvent OnEffectRemoved => _onEffectRemoved;

        [SerializeField]
        private GenericAbilityEvent _onGameplayAbilityCommitted = new GenericAbilityEvent();
        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityCommitted => _onGameplayAbilityCommitted;

        public void Awake() {
            this._activeGameplayEffectsContainer = new ActiveGameplayEffectsContainer(this);
        }
        /// <inheritdoc />
        public Transform GetActor() {
            return this.transform;
        }

        void Update() {

        }

        /// <inheritdoc />
        public void HandleGameplayEvent(GameplayTag EventTag, GameplayEventData Payload) {
            /**
             * TODO: Handle triggered abilities
             * Search component for all abilities that are automatically triggered from a gameplay event
             */

            OnGameplayEvent.Invoke(EventTag, Payload);
        }

        /// <inheritdoc />
        public void NotifyAbilityEnded(GameplayAbility ability) {
            _runningAbilities.Remove(ability);
        }

        /// <inheritdoc />
        public bool TryActivateAbility(GameplayAbility Ability) {
            if (!this.CanActivateAbility(Ability)) return false;
            if (!Ability.IsAbilityActivatable(this)) return false;
            _runningAbilities.Add(Ability);
            Ability.CommitAbility(this);

            return true;
        }

        /// <inheritdoc />
        public bool CanActivateAbility(GameplayAbility Ability) {
            // Check if an ability is already active on this ASC
            if (_runningAbilities.Count > 0) {
                return false;
            }

            return true;
        }

        public async void ApplyBatchGameplayEffects(IEnumerable<(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level)> BatchedGameplayEffects) {

            var instantEffects = BatchedGameplayEffects.Where(x => x.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.Instant);
            var durationalEffects = BatchedGameplayEffects.Where(
                x =>
                    x.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.HasDuration ||
                    x.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.Infinite
                    );

            // Apply instant effects
            foreach (var item in instantEffects) {
                if (await ApplyGameEffectToTarget(item.Effect, item.Target)) {
                    // item.Target.AddGameplayEffectToActiveList(Effect);

                }
            }

            // Apply durational effects
            foreach (var effect in durationalEffects) {
                if (await ApplyGameEffectToTarget(effect.Effect, effect.Target)) {

                }
            }

        }

        /// <inheritdoc />
        public Task<GameplayEffect> ApplyGameEffectToTarget(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level = 0) {
            // TODO: Check to make sure all the attributes being modified by this gameplay effect exist on the target

            // TODO: Get list of tags owned by target

            // TODO: Check for immunity tags, and don't apply gameplay effect if target is immune (and also add Immunity Tags container to IGameplayEffect)

            // TODO: Check to make sure Application Tag Requirements are met (i.e. target has the required tags, if any)

            // If this is a non-instant gameplay effect (i.e. it will modify the current value, not the base value)

            // If this is an instant gameplay effect (i.e. it will modify the base value)

            // If we can apply the GameEffect, apply it to target
            //Effect.ExecuteEffect(Target);

            // Handling Instant effects is different to handling HasDuration and Infinite effects
            if (Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.Instant) {
                Effect.ApplyInstantEffect(Target);
            } else {
                var EffectData = new ActiveGameplayEffectData(Effect);
                _ = Target.ActiveGameplayEffectsContainer.ApplyGameEffect(EffectData);
            }

            var gameplayCues = Effect.GameplayCues;
            // Apply gamecues
            for (var i = 0; i < gameplayCues.Count; i++) {
                var cue = gameplayCues[i];
                cue.HandleGameplayCue(Target.GetActor().gameObject, EGameplayCueEventTypes.Executed, new GameplayCueParameters(null, null, null));
            }

            return Task.FromResult(Effect);
        }


        /// <inheritdoc />
        public float GetNumericAttributeBase(AttributeType AttributeType) {
            var attributeSet = this.GetComponent<AttributeSet>();
            return attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType).BaseValue;
        }

        /// <inheritdoc />
        public float GetNumericAttributeCurrent(AttributeType AttributeType) {
            var attributeSet = this.GetComponent<AttributeSet>();
            return attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType).CurrentValue;
        }

        public void SetNumericAttributeBase(AttributeType AttributeType, float modifier) {
            var attributeSet = this.GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            var newValue = modifier;
            attribute.SetAttributeBaseValue(attributeSet, ref newValue);
        }

        public void SetNumericAttributeCurrent(AttributeType AttributeType, float NewValue) {
            var attributeSet = this.GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            attribute.SetAttributeCurrentValue(attributeSet, ref NewValue);
        }

    }




}
