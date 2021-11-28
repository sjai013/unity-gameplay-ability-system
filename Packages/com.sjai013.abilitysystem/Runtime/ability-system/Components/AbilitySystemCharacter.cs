using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTag.Authoring;
using UnityEngine;


namespace AbilitySystem
{
    [AddComponentMenu("Gameplay Ability System/Ability System Character")]
    [RequireComponent(typeof(GameplayTagAggregator))]

    public class AbilitySystemCharacter : MonoBehaviour
    {
        [SerializeField] protected AttributeSystemComponent m_AttributeSystem;
        [SerializeField] private GameplayTagAggregator m_GameplayTagAggregator;
        [SerializeField] private TickSystem m_TickSystem;
        public AttributeSystemComponent AttributeSystem { get { return m_AttributeSystem; } set { m_AttributeSystem = value; } }
        public List<GameplayEffectSpec> AppliedGameplayEffects = new();
        public HashSet<AbstractAbilitySpec> GrantedAbilities = new();
        public List<GameplayTagScriptableObject> AppliedTags;
        public float Level;
        private List<IGameplayTagProvider> TagProviders = new();
        private AbilityExecutionManager m_AbilityEM = new();

        void Start()
        {
            m_TickSystem.RegisterAbilitySystemCharacter(this);
        }

        /// <summary>
        /// Add a tag provider, so we can add customised tag sources to the character
        /// 
        /// </summary>
        /// <param name="source">The source of the tags</param>
        public void RegisterTagSource(IGameplayTagProvider source)
        {
            if (this.TagProviders.Contains(source)) return;
            this.TagProviders.Add(source);
        }

        /// <summary>
        /// Removes a Gameplay Effect Spec, given that spec
        /// </summary>
        /// <param name="spec">Gameplay Effect Spec to remove</param>
        /// <returns>True, if remove was successful</returns>
        public bool RemoveActiveGameplayEffect(GameplayEffectSpec spec)
        {
            return AppliedGameplayEffects.Remove(spec);
        }

        /// <summary>
        /// Removes a list of Gameplay Effect Specs, given the specs
        /// </summary>
        /// <param name="specs">List of specs to remove</param>
        /// <returns>True, if atleast one spec was removed</returns>
        public bool RemoveActiveGameplayEffect(List<GameplayEffectSpec> specs)
        {
            var removed = false;
            for (var i = 0; i < specs.Count; i++)
            {
                if (RemoveActiveGameplayEffect(specs[i]))
                {
                    removed = true;
                }
            }
            return removed;
        }

        /// <summary>
        /// Removes a list of Gameplay Effect Specs, given the specs
        /// </summary>
        /// <param name="specs">List of specs to remove</param>
        /// <returns>True, if atleast one spec was removed</returns>
        public bool RemoveActiveGameplayEffect(GameplayEffectSpec[] specs)
        {
            var removed = false;
            for (var i = 0; i < specs.Length; i++)
            {
                if (RemoveActiveGameplayEffect(specs[i]))
                {
                    removed = true;
                }
            }
            return removed;
        }

        /// <summary>
        /// Removes all Gameplay Effect Specs matching a Gameplay Effect (matched using their Asset Tag)
        /// </summary>
        /// <param name="ge">Gameplay Effect to match</param>
        /// <returns>Number of Gameplay Effect Specs removed</returns>
        public int RemoveActiveGameplayEffect(GameplayEffect ge)
        {
            return AppliedGameplayEffects.RemoveAll(x => x.GameplayEffect.GetGameplayEffectTags().AssetTag == ge.GetGameplayEffectTags().AssetTag);
        }

        public bool HasActiveGameplayEffect(GameplayEffectSpec spec)
        {
            if (spec == null) return false;
            return AppliedGameplayEffects.Contains(spec);
        }

        /// <summary>
        /// Removes a tag provider
        /// </summary>
        /// <param name="source">Tag provider to remove</param>
        public void UnregisterTagSource(IGameplayTagProvider source)
        {
            this.TagProviders.Remove(source);
        }


        public void GrantAbility(AbstractAbilitySpec spec)
        {
            this.GrantedAbilities.Add(spec);
        }

        /// <summary>
        /// Returns the first granted ability spec matching an ability
        /// </summary>
        /// <param name="ability">Ability to find</param>
        /// <param name="abilitySpec">Granted ability spec</param>
        /// <returns>True if an ability spec was found, false otherwise</returns>
        public bool GetGrantedAbilitySpec(AbstractAbility ability, out AbstractAbilitySpec abilitySpec)
        {
            abilitySpec = null;
            foreach (var spec in GrantedAbilities)
            {
                if (spec.Ability == ability)
                {
                    abilitySpec = spec;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the first granted ability spec which has an asset tag matching the passed in asset tag.
        /// </summary>
        /// <param name="assetTag">Asset tag to match</param>
        /// <param name="abilitySpec">Granted ability spec</param>
        /// <returns>True if an ability spec was found, false otherwise</returns>
        public bool GetGrantedAbilitySpec(GameplayTagScriptableObject assetTag, out AbstractAbilitySpec abilitySpec)
        {
            abilitySpec = null;
            foreach (var spec in GrantedAbilities)
            {
                if (spec.Ability.AbilityTags.AssetTag == assetTag.TagData)
                {
                    abilitySpec = spec;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Activate an ability defined by the spec.
        /// </summary>
        /// <param name="spec"></param>
        public void ActivateAbility(AbstractAbilitySpec spec)
        {
            m_AbilityEM.AddAbility(spec);
        }

        public void RemoveAbilitiesWithTag(GameplayTagScriptableObject tag)
        {
            GrantedAbilities.RemoveWhere(x => x.Ability.AbilityTags.AssetTag == tag.TagData);
        }

        public AbilityCooldownTime CheckCooldownForTags(GameplayTagScriptableObject.GameplayTag[] tags)
        {
            float longestCooldown = 0f;
            float maxDuration = 0;

            // Check if the cooldown tag is granted to the player, and if so, capture the remaining duration for that tag
            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var grantedTags = this.AppliedGameplayEffects[i].GameplayEffect.GetGameplayEffectTags().GrantedTags;
                if (grantedTags == null) continue;
                for (var iTag = 0; iTag < grantedTags.Length; iTag++)
                {
                    for (var iCooldownTag = 0; iCooldownTag < tags.Length; iCooldownTag++)
                    {
                        if (grantedTags[iTag] == tags[iCooldownTag])
                        {
                            // If this is an infinite GE, then return null to signify this is on CD
                            if (this.AppliedGameplayEffects[i].GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Infinite) return new AbilityCooldownTime()
                            {
                                TimeRemaining = float.MaxValue,
                                TotalDuration = 0
                            };

                            var durationRemaining = this.AppliedGameplayEffects[i].DurationRemaining;

                            if (durationRemaining > longestCooldown)
                            {
                                longestCooldown = durationRemaining;
                                maxDuration = this.AppliedGameplayEffects[i].TotalDuration;
                            }
                        }

                    }
                }
            }

            return new AbilityCooldownTime()
            {
                TimeRemaining = longestCooldown,
                TotalDuration = maxDuration
            };
        }

        /// <summary>
        /// Applies the gameplay effect spec to self
        /// </summary>
        /// <param name="geSpec">GameplayEffectSpec to apply</param>
        public bool ApplyGameplayEffectSpecToSelf(GameplayEffectSpec geSpec)
        {
            if (geSpec == null) return true;
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
                    break;
            }


            return true;
        }

        public GameplayEffectSpec MakeOutgoingSpec(GameplayEffect GameplayEffect, float? level = 1f)
        {
            level = level ?? this.Level;
            return GameplayEffectSpec.CreateNew(
                GameplayEffect: GameplayEffect,
                Source: this,
                Level: level.GetValueOrDefault(1));
        }

        bool CheckTagRequirementsMet(GameplayEffectSpec geSpec)
        {
            // Every tag in the ApplicationTagRequirements.RequireTags needs to be in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.RequireTags is not present, requirement is not met
            var geTags = geSpec.GameplayEffect.GetGameplayTagsAuthoring();

            for (var i = 0; i < geTags.ApplicationTagRequirements.RequireTags?.Length; i++)
            {
                var tag = AppliedTags.FirstOrDefault(x => x == geTags.ApplicationTagRequirements.RequireTags[i]);

                if (tag == default)
                {
                    return false;
                }
            }

            // No tag in the ApplicationTagRequirements.IgnoreTags must in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.IgnoreTags is present, requirement is not met
            for (var i = 0; i < geTags.ApplicationTagRequirements.IgnoreTags?.Length; i++)
            {
                var tag = AppliedTags.FirstOrDefault(x => x == geTags.ApplicationTagRequirements.RequireTags[i]);

                if (tag != default)
                {
                    return false;
                }
            }

            return true;
        }

        void ApplyInstantGameplayEffect(GameplayEffectSpec spec)
        {
            for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec, modifier.Multiplier)).GetValueOrDefault();
                var attribute = modifier.Attribute;

                // If attribute doesn't exist on this character, continue to next attribute
                if (attribute == null) continue;
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

            spec.RaiseOnApplyEvent();
            spec.RaiseOnRemoveEvent();
            HandleRemoveGameplayEffectsWithTag(spec);
        }

        void ApplyDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            GameplayEffectSpec.ModifierContainer[] modifiersToApply = new GameplayEffectSpec.ModifierContainer[spec.GameplayEffect.gameplayEffect.Modifiers.Length];
            for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec, modifier.Multiplier)).GetValueOrDefault();
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
                modifiersToApply[i] = new() { Attribute = modifier.Attribute, Modifier = attributeModifier };
            }
            spec.modifiers = modifiersToApply;
            spec.RaiseOnApplyEvent();
            HandleRemoveGameplayEffectsWithTag(spec);
            AppliedGameplayEffects.Add(spec);
        }

        private void HandleRemoveGameplayEffectsWithTag(GameplayEffectSpec spec)
        {
            // Remove Gameplay Effects With Tag
            var geToRemove = spec.GameplayEffect.GetGameplayEffectTags().RemoveGameplayEffectsWithTag;
            for (var i = 0; i < geToRemove.Length; i++)
            {
                if (m_GameplayTagAggregator.TryGetGameplayEffectsForTag(geToRemove[i], out var geSpecs))
                {
                    this.RemoveActiveGameplayEffect(geSpecs);
                }
            }
        }

        void UpdateAppliedTags()
        {
            // Build list of all gametags currently applied
            AppliedTags.Clear();

            // Get tags applied using gameplay effects
            for (var i = 0; i < AppliedGameplayEffects.Count; i++)
            {
                var grantedTags_Authoring = AppliedGameplayEffects[i].GameplayEffect.GetGameplayTagsAuthoring().GrantedTags;
                if (grantedTags_Authoring != null)
                {
                    AppliedTags.AddRange(grantedTags_Authoring);
                }
            }

            // Get tags applied using external tag providers
            for (var i = 0; i < TagProviders.Count; i++)
            {
                AppliedTags.AddRange(TagProviders[i].ListTags());
            }
        }

        void UpdateAttributeSystem()
        {
            // Reset all attributes to 0
            this.AttributeSystem.ResetAttributeModifiers();

            // Set Current Value to Base Value (default position if there are no GE affecting that atribute)
            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var modifiers = this.AppliedGameplayEffects[i].modifiers;
                for (var m = 0; m < modifiers.Length; m++)
                {
                    var modifier = modifiers[m];
                    AttributeSystem.UpdateAttributeModifiers(modifier.Attribute, modifier.Modifier, out _);
                }
            }
        }

        void TickGameplayEffects(float tickValue)
        {
            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var ge = this.AppliedGameplayEffects[i];

                // Can't tick instant GE
                if (ge.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Instant) continue;

                // Update time remaining.  Stritly, it's only really valid for durational GE, but calculating for infinite GE isn't harmful
                ge.UpdateRemainingDuration(tickValue);
                ge.UpdateState();

                // Tick the periodic component
                ge.TickPeriodic(tickValue, out var executePeriodicTick);
                if (executePeriodicTick && ge.IsActive && ge.PeriodDefinition.GameplayEffect != null)
                {
                    var spec = MakeOutgoingSpec(ge.PeriodDefinition.GameplayEffect, ge.Level);
                    spec.SetTarget(ge.Target);
                    spec.Target.ApplyGameplayEffectSpecToSelf(spec);
                    ge.RaiseOnTickEvent();
                }
            }
        }


        void CleanGameplayEffects()
        {
            for (var i = AppliedGameplayEffects.Count - 1; i > 0; i--)
            {
                var ge = AppliedGameplayEffects[i];
                if (ge.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration && ge.DurationRemaining <= 0)
                {
                    ge.RaiseOnRemoveEvent();
                    AppliedGameplayEffects.RemoveAt(i);
                }
            }
        }

        public void UpdateGameplayEffects(float tickValue)
        {
            TickGameplayEffects(tickValue);
            CleanGameplayEffects();
        }

        public void UpdateAbilitySpecs()
        {
            m_AbilityEM.StepAbilities();
        }

        public void Tick(float tickValue)
        {
            UpdateAttributeSystem();
            UpdateAbilitySpecs();
            UpdateGameplayEffects(tickValue);
            UpdateAppliedTags();
        }
    }

    internal class AbilityExecutionManager
    {
        private List<AbstractAbilitySpec> m_ActiveAbilities = new List<AbstractAbilitySpec>(5);
        public void StepAbilities()
        {
            // Step through all active abilities
            for (var i = 0; i < m_ActiveAbilities.Count; i++)
            {
                if (m_ActiveAbilities[i] == null)
                {
                    continue;
                }

                if (m_ActiveAbilities[i].StepAbility())
                {
                    m_ActiveAbilities[i] = null;
                }
            }
        }
        public void AddAbility(AbstractAbilitySpec spec)
        {
            m_ActiveAbilities.Add(spec);
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