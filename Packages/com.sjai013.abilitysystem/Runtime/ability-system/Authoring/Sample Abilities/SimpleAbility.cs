using System.Collections;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    /// <summary>
    /// Simple Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [AddComponentMenu("Gameplay Ability System/Abilities/Simple Ability")]
    public class SimpleAbility : AbstractAbility
    {
        /// <summary>
        /// Gameplay Effects to apply
        /// </summary>
        public GameplayEffect[] GameplayEffects;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new AbilitySpec(this, owner);
            spec.Level = owner.Level;
            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// </summary>
        private class AbilitySpec : AbstractAbilitySpec
        {
            public AbilitySpec(AbstractAbility abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
            }

            /// <summary>
            /// Checks to make sure Gameplay Tags checks are met. 
            /// 
            /// Since the target is also the character activating the ability,
            /// we can just use Owner for all of them.
            /// </summary>
            /// <returns></returns>
            public override bool CheckGameplayTags()
            {
                return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags)
                        && AscHasAllTags(Owner, this.Ability.AbilityTags.SourceTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.SourceTags.IgnoreTags)
                        && AscHasAllTags(Owner, this.Ability.AbilityTags.TargetTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.TargetTags.IgnoreTags);
            }

            public override bool StepAbility()
            {
                // Apply cost and cooldown
                this.Ability.ApplyCooldownTo(this.Owner);
                this.Ability.ApplyCostTo(this.Owner);


                // Apply primary effect
                GameplayEffect[] effectsToApply = (this.Ability as SimpleAbility).GameplayEffects;
                for (var i = 0; i < effectsToApply.Length; i++)
                {
                    var effectSpec = this.Owner.MakeOutgoingSpec(effectsToApply[i]);
                    this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);
                }

                return true;
            }
        }
    }

}