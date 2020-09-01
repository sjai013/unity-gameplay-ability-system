using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D.WorldBuilding
{
    [ExecuteInEditMode]
    public class SmartObject : MonoBehaviour
    {

        public float distance = 10;
        public float rotationOffset = 0;
        public GameObject prefab;

        void Update()
        {
            if (transform.childCount == 0) return;
            var step = 360f / transform.childCount;
            var dir = transform.forward;
            var rot = Quaternion.AngleAxis(step, transform.up);
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.position = (transform.position + (dir * distance));
                child.rotation = Quaternion.AngleAxis(rotationOffset, transform.up) * Quaternion.LookRotation(dir, transform.up);
                Vector3 center;
                var up = Ground(child, out center);
                child.position = center;
                child.rotation = Quaternion.AngleAxis(rotationOffset, transform.up) * Quaternion.LookRotation(dir, up);
                dir = rot * dir;
            }
        }

        Vector3 Ground(Transform child, out Vector3 center)
        {
            foreach (var c in child.GetComponentsInChildren<Collider>())
                c.enabled = false;
            try
            {

                var box = child.GetComponent<BoxCollider>();
                if (box == null)
                {
                    center = child.position;
                    return Vector3.up;
                }
                else
                {
                    var size = (box.size / 2) - box.center;
                    var x = size.x;
                    var y = size.y;
                    var z = size.z;
                    var backLeft = child.TransformPoint(new Vector3(-x, -y, -z));
                    var backRight = child.TransformPoint(new Vector3(+x, -y, -z));
                    var frontLeft = child.TransformPoint(new Vector3(-x, -y, +z));
                    var frontRight = child.TransformPoint(new Vector3(+x, -y, +z));
                    RaycastHit hit;
                    var up = Vector3.up;
                    var mid = child.position;
                    if (Physics.Raycast(backLeft + Vector3.up * 100, Vector3.down, out hit))
                    {
                        up = Vector3.Lerp(up, hit.normal, 0.5f);
                        mid = Vector3.Lerp(mid, hit.point, 0.5f);
                    }
                    if (Physics.Raycast(backRight + Vector3.up * 100, Vector3.down, out hit))
                    {
                        up = Vector3.Lerp(up, hit.normal, 0.5f);
                        mid = Vector3.Lerp(mid, hit.point, 0.5f);
                    }
                    if (Physics.Raycast(frontLeft + Vector3.up * 100, Vector3.down, out hit))
                    {
                        up = Vector3.Lerp(up, hit.normal, 0.5f);
                        mid = Vector3.Lerp(mid, hit.point, 0.5f);
                    }
                    if (Physics.Raycast(frontRight + Vector3.up * 100, Vector3.down, out hit))
                    {
                        up = Vector3.Lerp(up, hit.normal, 0.5f);
                        mid = Vector3.Lerp(mid, hit.point, 0.5f);
                    }

                    center = mid;
                    return up.normalized;
                }
            }
            finally
            {
                foreach (var c in child.GetComponentsInChildren<Collider>())
                    c.enabled = true;
            }
        }


    }
}
