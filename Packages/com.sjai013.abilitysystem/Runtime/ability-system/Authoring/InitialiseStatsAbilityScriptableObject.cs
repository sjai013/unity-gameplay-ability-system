using System.Collections;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Stat Initialisation")]
    public class InitialiseStatsAbilityScriptableObject : AbstractAbilityScriptableObject
    {
        public GameplayEffectScriptableObject[] InitialisationGE;

        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new InitialiseStatsAbility(this, owner);
            spec.Level = owner.Level;
            return spec;
        }

        public class InitialiseStatsAbility : AbstractAbilitySpec
        {
            public InitialiseStatsAbility(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
            }

            public override void CancelAbility()
            {
            }

            public override bool CheckGameplayTags()
            {
                return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags);
            }

            protected override IEnumerator ActivateAbility()
            {
                // Apply cost and cooldown (if any)
                if (this.Ability.Cooldown)
                {
                    var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                    this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                }

                if (this.Ability.Cost)
                {
                    var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                    this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);
                }

                InitialiseStatsAbilityScriptableObject abilitySO = this.Ability as InitialiseStatsAbilityScriptableObject;
                this.Owner.AttributeSystem.UpdateAttributeCurrentValues();

                for (var i = 0; i < abilitySO.InitialisationGE.Length; i++)
                {
                    var effectSpec = this.Owner.MakeOutgoingSpec(abilitySO.InitialisationGE[i]);
                    this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);
                    this.Owner.AttributeSystem.UpdateAttributeCurrentValues();
                }

                yield break;
            }

            protected override IEnumerator PreActivate()
            {
                yield break;
            }
        }
    }

}