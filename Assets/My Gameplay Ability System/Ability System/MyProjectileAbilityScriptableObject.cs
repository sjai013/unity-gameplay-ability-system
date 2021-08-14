using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using GameplayTag.Authoring;
using UnityEditor.Animations;
using UnityEngine;


[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Projectile")]
public class MyProjectileAbilityScriptableObject : AbstractAbilityScriptableObject
{
    protected Projectile projectile;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] GameplayTagScriptableObject AnimationId;
    public GameplayEffectScriptableObject GameplayEffect;
    public AnimationTagScriptableObject[] AttackPreparationTags;
    public AnimationTagScriptableObject[] AttackingTags;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new AbilitySpec(this, owner);
        spec.Level = owner.Level;
        spec.projectile = this.projectile;
        spec.CastPointComponent = owner.GetComponent<CastPointComponent>();
        spec.playerBehaviour = owner.GetComponentInParent<PlayerBehaviour>();
        this.projectile = projectilePrefab.GetComponent<Projectile>();
        spec.AnimationId = AnimationId;
        spec.AttackPreparationTags = AttackPreparationTags;
        spec.AttackingTags = AttackingTags;
        return spec;
    }


    public class AbilitySpec : AbstractAbilitySpec
    {
        public Projectile projectile;
        public CastPointComponent CastPointComponent;
        public PlayerBehaviour playerBehaviour;
        public GameplayTagScriptableObject AnimationId;
        public AnimationTagScriptableObject[] AttackPreparationTags;
        public AnimationTagScriptableObject[] AttackingTags;

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

            // Do casting animation


            // Fire projectile forwards.  First object hit is the target.
            EndAbility();

            var go = Instantiate(this.projectile.gameObject, this.CastPointComponent.GetPosition(), this.CastPointComponent.transform.rotation);
            var projectileInstance = go.GetComponent<Projectile>();
            projectileInstance.Source = Owner;
            projectileInstance.Spec = effectSpec;

            yield return projectileInstance.Spawn();
            yield return projectileInstance.TravelForward(this.Owner.transform.forward);
            yield return projectileInstance.Despawn();

            // Spawn instance of projectile prefab
        }

        protected override IEnumerator PreActivate()
        {
            playerBehaviour.TriggerAttack();

            // Wait to enter the Attack Preparation state
            if (!(AttackPreparationTags == null || AttackPreparationTags.Length == 0))
            {
                yield return WaitForAnimationStart(AnimationId, AttackPreparationTags);
            }

            // Wait for attack animation to complete
            if (!(AttackingTags == null || AttackingTags.Length == 0))
            {
                yield return WaitForAnimationComplete(AnimationId, AttackPreparationTags);
            }

            // Wait for animation finish
            yield break;
        }

        private IEnumerator WaitForAnimationStart(GameplayTagScriptableObject animationId, AnimationTagScriptableObject[] animTags)
        {
            while (!playerBehaviour.HasAnyTags(AnimationId, AttackPreparationTags)) yield return null;
        }

        private IEnumerator WaitForAnimationComplete(GameplayTagScriptableObject animationId, AnimationTagScriptableObject[] animTags)
        {
            while (!playerBehaviour.HasNoneTags(AnimationId, AttackPreparationTags)) yield return null;
        }
    }
}

