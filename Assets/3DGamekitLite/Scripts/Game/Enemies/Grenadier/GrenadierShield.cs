using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class GrenadierShield : MonoBehaviour
    {
        private static Collider[] sOverlapCache = new Collider[16];

        public new ParticleSystem particleSystem;

        protected SphereCollider m_Collider;
        protected int m_PlayerMask;

        private void OnEnable()
        {
            particleSystem.gameObject.SetActive(true);
            particleSystem.time = 0;
            particleSystem.Play(true);

            m_Collider = GetComponent<SphereCollider>();

            m_PlayerMask = 1 << LayerMask.NameToLayer("Player");
        }

        private void Update()
        {
            Damageable.DamageMessage data;
            data.damageSource = transform.position;
            data.amount = 1;
            data.damager = this;
            data.stopCamera = false;
            data.throwing = true;

            int count = Physics.OverlapSphereNonAlloc(transform.position, m_Collider.radius * transform.localScale.x,
                sOverlapCache, m_PlayerMask);

            for (int i = 0; i < count; ++i)
            {
                Damageable d = sOverlapCache[i].GetComponent<Damageable>();

                if (d != null)
                {
                    data.direction = d.transform.position - transform.position;
                    d.ApplyDamage(data);
                }
            }
        }
    }
}