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
        [SerializeField] private float m_MaxLife;
        [SerializeField] private VisualEffect m_TrailVfx;
        [SerializeField] private VisualEffect m_MuzzleVfx;
        [SerializeField] private GameObject m_Projectile;
        [SerializeField] private float projectileDelay;
        [SerializeField] List<GameplayEffect> m_TargetGE = new List<GameplayEffect>();
        [SerializeField] private ProjectileAbility.AbilitySpec m_AbilitySpec;
        private float currentLife;
        private Vector3 m_Direction;
        private Rigidbody m_Rb;
        private bool m_MuzzleActivated = false;
        private bool m_ProjectileEnabled = false;
        protected void Start()
        {
            m_Rb = this.GetComponent<Rigidbody>();
            this.m_Projectile.SetActive(false);
            ActivateMuzzle();
            ApplyGE();
        }

        private void ApplyGE()
        {
            for (var i = 0; i < m_TargetGE.Count; i++)
            {
                if (m_TargetGE[i] == null) continue;
                var GE_spec = m_AbilitySpec.Owner.MakeOutgoingSpec(m_TargetGE[i], m_AbilitySpec.Level);
                m_AbilitySpec.Owner.ApplyGameplayEffectSpecToSelf(GE_spec);
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
            currentLife += Time.deltaTime;
            HandleProjectile();
            HandleDeath();
        }

        private void HandleProjectile()
        {
            if (!m_ProjectileEnabled && currentLife >= projectileDelay)
            {
                m_Projectile.SetActive(true);
                m_ProjectileEnabled = true;
            }
        }

        private void HandleDeath()
        {
            if (currentLife >= m_MaxLife)
            {
                // Take trail VFX and unparent it first so it stays animating
                if (m_TrailVfx != null)
                {
                    m_TrailVfx.transform.parent = null;
                    m_TrailVfx.Stop();
                    Destroy(m_TrailVfx.gameObject, 2f);
                }

                Destroy(gameObject);
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
                this.m_Rb.position += transform.right * m_Speed * Time.deltaTime;
            }
        }
    }
}