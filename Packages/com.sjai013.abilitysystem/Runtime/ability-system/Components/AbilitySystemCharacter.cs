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
    public class AbilitySystemCharacter : MonoBehaviour
    {
        [SerializeField] protected AttributeSystemComponent _attributeSystem;
        public AttributeSystemComponent AttributeSystem { get { return _attributeSystem; } set { _attributeSystem = value; } }
        public List<GameplayEffectContainer> AppliedGameplayEffects = new List<GameplayEffectContainer>();
        public HashSet<AbstractAbilitySpec> GrantedAbilities = new HashSet<AbstractAbilitySpec>();
        public List<GameplayTagScriptableObject> AppliedTags;
        private List<IGameplayTagProvider> TagProviders = new List<IGameplayTagProvider>();
        private AbilityExecutionManager m_AbilityEM = new AbilityExecutionManager();

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
        /// Removes a tag provider
        /// </summary>
        /// <param name="source">Tag provider to remove</param>
        public void UnregisterTagSource(IGameplayTagProvider source)
        {
            this.TagProviders.Remove(source);
        }

        public float Level;

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
                var grantedTags = this.AppliedGameplayEffects[i].spec.GameplayEffect.GetGameplayEffectTags().GrantedTags;
                if (grantedTags == null) continue;
                for (var iTag = 0; iTag < grantedTags.Length; iTag++)
                {
                    for (var iCooldownTag = 0; iCooldownTag < tags.Length; iCooldownTag++)
                    {
                        if (grantedTags[iTag] == tags[iCooldownTag])
                        {
                            // If this is an infinite GE, then return null to signify this is on CD
                            if (this.AppliedGameplayEffects[i].spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Infinite) return new AbilityCooldownTime()
                            {
                                TimeRemaining = float.MaxValue,
                                TotalDuration = 0
                            };

                            var durationRemaining = this.AppliedGameplayEffects[i].spec.DurationRemaining;

                            if (durationRemaining > longestCooldown)
                            {
                                longestCooldown = durationRemaining;
                                maxDuration = this.AppliedGameplayEffects[i].spec.TotalDuration;
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
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier).GetValueOrDefault();
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
            spec.RaiseOnApplyEvent();

        }

        void UpdateAppliedTags()
        {
            // Build list of all gametags currently applied
            AppliedTags.Clear();

            // Get tags applied using gameplay effects
            for (var i = 0; i < AppliedGameplayEffects.Count; i++)
            {
                var grantedTags_Authoring = AppliedGameplayEffects[i].spec.GameplayEffect.GetGameplayTagsAuthoring().GrantedTags;
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

        void TickGameplayEffects()
        {
            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var ge = this.AppliedGameplayEffects[i].spec;

                // Can't tick instant GE
                if (ge.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Instant) continue;

                // Update time remaining.  Stritly, it's only really valid for durational GE, but calculating for infinite GE isn't harmful
                ge.UpdateRemainingDuration(Time.deltaTime);
                ge.UpdateState();
                // Tick the periodic component
                ge.TickPeriodic(Time.deltaTime, out var executePeriodicTick);
                if (executePeriodicTick && ge.IsActive)
                {
                    ApplyInstantGameplayEffect(ge);
                    ge.RaiseOnTickEvent();
                }
            }
        }


        void CleanGameplayEffects()
        {
            for (var i = AppliedGameplayEffects.Count - 1; i > 0; i--)
            {
                var ge = AppliedGameplayEffects[i];
                if (ge.spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration && ge.spec.DurationRemaining <= 0)
                {
                    ge.spec.RaiseOnRemoveEvent();
                    AppliedGameplayEffects.RemoveAt(i);
                }
            }
        }


        void Update()
        {
            // Reset all attributes to 0
            this.AttributeSystem.ResetAttributeModifiers();
            UpdateAttributeSystem();
            m_AbilityEM.StepAbilities();
            TickGameplayEffects();
            CleanGameplayEffects();
            UpdateAppliedTags();
        }
    }

    internal class AbilityExecutionManager
    {
        private float timeSinceFlush;
        private List<AbstractAbilitySpec> m_ActiveAbilities = new List<AbstractAbilitySpec>(5);
        private const float MAX_FLUSH_TIME = 5;

        public void StepAbilities()
        {
            timeSinceFlush += Time.deltaTime;

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

        private void FlushList()
        {
            if (timeSinceFlush > MAX_FLUSH_TIME)
            {
                timeSinceFlush = 0;
                if (m_ActiveAbilities.Count > 0) m_ActiveAbilities.RemoveAll(x => x == null);
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