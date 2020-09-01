using System;
using System.Collections;
using UnityEngine;

namespace Gamekit3D.GameCommands
{

    [RequireComponent(typeof(Collider))]
    public class Teleporter : MonoBehaviour
    {
        new public Collider collider;
        public LayerMask layers;

        public GameObject enterEffect;
        public GameObject exitEffect;
        public Transform destinationTransform;
        public float delayTime;

        WaitForSeconds delay;

        IEnumerator Activate(GameObject teleportee)
        {
            if (destinationTransform)
            {
                foreach (var i in teleportee.GetComponentsInChildren<OnTeleportEvent>())
                    i.OnTeleport(this);
                if (enterEffect) enterEffect.SetActive(true);
                yield return delay;
                if (exitEffect) exitEffect.SetActive(true);
                teleportee.transform.position = destinationTransform.position;
                teleportee.transform.rotation = destinationTransform.rotation;
            }
        }

        void Reset()
        {
            collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        void Awake()
        {
            delay = new WaitForSeconds(delayTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsTeleportable(other))
                StartCoroutine(Activate(other.gameObject));
        }

        bool IsTeleportable(Collider other)
        {
            return 0 != (layers.value & 1 << other.gameObject.layer);
        }

    }


}