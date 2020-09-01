using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class SoundTrackVolume : MonoBehaviour
    {
        public LayerMask layers;
        SoundTrack soundTrack;

        void OnEnable()
        {
            soundTrack = GetComponentInParent<SoundTrack>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
                soundTrack.PushTrack(this.name);
        }

        void OnTriggerExit(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
                soundTrack.PopTrack();
        }

    }
}
