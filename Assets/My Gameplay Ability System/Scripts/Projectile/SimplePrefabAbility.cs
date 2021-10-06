using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

namespace MyGameplayAbilitySystem.SampleAbilities.SimplePrefab
{
    /// <summary>
    /// Simple Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [AddComponentMenu("Gameplay Ability System/Abilities/Prefab/Simple Prefab Ability")]
    public class SimplePrefabAbility : AbstractAbility
    {
        [SerializeField] private SimplePrefabAbilitySpecContext m_Ability;
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            AbilitySpec abilitySpec = new AbilitySpec(this, owner);
            abilitySpec.Position = owner.GetComponent<CursorTargetLocator>().GetCursorWorldPosition();
            abilitySpec.Prefab = m_Ability.gameObject;
            return abilitySpec;
        }

        public class AbilitySpec : AbstractAbilitySpec
        {
            public GameObject Prefab;
            public Vector2 Position;

            public AbilitySpec(AbstractAbility ability, AbilitySystemCharacter owner) : base(ability, owner)
            {
            }

            public override bool CheckGameplayTags()
            {
                //
                return true;
            }

            public override bool StepAbility()
            {
                // Sequence:
                // 1. Check resources and CD
                // 2. Apply cost and cooldown
                // 3. Do Cast animation and wait for completion (TBD)
                // 4. Spawn ability Prefab
                // 5. End (return true)

                if (this.CheckCooldown().TimeRemaining > 0) return true;
                if (!this.CheckCost()) return true;

                // Apply cost and cooldown
                this.Ability.ApplyCooldownTo(this.Owner);
                this.Ability.ApplyCostTo(this.Owner);

                var prefab = Instantiate(this.Prefab);
                prefab.GetComponent<SimplePrefabAbilitySpecContext>().AbilitySpec = this;
                return true;
            }
        }
    }
}