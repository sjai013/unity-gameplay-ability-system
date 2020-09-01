using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D.GameCommands
{

    public class SendOnTriggerStay : TriggerCommand
    {
        public LayerMask layers;

        void OnTriggerStay(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
            {
                Send();
            }
        }
    }

}
