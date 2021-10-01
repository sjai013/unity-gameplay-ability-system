using System;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;
using UnityEngine.VFX;

namespace MyGameplayAbilitySystem.SampleAbilities.Projectile
{
    [AddComponentMenu("Gameplay Ability System/Abilities/Projectile/Projectile Brain")]
    public class ProjectileBrain : MonoBehaviour
    {
        [SerializeField] private float m_Speed;
        [SerializeField] private AnimationCurve m_SpeedMultiplier = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private bool m_UseSpeedMultiplierCurve;
        [SerializeField] private float m_MaxLife;
        [SerializeField] private VisualEffect m_TrailVfx;
        [SerializeField] private VisualEffect m_MuzzleVfx;
        [SerializeField] private VisualEffect m_DeathVfx;
        [SerializeField] private GameObject m_Projectile;
        [SerializeField] private float projectileDelay;
        [SerializeField] List<GameplayEffect> m_TargetGE = new List<GameplayEffect>();
        [SerializeField] private ProjectileAbility.AbilitySpec m_AbilitySpec;
        private int abilitySystemLayerMask;
        private float m_CurrentLife;
        private Vector3 m_Direction;
        private Rigidbody2D m_Rb;
        private bool m_MuzzleActivated = false;
        private bool m_ProjectileEnabled = false;
        protected void Start()
        {
            m_Rb = this.GetComponent<Rigidbody2D>();
            this.m_Projectile.SetActive(false);
            ActivateMuzzle();
            abilitySystemLayerMask = LayerMask.NameToLayer("Ability System Tag");

        }

        private void ApplyGE(AbilitySystemCharacter target)
        {
            if (target == null) return;
            for (var i = 0; i < m_TargetGE.Count; i++)
            {
                if (m_TargetGE[i] == null) continue;
                var GE_spec = m_AbilitySpec.Owner.MakeOutgoingSpec(m_TargetGE[i], m_AbilitySpec.Level);
                target.ApplyGameplayEffectSpecToSelf(GE_spec);
            }
        }

        public void SetDirection(Vector3 direction)
        {
            this.m_Direction = direction;
        }

        public void SetSpec(ProjectileAbility.AbilitySpec abilitySpec)
        {
            this.m_AbilitySpec = abilitySpec;
        }

        protected void Update()
        {
            m_CurrentLife += Time.deltaTime;
            HandleProjectile();
            HandleDeath();
        }

        private void HandleProjectile()
        {
            if (!m_ProjectileEnabled && m_CurrentLife >= projectileDelay)
            {
                m_Projectile.SetActive(true);
                m_ProjectileEnabled = true;
            }
        }

        private void HandleDeath()
        {
            if (m_CurrentLife >= m_MaxLife)
            {
                // Take trail VFX and unparent it first so it stays animating
                if (m_TrailVfx != null)
                {
                    m_TrailVfx.transform.parent = null;
                    m_TrailVfx.Stop();
                    Destroy(m_TrailVfx.gameObject, 2f);
                }

                Die();
            }
        }

        private void ActivateMuzzle()
        {
            if (!m_MuzzleActivated)
            {
                this.m_MuzzleVfx.transform.SetParent(m_AbilitySpec.Owner.gameObject.transform);
                Destroy(this.m_MuzzleVfx.gameObject, 0.5f);
                m_MuzzleActivated = true;
            }
        }

        protected void FixedUpdate()
        {
            if (m_ProjectileEnabled)
            {
                var multiplier = 1f;
                if (m_UseSpeedMultiplierCurve)
                {
                    multiplier = m_SpeedMultiplier.Evaluate(m_CurrentLife / m_MaxLife);
                }
                Vector2 offset = (transform.right * m_Speed * Time.deltaTime);

                this.m_Rb.position += offset * multiplier;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == abilitySystemLayerMask)
            {
                var asc = other.GetComponent<AbilitySystemTag>()?.Owner;
                if (asc == m_AbilitySpec.Owner) return;
                ApplyGE(asc);
                Die();
            }
        }

        private void Die()
        {
            if (m_DeathVfx != null)
            {
               m_DeathVfx.gameObject.transform.SetParent(null);
               m_DeathVfx.gameObject.SetActive(true); 
            }
            Destroy(gameObject);
        }

    }
}