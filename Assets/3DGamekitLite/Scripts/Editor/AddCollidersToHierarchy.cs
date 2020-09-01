using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gamekit3D
{
    public class AddCollidersToHierarchy : MonoBehaviour
    {

        [MenuItem("GameObject/Add Colliders To Children")]
        static void AddColliders()
        {

            var parent = Selection.activeGameObject;
            if (parent != null)
            {
                var lodComponents = parent.GetComponentsInChildren<LODGroup>();
                foreach (var lodg in lodComponents)
                {
                    var lods = lodg.GetLODs();
                    var lastLod = lods[lods.Length - 1];
                    var existingColliders = lodg.gameObject.GetComponents<MeshCollider>();
                    foreach (var ec in existingColliders) DestroyImmediate(ec);
                    foreach (var renderer in lastLod.renderers)
                    {
                        var mc = lodg.gameObject.AddComponent<MeshCollider>();
                        mc.sharedMesh = renderer.GetComponent<MeshFilter>().sharedMesh;
                    }

                }
            }
        }
    } 
}
