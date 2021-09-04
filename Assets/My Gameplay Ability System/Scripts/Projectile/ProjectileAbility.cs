using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

namespace MyGameplayAbilitySystem.SampleAbilities.Projectile
{
    /// <summary>
    /// Simple Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [AddComponentMenu("Gameplay Ability System/Abilities/Projectile/Projectile Ability")]
    public class ProjectileAbility : AbstractAbility
    {
        [SerializeField] private ProjectileBrain m_Projectile;
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            AbilitySpec abilitySpec = new AbilitySpec(this, owner);
            abilitySpec.m_Projectile = this.m_Projectile;
            abilitySpec.m_SpawnPosition = owner.GetComponent<ProjectileSpawn>()?.GetLocation();
            return abilitySpec;
        }

        public class AbilitySpec : AbstractAbilitySpec
        {
            public ProjectileBrain m_Projectile;
            public Transform m_SpawnPosition;

            public AbilitySpec(AbstractAbility ability, AbilitySystemCharacter owner) : base(ability, owner)
            {
            }

            public override bool CheckGameplayTags()
            {
                throw new System.NotImplementedException();
            }

            public override bool StepAbility()
            {
                // Sequence:
                // 1. Check resources and CD
                // 2. Apply cost and cooldown
                // 3. Do Cast animation and wait for completion (TBD)
                // 4. Spawn projectile
                // 5. End (return true)

                if (this.CheckCooldown().TimeRemaining > 0) return true;
                if (!this.CheckCost()) return true;

                // Apply cost and cooldown
                this.Ability.ApplyCooldownTo(this.Owner);
                this.Ability.ApplyCostTo(this.Owner);

                // Try to get spawning position of projectile
                var projectile = Instantiate(this.m_Projectile, m_SpawnPosition?.position ?? this.Owner.transform.position, m_SpawnPosition?.rotation ?? Quaternion.identity);
                projectile.SetSpec(this);
                projectile.SetDirection(m_SpawnPosition.rotation * m_SpawnPosition.forward);
                return true;
            }
        }
    }
}