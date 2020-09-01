using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class DamageZone : MonoBehaviour
    {
        public int damageAmount = 1;
        public bool stopCamera = false;

        private void Reset()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            var d = other.GetComponent<Damageable>();
            if (d == null)
                return;

            var msg = new Damageable.DamageMessage()
            {
                amount = damageAmount,
                damager = this,
                direction = Vector3.up,
                stopCamera = stopCamera
            };

            d.ApplyDamage(msg);
        }
    } 
}
