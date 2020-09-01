using System;
using UnityEngine;

namespace Gamekit3D.GameCommands
{

    public class SimpleRotator : SimpleTransformer
    {
        public Vector3 axis = Vector3.forward;
        public float start = 0;
        public float end = 90;


        public override void PerformTransform(float position)
        {
            var curvePosition = accelCurve.Evaluate(position);
            var q = Quaternion.AngleAxis(Mathf.Lerp(start, end, curvePosition), axis);
            transform.localRotation = q;
        }


    }
}
