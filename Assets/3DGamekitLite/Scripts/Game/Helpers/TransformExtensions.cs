using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public static class TransformExtensions
    {
        public static void SetRotationWithXY(this Transform transform, Vector3 x, Vector3 y)
        {
            Matrix4x4 mat = Matrix4x4.identity;
            mat.SetColumn(0, x);
            mat.SetColumn(1, y);
            mat.SetColumn(2, Vector3.Cross(x, y));

            transform.rotation = mat.rotation;
        }

        public static void SetRotationWithXZ(this Transform transform, Vector3 x, Vector3 z)
        {
            Matrix4x4 mat = Matrix4x4.identity;
            mat.SetColumn(0, x);
            mat.SetColumn(1, Vector3.Cross(z, x));
            mat.SetColumn(2, z);

            transform.rotation = mat.rotation;
        }

        public static void SetRotationWithYZ(this Transform transform, Vector3 y, Vector3 z)
        {
            Matrix4x4 mat = Matrix4x4.identity;
            mat.SetColumn(0, Vector3.Cross(y, z));
            mat.SetColumn(1, y);
            mat.SetColumn(2, z);

            transform.rotation = mat.rotation;
        }
    } 
}
