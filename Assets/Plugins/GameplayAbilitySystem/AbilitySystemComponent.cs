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

namespace GameplayAbilitySystem
{
    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Ability System")]
    public class AbilitySystemComponent : MonoBehaviour, IGameplayAbilitySystem
    {
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

        public void Awake()
        {
            this._activeGameplayEffectsContainer = new ActiveGameplayEffectsContainer(this);
        }
        /// <inheritdoc />
        public Transform GetActor()
        {
            return this.transform;
        }

        void Update()
        {

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

        public async void ApplyBatchGameplayEffects(IEnumerable<(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level)> BatchedGameplayEffects)
        {

            var instantEffects = BatchedGameplayEffects.Where(x => x.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.Instant);
            var durationalEffects = BatchedGameplayEffects.Where(
                x =>
                    x.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.HasDuration ||
                    x.Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.Infinite
                    );

            // Apply instant effects
            foreach (var item in instantEffects)
            {
                if (await ApplyGameEffectToTarget(item.Effect, item.Target))
                {
                    // item.Target.AddGameplayEffectToActiveList(Effect);

                }
            }

            // Apply durational effects
            foreach (var effect in durationalEffects)
            {
                if (await ApplyGameEffectToTarget(effect.Effect, effect.Target))
                {

                }
            }

        }

        /// <inheritdoc />
        public async Task<GameplayEffect> ApplyGameEffectToTarget(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level = 0)
        {
            // TODO: Check to make sure all the attributes being modified by this gameplay effect exist on the target

            // TODO: Get list of tags owned by target

            // TODO: Check for immunity tags, and don't apply gameplay effect if target is immune (and also add Immunity Tags container to IGameplayEffect)

            // TODO: Check to make sure Application Tag Requirements are met (i.e. target has the required tags, if any)

            // If this is a non-instant gameplay effect (i.e. it will modify the current value, not the base value)

            // If this is an instant gameplay effect (i.e. it will modify the base value)

            // If we can apply the GameEffect, apply it to target
            //Effect.ExecuteEffect(Target);

            // Handling Instant effects is different to handling HasDuration and Infinite effects
            if (Effect.GameplayEffectPolicy.DurationPolicy == Enums.EDurationPolicy.Instant)
            {
                // Modify base attribute values.  Collect the overall change for each modifier
                Dictionary<AttributeType, Dictionary<EModifierOperationType, float>> modifierTotals = new Dictionary<AttributeType, Dictionary<EModifierOperationType, float>>();
                foreach (var modifier in Effect.GameplayEffectPolicy.Modifiers)
                {
                    if (!modifierTotals.TryGetValue(modifier.Attribute, out var modifierType))
                    {
                        // This attribute hasn't been recorded before, so create a blank new record
                        modifierType = new Dictionary<EModifierOperationType, float>();
                        modifierTotals.Add(modifier.Attribute, modifierType);
                    }

                    if (!modifierType.TryGetValue(modifier.ModifierOperation, out var value))
                    {
                        value = 0;
                        modifierType.Add(modifier.ModifierOperation, 0);

                    }

                    switch (modifier.ModifierOperation)
                    {
                        case EModifierOperationType.Add:
                            modifierTotals[modifier.Attribute][modifier.ModifierOperation] += modifier.ScaledMagnitude;
                            break;
                        case EModifierOperationType.Multiply:
                            modifierTotals[modifier.Attribute][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                            break;
                        case EModifierOperationType.Divide:
                            modifierTotals[modifier.Attribute][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                            break;
                    }
                }

                // For each attribute, calculate the final value as (base + added) * (multiplied/divided)
                foreach (var attribute in modifierTotals)
                {
                    if (!attribute.Value.TryGetValue(EModifierOperationType.Add, out var addition))
                    {
                        addition = 0;
                    }

                    if (!attribute.Value.TryGetValue(EModifierOperationType.Multiply, out var multiplication))
                    {
                        multiplication = 1;
                    }

                    if (!attribute.Value.TryGetValue(EModifierOperationType.Multiply, out var division))
                    {
                        division = 1;
                    }

                    var oldBase = this.GetNumericAttributeBase(attribute.Key);
                    var newBase = (oldBase + addition) * (multiplication / division);
                    this.SetNumericAttributeBase(attribute.Key, newBase);

                    // mark the corresponding aggregator as dirty so we can recalculate the current values
                    this.ActiveGameplayEffectsContainer.AttributeAggregatorMap.TryGetValue(attribute.Key, out var aggregator);
                    aggregator.MarkDirty();

                }
            }
            else
            {
                var EffectData = new ActiveGameplayEffectData(Effect);
                await ActiveGameplayEffectsContainer.ApplyGameEffect(EffectData);
            }


            return Effect;
        }


        /// <inheritdoc />
        public float GetNumericAttributeBase(AttributeType AttributeType)
        {
            var attributeSet = this.GetComponent<AttributeSet>();
            return attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType).BaseValue;
        }

        public void SetNumericAttributeBase(AttributeType AttributeType, float modifier)
        {
            var attributeSet = this.GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            var newValue = modifier;
            attribute.SetAttributeBaseValue(attributeSet, ref newValue);
        }

        public void SetNumericAttributeCurrent(AttributeType AttributeType, float NewValue)
        {
            var attributeSet = this.GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            attribute.SetAttributeCurrentValue(attributeSet, ref NewValue);
        }

    }




}
