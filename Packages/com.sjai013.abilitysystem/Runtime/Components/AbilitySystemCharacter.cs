using System.Collections.Generic;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTag.Authoring;
using UnityEngine;


namespace AbilitySystem
{
    public class AbilitySystemCharacter : MonoBehaviour
    {
        [SerializeField]
        protected AttributeSystemComponent _attributeSystem;
        public AttributeSystemComponent AttributeSystem { get { return _attributeSystem; } set { _attributeSystem = value; } }
        List<GameplayEffectContainer> AppliedGameplayEffects = new List<GameplayEffectContainer>();

        [SerializeField] protected GameplayEffectScriptableObject[] InitialGameplayEffects;
        public GameplayEffectScriptableObject Test;
        public float Level;

        /// <summary>
        /// Applies the gameplay effect spec to self
        /// </summary>
        /// <param name="geSpec">GameplayEffectSpec to apply</param>
        public bool ApplyGameplayEffectSpecToSelf(GameplayEffectSpec geSpec)
        {
            bool tagRequirementsOK = CheckTagRequirementsMet(geSpec);

            if (tagRequirementsOK == false) return false;


            switch (geSpec.GameplayEffect.gameplayEffect.DurationPolicy)
            {
                case EDurationPolicy.HasDuration:
                case EDurationPolicy.Infinite:
                    ApplyDurationalGameplayEffect(geSpec);
                    break;
                case EDurationPolicy.Instant:
                    ApplyInstantGameplayEffect(geSpec);
                    return true;
            }

            return true;
        }
        public GameplayEffectSpec MakeOutgoingSpec(GameplayEffectScriptableObject GameplayEffect, float level = 1f)
        {
            return GameplayEffectSpec.CreateNew(
                GameplayEffect: GameplayEffect,
                Source: this,
                Level: level);
        }

        public void InitialiseAttributes()
        {
            this.AttributeSystem.ResetAll();
            for (var i = 0; i < this.InitialGameplayEffects.Length; i++)
            {
                var spec = MakeOutgoingSpec(this.InitialGameplayEffects[i], (int)this.Level);
                this.ApplyGameplayEffectSpecToSelf(spec);
                AttributeSystem.UpdateCurrentAttributeValues();
                this.UpdateAttributeSystem();
            }
        }


        bool CheckTagRequirementsMet(GameplayEffectSpec geSpec)
        {
            bool bApplyGE = true;

            /// Build temporary list of all gametags currently applied
            var appliedTags = new List<GameplayTagScriptableObject>();
            for (var i = 0; i < AppliedGameplayEffects.Count; i++)
            {
                appliedTags.AddRange(AppliedGameplayEffects[i].spec.GameplayEffect.gameplayEffectTags.GrantedTags);
            }

            // Every tag in the ApplicationTagRequirements.RequireTags needs to be in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.RequireTags is not present, requirement is not met
            for (var i = 0; i < geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.RequireTags.Length; i++)
            {
                if (!appliedTags.Contains(geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.RequireTags[i]))
                {
                    return false;
                }
            }

            // No tag in the ApplicationTagRequirements.IgnoreTags must in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.IgnoreTags is present, requirement is not met
            for (var i = 0; i < geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags.Length; i++)
            {
                if (appliedTags.Contains(geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags[i]))
                {
                    return false;
                }
            }

            return bApplyGE;
        }

        void ApplyInstantGameplayEffect(GameplayEffectSpec spec)
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
        void ApplyDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            var modifiersToApply = new List<GameplayEffectContainer.ModifierContainer>();
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
                modifiersToApply.Add(new GameplayEffectContainer.ModifierContainer() { Attribute = modifier.Attribute, Modifier = attributeModifier });
            }
            AppliedGameplayEffects.Add(new GameplayEffectContainer() { spec = spec, modifiers = modifiersToApply.ToArray() });
        }

        void UpdateAttributeSystem()
        {
            // Set Current Value to Base Value (default position if there are no GE affecting that atribute)


            for (var i = 0; i < AppliedGameplayEffects.Count; i++)
            {
                var modifiers = AppliedGameplayEffects[i].modifiers;
                for (var m = 0; m < modifiers.Length; m++)
                {
                    var modifier = modifiers[m];
                    AttributeSystem.ModifyAttributeValue(modifier.Attribute, modifier.Modifier, out _);
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
                ApplyGameplayEffectSpecToSelf(geSpec);
            }
        }

    }
}


namespace AbilitySystem
{
    public class GameplayEffectContainer
    {
        public GameplayEffectSpec spec;
        public ModifierContainer[] modifiers;

        public class ModifierContainer
        {
            public AttributeScriptableObject Attribute;
            public AttributeModifier Modifier;
        }
    }
}