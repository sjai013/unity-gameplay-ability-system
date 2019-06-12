using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Events;
using GameplayAbilitySystem.GameplayEffects;
using UnityEngine;
using UnityEngine.Events;
using GameplayAbilitySystem.Enums;
using System.Threading.Tasks;

namespace GameplayAbilitySystem.Interfaces
{

    /// <summary>
    /// The <see cref="IGameplayAbilitySystem"/> is the primary component of the Gameplay Ability System.
    /// Every component that needs to participate with the Ability System (such as receiving or dealing damage)
    /// needs to have this component.
    /// </summary>
    public interface IGameplayAbilitySystem
    {
        /// <summary>
        /// Called when a <see cref="IGameplayAbility"/> is activated on this <see cref="IGameplayAbilitySystem"/>.
        /// Activation may or may not result in the ability actually running.  
        /// </summary>
        /// <value></value>
        GenericAbilityEvent OnGameplayAbilityActivated { get; }

        /// <summary>
        /// Called when a <see cref="IGameplayAbility"/> is committed on this <see cref="IGameplayAbilitySystem"/>.
        /// This indicates that the resource/cooldown for the ability have been commited and the ability will be executed
        /// </summary>
        /// <value></value>
        GenericAbilityEvent OnGameplayAbilityCommitted { get; }

        /// <summary>
        /// Called when a <see cref="IGameplayAbility"/> ends <see cref="IGameplayAbilitySystem"/> (e.g. <see cref="IGameplayAbility.EndAbility(IGameplayAbilitySystem)"/> is called)
        /// This indicates that the resource/cooldown for the ability have been commited and the ability will be executed
        /// </summary>
        /// <value></value>
        GenericAbilityEvent OnGameplayAbilityEnded { get; }

        /// <summary>
        /// List of running abilities that have not ended on this <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <value></value>
        List<IGameplayAbility> RunningAbilities { get; }

        /// <summary>
        /// This event is called when a <see cref="GameplayEvent"/> is executed this <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <value></value>
        GameplayEvent OnGameplayEvent { get; }

        /// <summary>
        /// Event called when an effect is removed on this <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <value></value>
        GenericGameplayEffectEvent OnEffectRemoved { get; }

        /// <summary>
        /// Event called when an effect is added on this <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <value></value>
        GenericGameplayEffectEvent OnEffectAdded { get; }

        /// <summary>
        /// Lists all active <see cref="GameplayEffect"/> on this <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <value></value>
        ActiveGameplayEffectsContainer ActiveGameplayEffectsContainer { get; }


        [Obsolete]
        Dictionary<GameplayEffect, List<(AttributeType AttributeType, float Modifier)>> PersistedAttributeModifiers { get; }

        /// <summary>
        /// Checks to see if the <see cref="GameplayAbility"/> can be activated on this component.  Does not execute the ability.
        /// </summary>
        /// <param name="Ability">The <see cref="GameplayAbility"/> to execute</param>
        /// <returns>True if the ability can be activated, false otherwise</returns>
        bool CanActivateAbility(GameplayAbility Ability);

        /// <summary>
        /// Attempts to activate the <see cref="GameplayAbility"/>.
        /// </summary>
        /// <param name="Ability">The <see cref="GameplayAbility"/> to execute</param>
        /// <returns>True if the ability was activated, false otherwise</returns>
        bool TryActivateAbility(GameplayAbility Ability);

        /// <summary>
        /// Applies a <see cref="GameplayEffect"/> to the target <see cref="IGameplayAbilitySystem"/>.
        /// The overall effect may be modulated by the Level.
        /// </summary>
        /// <param name="Effect">Effect to be applied</param>
        /// <param name="Target">Target on which effect is to be applied</param>
        /// <param name="Level">Level of the effect.  May be used to affect the "strength" of the effect</param>
        /// <returns><see cref="GameplayEffect"/> that was applied  to the target</returns>
        Task<GameplayEffect> ApplyGameEffectToTarget(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level = 0f);

        /// <summary>
        /// Notifies this <see cref="IGameplayAbilitySystem"/> that the specified <see cref="GameplayAbility"/> has ended
        /// </summary>
        /// <param name="Ability"><see cref="GameplayAbility"/> which has ended</param>
        void NotifyAbilityEnded(GameplayAbility Ability);

        /// <summary>
        /// Gets the parent <see cref="Transform"/> of this <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <returns></returns>
        Transform GetActor();

        /// <summary>
        /// Responds to a Gameplay Event on this <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <param name="EventTag">Tag of the event</param>
        /// <param name="Payload">Payload containing information about the event, such as target, etc.</param>
        void HandleGameplayEvent(GameplayTag EventTag, GameplayEventData Payload);

        /// <summary>
        /// Gets the numerical value of an attribute attached to this <see cref="GameObject"/>
        /// </summary>
        /// <param name="AttributeType">Type of attribute to get value of</param>
        /// <returns>Current value of attribute</returns>
        float GetNumericAttributeBase(AttributeType AttributeType);

        /// <summary>
        /// Sets the numerical value of an attribute attached to this <see cref="GameObject"/>
        /// </summary>
        /// <param name="AttributeType">Type of attribute to get value of</param>
        /// <param name="NewValue">New value of attribute</param>
        /// <returns>Current value of attribute</returns>
        void SetNumericAttributeCurrent(AttributeType AttributeType, float NewValue);

        /// <summary>
        /// <para>
        /// Applies batched <see cref="GameplayEffect"/>.  This can be useful if e.g. an ability applies
        /// a number of <see cref="GameplayEffect"/>, some with <see cref="EDurationPolicy.Instant"/> Instant modifiers, and some with 
        /// <see cref="EDurationPolicy.Infinite"/> or <see cref="EDurationPolicy.HasDuration"/> modifiers.  
        /// By batching these <see cref="GameplayEffect"/>, we can ensure that all of these <see cref="GameplayEffect"/> happen
        /// with reference to the same base attribute value.  
        /// </para>
        /// <para>
        /// If you were to apply these as separate <see cref="GameplayEffect"/>, then the effect of the same game effect
        /// could be different depending on the order in which the <see cref="GameplayEffect"/> are applied.
        /// </para>
        /// </summary>
        void ApplyBatchGameplayEffects(IEnumerable<(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level)> BatchedGameplayEffects);
    }

    public class GenericGameplayEffectEvent : UnityEvent<GameplayEffect>
    {

    }


}
