using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Samples.Runtime.Rendering
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 rotationSpeed = Vector3.one;

        void Update()
        {
            var delta = rotationSpeed * Time.deltaTime;

            var quaternion = Quaternion.Euler(delta.x, delta.y, delta.z);

            transform.localRotation *= quaternion;
        }
    }
}
