using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class Spinner : MonoBehaviour
    {
        public Vector3 axis = Vector3.up;
        public float speed = 1;

        void Update()
        {
            transform.Rotate(axis, speed * Time.deltaTime);
        }
    } 
}
