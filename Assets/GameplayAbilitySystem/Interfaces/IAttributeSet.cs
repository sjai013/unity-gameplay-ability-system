using GameplayAbilitySystem.GameplayEffects;
using UnityEngine.Events;
using System;
using GameplayAbilitySystem.Attributes.Components;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitySystem.Interfaces {
    /// <summary>
    /// <para>
    /// An <see cref="IAttributeSet"/> defines the list of <see cref="IAttribute"/> that a player (that has an <see cref="AbilitySystemComponent"/>) possesses.  
    /// The attributes themselves are assigned to the <see cref="IAttributeSet"/> by dragging <see cref="ScriptableObject"/> of type
    /// <see cref="IAttribute"/> to the <see cref="IAttributeSet"/> in the Unity Inspector.
    /// </para>
    /// <para>
    /// This will usually include <see cref="IAttribute"/>s such as Health, Mana, Speed, etc.
    /// </para>
    /// </summary>
    public interface IAttributeSet {
        /// <summary>
        /// Decides whether this attribute change should take effect
        /// </summary>
        /// <param name="Effect">Effect being executed</param>
        /// <param name="EvalData">Attribute modifier</param>
        /// <returns>True if attribute change should take effect, false otherwise</returns>
        bool PreGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData);

        /// <summary>
        /// Called before modification to the base attribute value.  This should be used to enforce clamping rules
        /// (e.g.) health can't be greater than max health.
        /// </summary>
        /// <param name="IAttribute">Attribute which is changing</param>
        /// <param name="NewBaseValue">New value of the attribute.  This is passed as ref, so this method can modify it.</param>        
        void PreAttributeBaseChange(IAttribute Attribute, ref float NewBaseValue);

        /// <summary>
        /// Called before modification to the current attribute value.  This should be used to enforce clamping rules
        /// (e.g.) health can't be greater than max health
        /// </summary>
        /// <param name="IAttribute">Attribute which is changing</param>
        /// <param name="NewValue">New value of the attribute.  This is passed as ref, so this method can modify it.</param>
        void PreAttributeChange(IAttribute Attribute, ref float NewValue);

        /// <summary>
        /// Called after modification to attribute.  This should be used to execute reactions to attribute change, such 
        /// as dying when health is 0, or showing a "damaged" animation when health decreases
        /// </summary>
        /// <param name="Effect">Effect that was executed</param>
        /// <param name="EvalData">Attribute modifier</param>
        void PostGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData);


        /// <summary>
        /// Event called whenever a base attribute value changes
        /// </summary>
        /// <value>Event is raised with a payload of <see cref="AttributeChangeDataEvent"/></value>
        AttributeChangeDataEvent AttributeBaseValueChanged { get; }

        /// <summary>
        /// Event called whenever a current attribute value changes
        /// </summary>
        /// <value>Event is raised with a payload of <see cref="AttributeChangeDataEvent"/></value>
        AttributeChangeDataEvent AttributeCurrentValueChanged { get; }

        /// <summary>
        /// Gets the <see cref="AbilitySystemComponent"/> that owns this Attribute Set
        /// </summary>
        /// <returns>Ability System Component which owns this <see cref="IAttributeSet"/></returns>
        AbilitySystemComponent GetOwningAbilitySystem();

        /// <summary>
        /// List of <see cref="IAttribute"/> that belong to this <see cref="IAttributeSet"/>
        /// </summary>
        /// <value>List of <see cref="IAttribute"/> </value>
        List<Attribute> Attributes { get; set; }

        BaseAttributeChangeHandler PreAttributeBaseChangeHandler { get; }
        BaseAttributeChangeHandler PreAttributeChangeHandler { get; }


    }


}
