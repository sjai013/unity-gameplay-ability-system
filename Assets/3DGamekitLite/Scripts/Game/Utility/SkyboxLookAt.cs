using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class SkyboxLookAt : MonoBehaviour
    {

        public Transform target;

        void Update()
        {
            transform.LookAt(target);
        }
    } 
}
