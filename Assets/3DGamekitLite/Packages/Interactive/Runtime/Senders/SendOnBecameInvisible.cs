using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D.GameCommands
{

    public class SendOnBecameInvisible : SendGameCommand
    {
        void OnBecameInvisible()
        {
            Send();
        }
    }
}
