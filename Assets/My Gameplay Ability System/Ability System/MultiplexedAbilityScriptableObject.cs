using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Multiplexed")]
public class MultiplexedAbilityScriptableObject : AbstractAbilityScriptableObject
{
    public MultiplexerDecisionScriptableObject[] Abilities;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new AbilitySpec(this, owner);
        spec.MultiplexedAbilitySpecs = new AbilitySpec.MultiplexAbilitySpec[Abilities.Length];
        for (var i = 0; i < Abilities.Length; i++)
        {
            var multplexAbilitySpec = new AbilitySpec.MultiplexAbilitySpec()
            {
                AbilitySpec = Abilities[i].CreateSpec(owner),
                AbilityMultiplexer = Abilities[i]
            };
            spec.MultiplexedAbilitySpecs[i] = multplexAbilitySpec;
        }
        return spec;
    }


    public class AbilitySpec : AbstractAbilitySpec
    {
        [SerializeField]
        public struct MultiplexAbilitySpec
        {
            public AbstractAbilitySpec AbilitySpec;
            public MultiplexerDecisionScriptableObject AbilityMultiplexer;
        }

        public MultiplexAbilitySpec[] MultiplexedAbilitySpecs;
        public AbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }

        public override AbilityCooldownTime CheckCooldown()
        {
            if (GetActivatableAbility(out var abilitySpec))
            {
                return abilitySpec.CheckCooldown();
            }
            return default(AbilityCooldownTime);
        }

        public override void CancelAbility()
        {
            return;
        }

        public override bool CheckGameplayTags()
        {
            return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                    && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags)
                    && AscHasAllTags(Owner, this.Ability.AbilityTags.SourceTags.RequireTags)
                    && AscHasNoneTags(Owner, this.Ability.AbilityTags.SourceTags.IgnoreTags);
        }

        public bool GetActivatableAbility(out AbstractAbilitySpec activatableAbility)
        {
            for (var i = 0; i < MultiplexedAbilitySpecs.Length; i++)
            {
                if (MultiplexedAbilitySpecs[i].AbilityMultiplexer.ShouldActivate(Owner))
                {
                    activatableAbility = MultiplexedAbilitySpecs[i].AbilitySpec;
                    return true;
                }
            }
            activatableAbility = null;
            return false;
        }

        protected override IEnumerator ActivateAbility()
        {
            if (GetActivatableAbility(out var abilitySpec))
            {
                if (abilitySpec.CanActivateAbility())
                {
                    Owner.StartCoroutine(abilitySpec.TryActivateAbility());
                }
            }
            yield break;
        }

        protected override IEnumerator PreActivate()
        {
            yield break;
        }
    }
}

