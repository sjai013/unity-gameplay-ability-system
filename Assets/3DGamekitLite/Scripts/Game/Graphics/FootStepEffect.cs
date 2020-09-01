using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class FootStepEffect : MonoBehaviour
    {

        public Transform rayStart;
        public Transform particleOffset;
        public ParticleSystem PS;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            RaycastHit hit;
            if (!Physics.Raycast(rayStart.position, -Vector3.up, out hit))
                return;

            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return;

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Color[] colors = mesh.colors;
            int[] triangles = mesh.triangles;
            Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
            Transform hitTransform = hit.collider.transform;
            p0 = hitTransform.TransformPoint(p0);
            p1 = hitTransform.TransformPoint(p1);
            p2 = hitTransform.TransformPoint(p2);
            Debug.DrawLine(p0, p1);
            Debug.DrawLine(p1, p2);
            Debug.DrawLine(p2, p0);

            //closest point
            Vector3 closest = p0;
            int vertNum = 0;
            if (Vector3.Distance(hit.point, p1) < Vector3.Distance(hit.point, closest))
            {
                closest = p1;
                vertNum = 1;
            }

            if (Vector3.Distance(hit.point, p2) < Vector3.Distance(hit.point, closest))
            {
                closest = p2;
                vertNum = 2;
            }
            var emission = PS.emission;

            Color col = colors[triangles[hit.triangleIndex * 3 + vertNum]];
            if (col.a < 0.5)
            {
                Debug.DrawLine(closest, new Vector3(closest.x, closest.y + 1, closest.z), Color.red);
                particleOffset.localPosition = new Vector3(0, hit.point.y - this.transform.position.y, 0);
                emission.enabled = true;
            }
            else
            {
                emission.enabled = false;
            }




        }
    } 
}
