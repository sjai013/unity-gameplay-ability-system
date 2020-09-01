using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;


namespace Gamekit3D.WorldBuilding
{

    [CustomEditor(typeof(InstancePainter))]
    public partial class InstancePainterEditor : Editor
    {
        Texture2D[] palleteImages;
        GameObject stamp;

        List<GameObject> erase = new List<GameObject>();
        Vector3 worldCursor;
        InstancePainter ip;
        Variations variations;
        Editor variationsEditor;
        List<Bounds> overlaps = new List<Bounds>();
        List<GameObject> overlappedGameObjects = new List<GameObject>();

        void OnEnable()
        {
            stamp = new GameObject("Stamp");
            stamp.hideFlags = HideFlags.HideAndDontSave;
            ip = target as InstancePainter;
            if (ip.SelectedPrefab != null)
            {
                variations = ip.SelectedPrefab.GetComponent<Variations>();
                if (variationsEditor != null)
                    DestroyImmediate(variationsEditor);
                if (variations != null)
                    variationsEditor = Editor.CreateEditor(variations);
                CreateNewStamp();
            }

        }

        void OnDisable()
        {
            if (variationsEditor != null) DestroyImmediate(variationsEditor);
            if (stamp != null)
                DestroyImmediate(stamp);
        }

        void CreateNewStamp()
        {
            while (stamp.transform.childCount > 0)
                DestroyImmediate(stamp.transform.GetChild(0).gameObject);

            var count = Mathf.Min(1000, (Mathf.PI * Mathf.Pow(ip.brushRadius, 2)) / (1f / ip.brushDensity));

            for (var i = 0; i < count; i++)
            {
                var child = new GameObject("Dummy");
                child.transform.parent = stamp.transform;
                var p = Random.insideUnitCircle;
                child.transform.localPosition = new Vector3(p.x, 0, p.y) * ip.brushRadius;
                var eulerAngles = Vector3.zero;
                if (ip.maxRandomRotation > 0)
                {
                    eulerAngles.y = Random.value * ip.maxRandomRotation;
                    if (ip.rotationStep > 0)
                        eulerAngles.y = Mathf.Round(eulerAngles.y / ip.rotationStep) * ip.rotationStep;
                }
                child.transform.localEulerAngles = eulerAngles;
                GameObject dummy;
                if (variations != null)
                    dummy = PrefabUtility.InstantiatePrefab(variations.gameObjects[Random.Range(0, variations.gameObjects.Count)]) as GameObject;
                else
                    dummy = PrefabUtility.InstantiatePrefab(ip.SelectedPrefab) as GameObject;
                foreach (var c in dummy.GetComponentsInChildren<Collider>())
                    c.enabled = false;
                dummy.transform.parent = child.transform;
                if (variations != null)
                    child.transform.localScale = Vector3.one * Mathf.Lerp(variations.minScale, variations.maxScale, Random.value);
                dummy.transform.localPosition = Vector3.zero;
                dummy.transform.localRotation = Quaternion.identity;
            }

            var toDestroy = new HashSet<GameObject>();
            for (var i = 0; i < stamp.transform.childCount; i++)
            {
                var child = stamp.transform.GetChild(i);
                if (toDestroy.Contains(child.gameObject)) continue;


                var bounds = child.gameObject.GetRendererBounds();
                var childVolume = bounds.size.x * bounds.size.y * bounds.size.z;
                for (var x = 0; x < stamp.transform.childCount; x++)
                {
                    var check = stamp.transform.GetChild(x);
                    if (check.gameObject == child.gameObject) continue;
                    if (toDestroy.Contains(check.gameObject)) continue;
                    var b = check.gameObject.GetRendererBounds();
                    if (b.Intersects(bounds))
                    {
                        var overlapVolume = b.size.x * b.size.y * b.size.z;
                        var intersection = Intersection(b, bounds);
                        var intersectionVolume = intersection.size.x * intersection.size.y * intersection.size.z;
                        // Handles.DrawWireCube(intersection.center, intersection.size);
                        var maxIntersection = Mathf.Max(intersectionVolume / overlapVolume, intersectionVolume / childVolume);
                        // Handles.Label(intersection.center, maxIntersection.ToString());
                        if (maxIntersection > ip.maxIntersectionVolume)
                        {
                            toDestroy.Add(child.gameObject);
                            break;
                        }
                    }
                }
            }
            foreach (var i in toDestroy)
            {
                DestroyImmediate(i);
            }
        }

        void PerformErase()
        {
            foreach (var g in erase)
                Undo.DestroyObjectImmediate(g);
            erase.Clear();
        }

        void PerformStamp()
        {
            var removeVariations = ip.SelectedPrefab.GetComponent<Variations>() != null;
            for (var i = 0; i < stamp.transform.childCount; i++)
            {
                var dummy = stamp.transform.GetChild(i);
                if (dummy.gameObject.activeSelf)
                {
                    var stampObject = dummy.transform.GetChild(0);
                    var prefab = PrefabUtility.GetCorrespondingObjectFromSource(stampObject.gameObject);
                    var g = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    Undo.RegisterCreatedObjectUndo(g, "Stamp");
                    if (removeVariations) DestroyImmediate(g.GetComponent<Variations>());
                    g.transform.position = stampObject.position;
                    g.transform.rotation = stampObject.rotation;
                    g.transform.localScale = stampObject.lossyScale;
                    if (ip.rootTransform != null)
                    {
                        g.transform.parent = ip.rootTransform;
                        g.isStatic = ip.rootTransform.gameObject.isStatic;
                        g.layer = ip.rootTransform.gameObject.layer;
                    }
                }
            }
            if (ip.randomizeAfterStamp)
                CreateNewStamp();
        }

        void RotateStamp(Vector2 delta)
        {
            var rotation = Quaternion.AngleAxis(delta.x, Vector3.up);
            foreach (Transform t in stamp.transform)
            {
                t.localPosition = rotation * t.localPosition;
            }
        }

        void AdjustMaxScale(float s)
        {
            for (var i = 0; i < stamp.transform.childCount; i++)
                stamp.transform.GetChild(i).localScale *= s;
        }

    }
}