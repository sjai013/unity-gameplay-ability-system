using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;


namespace AbilitySystem
{
    public class AbilitySystemCharacter : MonoBehaviour
    {
        public AttributeSystemComponent AttributeSystem;
        public List<GameplayEffectSpec> AppliedGameplayEffects;

        public void ApplyGameplayEffectToSelf(GameplayEffectSpec spec)
        {
            for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier).GetValueOrDefault();
                var attributeModifier = new AttributeModifier();
                switch (modifier.ModifierOperator)
                {
                    case EAttributeModifier.Add:
                        attributeModifier.Add = magnitude;
                        break;
                    case EAttributeModifier.Multiply:
                        attributeModifier.Multiply = magnitude;
                        break;
                    case EAttributeModifier.Override:
                        attributeModifier.Override = magnitude;
                        break;
                }
                AttributeSystem.ModifyAttributeValue(modifier.Attribute, attributeModifier, out _);
            }
        }

        public GameplayEffectSpec MakeOutgoingSpec(GameplayEffectScriptableObject GameplayEffect, int level = 1)
        {

            return new GameplayEffectSpec(
                GameplayEffect: GameplayEffect,
                Source: this,
                Duration: GameplayEffect.gameplayEffect.Duration,
                Level: 1);
        }

    }
}
