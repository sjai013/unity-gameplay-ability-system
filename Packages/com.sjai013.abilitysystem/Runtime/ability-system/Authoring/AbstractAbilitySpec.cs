using System.Collections;
using GameplayTag.Authoring;

namespace AbilitySystem.Authoring
{
    public struct AbilityCooldownTime
    {
        public float TimeRemaining;
        public float TotalDuration;
    }

    public abstract class AbstractAbilitySpec
    {
        /// <summary>
        /// The ability this AbilitySpec is linked to
        /// </summary>
        public AbstractAbility Ability;

        /// <summary>
        /// The owner of the AbilitySpec - usually the source
        /// </summary>
        public AbilitySystemCharacter Owner { get; protected set; }

        /// <summary>
        /// Ability level
        /// </summary>
        public float Level;

        /// <summary>
        /// Is this AbilitySpec currently active?
        /// </summary>
        public bool isActive;

        /// <summary>
        /// Default constructor.  Initialises the AbilitySpec from the AbstractAbilityScriptableObject
        /// </summary>
        /// <param name="ability">Ability</param>
        /// <param name="owner">Owner - usually the character activating the ability</param>
        public AbstractAbilitySpec(AbstractAbility ability, AbilitySystemCharacter owner)
        {
            this.Ability = ability;
            this.Owner = owner;
        }

        // Frame-dependant code.  Returning TRUE from this indicates the ability should end.
        public abstract bool StepAbility();

        /// <summary>
        /// Checks if this ability can be activated
        /// </summary>
        /// <returns></returns>
        public virtual bool CanActivateAbility()
        {
            return !isActive
                    && CheckGameplayTags()
                    && CheckCost()
                    && CheckCooldown().TimeRemaining <= 0;
        }

        /// <summary>
        /// Checks if Gameplay Tag requirements allow activating this ability
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckGameplayTags();

        /// <summary>
        /// Check if this ability is on cooldown
        /// </summary>
        /// <param name="maxDuration">The maximum duration associated with the Gameplay Effect 
        /// causing ths ability to be on cooldown</param>
        /// <returns></returns>
        public virtual AbilityCooldownTime CheckCooldown()
        {
            if (this.Ability.Cooldown == null) return new AbilityCooldownTime();
            var cooldownTags = this.Ability.Cooldown.GetGameplayEffectTags().GrantedTags;

            return this.Owner.CheckCooldownForTags(cooldownTags);
        }

        /// <summary>
        /// Method to run once the ability ends
        /// </summary>
        public virtual void EndAbility()
        {
            this.isActive = false;
        }

        private GameplayEffectSpec m_CostCheckGE_Cache;

        private GameplayEffectSpec TryGetCostSpec()
        {
            if (m_CostCheckGE_Cache == null || m_CostCheckGE_Cache.Level != this.Level)
            {
                m_CostCheckGE_Cache = this.Owner.MakeOutgoingSpec(this.Ability.Cost, this.Level);
            }
            return m_CostCheckGE_Cache;
        }

        /// <summary>
        /// Checks whether the activating character has enough resources to activate this ability.
        /// </summary>
        /// <returns>True - Resources available.  False - Resources unavailable.</returns>
        public virtual bool CheckCost()
        {
            if (this.Ability.Cost == null) return true;
            var costGe = TryGetCostSpec();
            if (costGe == null) return false;

            // If this isn't an instant cost, then assume it passes cooldown check
            if (costGe.GameplayEffect.gameplayEffect.DurationPolicy != EDurationPolicy.Instant) return true;

            for (var i = 0; i < costGe.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = costGe.GameplayEffect.gameplayEffect.Modifiers[i];

                // Only worry about additive.  Anything else passes.
                if (modifier.ModifierOperator != EAttributeModifier.Add) continue;
                var costValue = (modifier.ModifierMagnitude.CalculateMagnitude(costGe) * modifier.Multiplier).GetValueOrDefault();

                this.Owner.AttributeSystem.GetAttributeValue(modifier.Attribute, out var attributeValue);

                // The total attribute after accounting for cost should be >= 0 for the cost check to succeed
                if (attributeValue.CurrentValue + costValue < 0) return false;

            }
            return true;
        }

        /// <summary>
        /// Checks if an Ability System Character has all the listed tags
        /// </summary>
        /// <param name="asc">Ability System Character</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns>True, if the Ability System Character has all tags</returns>
        protected virtual bool AscHasAllTags(AbilitySystemCharacter asc, GameplayTagScriptableObject.GameplayTag[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!asc) return true;

            for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
            {
                var abilityTag = tags[iAbilityTag];

                bool requirementPassed = false;
                for (var iAscTag = 0; iAscTag < asc.AppliedTags.Count; iAscTag++)
                {
                    if (asc.AppliedTags[iAscTag].TagData == abilityTag)
                    {
                        requirementPassed = true;
                        continue;
                    }
                }
                // If any ability tag wasn't found, requirements failed
                if (!requirementPassed) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if an Ability System Character has none of the listed tags
        /// </summary>
        /// <param name="asc">Ability System Character</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns>True, if the Ability System Character has none of the tags</returns>
        protected virtual bool AscHasNoneTags(AbilitySystemCharacter asc, GameplayTagScriptableObject.GameplayTag[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!asc) return true;

            for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
            {
                var abilityTag = tags[iAbilityTag];

                bool requirementPassed = true;
                for (var iAscTag = 0; iAscTag < asc.AppliedTags.Count; iAscTag++)
                {
                    if (asc.AppliedTags[iAscTag].TagData == abilityTag)
                    {
                        requirementPassed = false;
                    }
                }
                // If any ability tag wasn't found, requirements failed
                if (!requirementPassed) return false;
            }
            return true;
        }
    }

}