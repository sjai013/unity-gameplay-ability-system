using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;


namespace AbilitySystem
{


    public class AbilitySystemCharacter : MonoBehaviour
    {
        [SerializeField]
        private AttributeSystemComponent _attributeSystem;
        public AttributeSystemComponent AttributeSystem { get { return _attributeSystem; } set { _attributeSystem = value; } }
        public List<(GameplayEffectSpec spec, (AttributeScriptableObject attribute, AttributeModifier modifier)[] modifiers)> AppliedGameplayEffects = new List<(GameplayEffectSpec spec, (AttributeScriptableObject attribute, AttributeModifier modifier)[] modifiers)>();

        public GameplayEffectScriptableObject[] InitialGE;
        public GameplayEffectScriptableObject Test;
        public float Level;

        /// <summary>
        /// Applies the gameplay effect spec to self
        /// </summary>
        /// <param name="spec">GameplayEffectSpec to apply</param>
        public void ApplyGameplayEffect(GameplayEffectSpec spec)
        {
            switch (spec.GameplayEffect.gameplayEffect.DurationPolicy)
            {
                case EDurationPolicy.HasDuration:
                case EDurationPolicy.Infinite:
                    ApplyDurationalGameplayEffect(spec);
                    return;
                case EDurationPolicy.Instant:
                    ApplyInstantGameplayEffect(spec);
                    break;
            }

        }

        private void ApplyInstantGameplayEffect(GameplayEffectSpec spec)
        {
            for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier).GetValueOrDefault();
                var attribute = modifier.Attribute;
                this.AttributeSystem.GetAttributeValue(attribute, out var attributeValue);

                switch (modifier.ModifierOperator)
                {
                    case EAttributeModifier.Add:
                        attributeValue.BaseValue += magnitude;
                        break;
                    case EAttributeModifier.Multiply:
                        attributeValue.BaseValue *= magnitude;
                        break;
                    case EAttributeModifier.Override:
                        attributeValue.BaseValue = magnitude;
                        break;
                }
                this.AttributeSystem.SetAttributeBaseValue(attribute, attributeValue.BaseValue);
            }
        }
        private void ApplyDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            var modifiersToApply = new List<(AttributeScriptableObject attribute, AttributeModifier modifier)>();
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
                modifiersToApply.Add((modifier.Attribute, attributeModifier));
            }
            AppliedGameplayEffects.Add((spec, modifiersToApply.ToArray()));
        }

        public GameplayEffectSpec MakeOutgoingSpec(GameplayEffectScriptableObject GameplayEffect, float level = 1f)
        {
            return new GameplayEffectSpec(
                GameplayEffect: GameplayEffect,
                Source: this,
                Duration: GameplayEffect.gameplayEffect.Duration,
                Level: level);
        }

        public void InitialiseAttributes()
        {
            this.AttributeSystem.ResetAll();
            for (var i = 0; i < this.InitialGE.Length; i++)
            {
                var spec = MakeOutgoingSpec(this.InitialGE[i], (int)this.Level);
                this.ApplyGameplayEffect(spec);
            }
        }

        void UpdateAttributeSystem()
        {
            for (var i = 0; i < AppliedGameplayEffects.Count; i++)
            {
                var modifiers = AppliedGameplayEffects[i].modifiers;
                for (var m = 0; m < modifiers.Length; m++)
                {
                    var modifier = modifiers[m];
                    AttributeSystem.ModifyAttributeValue(modifier.attribute, modifier.modifier, out _);
                }
            }
        }

        void TickGameplayEffects()
        {
            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var ge = this.AppliedGameplayEffects[i].spec;
                if (ge.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration)
                {
                    ge.Tick(Time.deltaTime);
                }
            }
        }

        void CleanGameplayEffects()
        {
            this.AppliedGameplayEffects.RemoveAll(x => x.spec.Duration <= 0);
        }

        void Start()
        {
            InitialiseAttributes();
        }
        void Update()
        {
            // Reset all attributes to 0
            this.AttributeSystem.ResetAttributeModifiers();
            UpdateAttributeSystem();

            TickGameplayEffects();
            CleanGameplayEffects();
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
            {
                var geSpec = MakeOutgoingSpec(this.Test, level: (int)this.Level);
                ApplyGameplayEffect(geSpec);
            }
        }

    }
}
