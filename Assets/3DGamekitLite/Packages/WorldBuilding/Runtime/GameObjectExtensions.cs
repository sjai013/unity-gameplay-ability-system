using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamekit3D.WorldBuilding
{
    public static class GameObjectExtensions
    {
        public static GameObject Dummy(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;
            var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            var dummy = new GameObject(gameObject.name);
            dummy.transform.position = gameObject.transform.position;
            dummy.transform.rotation = gameObject.transform.rotation;
            foreach (var i in renderers)
            {
                var child = new GameObject(i.gameObject.name);
                child.transform.parent = dummy.transform;
                child.transform.position = i.transform.position;
                child.transform.rotation = i.transform.rotation;
                child.transform.localScale = i.transform.lossyScale;
                child.AddComponent<MeshFilter>().sharedMesh = i.GetComponent<MeshFilter>().sharedMesh;
                child.AddComponent<MeshRenderer>().sharedMaterial = i.sharedMaterial;
            }
            return dummy;
        }

        public static Bounds GetRendererBounds(this GameObject gameObject)
        {
            var bounds = new Bounds();
            var firstBounds = true;
            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (firstBounds)
                {
                    bounds = r.bounds;
                    firstBounds = false;
                }
                bounds.Encapsulate(r.bounds);
            }
            return bounds;
        }

        public static Bounds GetMeshBounds(this GameObject gameObject)
        {
            var bounds = new Bounds();
            var firstBounds = true;
            foreach (var r in gameObject.GetComponentsInChildren<MeshFilter>())
            {
                if (firstBounds)
                {
                    bounds = r.sharedMesh.bounds;
                    firstBounds = false;
                }
                bounds.Encapsulate(r.sharedMesh.bounds);
            }
            return bounds;
        }

        public static Bounds GetLocalBounds(this GameObject gameObject)
        {
            var rotation = gameObject.transform.rotation;
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            var bounds = new Bounds(gameObject.transform.position, Vector3.zero);
            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
                bounds.Encapsulate(renderer.bounds);
            bounds.center = bounds.center - gameObject.transform.position;
            gameObject.transform.rotation = rotation;
            return bounds;
        }
    }
}