using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Projectile")]
public class MyProjectileAbilityScriptableObject : AbstractAbilityScriptableObject
{
    protected Projectile projectile;

    [SerializeField]
    protected GameObject projectilePrefab;

    public GameplayEffectScriptableObject GameplayEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new AbilitySpec(this, owner);
        spec.Level = owner.Level;
        spec.projectile = this.projectile;
        spec.CastPointComponent = owner.GetComponent<CastPointComponent>();
        this.projectile = projectilePrefab.GetComponent<Projectile>();
        return spec;
    }


    public class AbilitySpec : AbstractAbilitySpec
    {
        public Projectile projectile;
        public CastPointComponent CastPointComponent;
        public AbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {

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

        protected override IEnumerator ActivateAbility()
        {
            var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
            var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
            var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as MyProjectileAbilityScriptableObject).GameplayEffect);
            this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
            this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

            // Fire projectile forwards.  First object hit is the target.
            var go = Instantiate(this.projectile.gameObject, this.CastPointComponent.GetPosition(), this.CastPointComponent.transform.rotation);
            var projectileInstance = go.GetComponent<Projectile>();
            projectileInstance.Source = Owner;
            projectileInstance.Spec = effectSpec;

            yield return projectileInstance.Spawn();
            yield return projectileInstance.TravelForward(this.Owner.transform.forward);
            yield return projectileInstance.Despawn();


            EndAbility();

            // Spawn instance of projectile prefab
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}

