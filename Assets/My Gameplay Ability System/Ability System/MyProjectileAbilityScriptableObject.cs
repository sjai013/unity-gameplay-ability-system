using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Projectile")]
public class MyProjectileAbilityScriptableObject : AbstractAbilityScriptableObject
{
    [SerializeField]
    protected Projectile projectile;
    public GameplayEffectScriptableObject GameplayEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new MyProjectileAbilitySpec(this, owner);
        spec.Level = owner.Level;
        spec.projectile = this.projectile;
        spec.CastPointComponent = owner.GetComponent<CastPointComponent>();
        return spec;
    }


    public class MyProjectileAbilitySpec : AbstractAbilitySpec
    {
        public Projectile projectile;
        public CastPointComponent CastPointComponent;
        public MyProjectileAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
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
            AbilitySystemCharacter target = null;

            var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
            var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
            // Find enemy in front using raycast and set that as target
            if (Physics.Raycast(this.CastPointComponent.GetPosition() + new Vector3(0, 0, 1), this.Owner.transform.TransformDirection(Vector3.forward), out var hit, Mathf.Infinity))
            {
                Debug.DrawRay(this.CastPointComponent.GetPosition(), this.Owner.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                target = hit.transform.GetComponent<AbilitySystemCharacter>();
                if (!target)
                {
                    EndAbility();
                    yield break;
                }

                var go = Instantiate(this.projectile.gameObject, this.CastPointComponent.GetPosition(), this.CastPointComponent.transform.rotation);
                var projectileInstance = go.GetComponent<Projectile>();
                projectileInstance.Source = Owner;
                projectileInstance.Target = target;
                this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);
                yield return projectileInstance.TravelToTarget();
                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as MyProjectileAbilityScriptableObject).GameplayEffect);
                target.ApplyGameplayEffectSpecToSelf(effectSpec);
                Destroy(go.gameObject);
            }

            EndAbility();

            // Spawn instance of projectile prefab
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}

