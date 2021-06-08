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
        public AbstractAbilityScriptableObject Ability;

        /// <summary>
        /// The owner of the AbilitySpec - usually the source
        /// </summary>
        protected AbilitySystemCharacter Owner;

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
        public AbstractAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner)
        {
            this.Ability = ability;
            this.Owner = owner;
        }

        /// <summary>
        /// Try activating the ability.  Remember to use StartCoroutine() since this 
        /// is a couroutine, to allow abilities to span more than one frame.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator TryActivateAbility()
        {
            if (!CanActivateAbility()) yield break;

            isActive = true;
            yield return PreActivate();
            yield return ActivateAbility();
            EndAbility();

        }

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
        /// Cancels the ability, if it is active
        /// </summary>
        public abstract void CancelAbility();

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
            float maxDuration = 0;
            float longestCooldown = 0f;

            if (this.Ability.Cooldown == null) return new AbilityCooldownTime();
            var cooldownTags = this.Ability.Cooldown.gameplayEffectTags.GrantedTags;

            for (var i = 0; i < cooldownTags.Length; i++)
            {
                var geList = this.Owner.GetAppliedGameplayEffectsForTag(cooldownTags[i]);
                for (var iList = 0; iList < geList.Count; iList++)
                {
                    // If this is an infinite GE, then return infinite time remaining 
                    if (geList[iList].GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Infinite) return new AbilityCooldownTime()
                    {
                        TimeRemaining = float.MaxValue,
                        TotalDuration = 0
                    };

                    var durationRemaining = geList[iList].DurationRemaining;

                    if (durationRemaining > longestCooldown)
                    {
                        longestCooldown = durationRemaining;
                        maxDuration = geList[iList].TotalDuration;
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
        /// Method to activate before activating this ability.  This method is run after activation checks.
        /// </summary>
        protected abstract IEnumerator PreActivate();

        /// <summary>
        /// The logic that dictates what the ability does.  Targetting logic should be placed here.
        /// Gameplay Effects are applied in this method.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator ActivateAbility();

        /// <summary>
        /// Method to run once the ability ends
        /// </summary>
        public virtual void EndAbility()
        {
            this.isActive = false;
        }

        /// <summary>
        /// Checks whether the activating character has enough resources to activate this ability
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckCost()
        {
            if (this.Ability.Cost == null) return true;
            var geSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost, this.Level);
            // If this isn't an instant cost, then assume it passes cooldown check
            if (geSpec.GameplayEffect.gameplayEffect.DurationPolicy != EDurationPolicy.Instant) return true;

            for (var i = 0; i < geSpec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = geSpec.GameplayEffect.gameplayEffect.Modifiers[i];

                // Only worry about additive.  Anything else passes.
                if (modifier.ModifierOperator != EAttributeModifier.Add) continue;
                var costValue = (modifier.ModifierMagnitude.CalculateMagnitude(geSpec) * modifier.Multiplier).GetValueOrDefault();

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
        protected virtual bool AscHasAllTags(AbilitySystemCharacter asc, GameplayTagScriptableObject[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!asc) return true;

            for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
            {
                var abilityTag = tags[iAbilityTag];

                // If any ability tag isn't found, requirements failed
                if (asc.GetTagCount(tags[iAbilityTag]) < 1) return false;

            }
            return true;
        }

        /// <summary>
        /// Checks if an Ability System Character has none of the listed tags
        /// </summary>
        /// <param name="asc">Ability System Character</param>
        /// <param name="tags">List of tags to check</param>
        /// <returns>True, if the Ability System Character has none of the tags</returns>
        protected virtual bool AscHasNoneTags(AbilitySystemCharacter asc, GameplayTagScriptableObject[] tags)
        {
            // If the input ASC is not valid, assume check passed
            if (!asc) return true;

            for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
            {
                var abilityTag = tags[iAbilityTag];

                // If any ability tag was found, requirements failed
                if (asc.GetTagCount(tags[iAbilityTag]) >= 1) return false;

            }
            return true;
        }
    }

}