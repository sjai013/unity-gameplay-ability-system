using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System;
using System.Linq;

namespace Gamekit3D.WorldBuilding
{

    public partial class InstancePainterEditor : Editor
    {

        void OnSceneGUI()
        {
            SceneView.RepaintAll();
            //if (ip == null || ip.SelectedPrefab == null) return;
            var isErasing = Event.current.control;
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            var mousePos = Event.current.mousePosition;

            var ray = HandleUtility.GUIPointToWorldRay(mousePos);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, ip.layerMask))
            {
                worldCursor = hit.point;
                var up = ip.followOnSurface ? hit.normal : Vector3.up;
                Handles.color = isErasing ? Color.red : Color.white;
                Handles.DrawWireDisc(worldCursor, up, ip.brushRadius);
                Handles.color = Color.white * 0.5f;
                Handles.DrawWireDisc(worldCursor + up * ip.brushHeight, up, ip.brushRadius);
                Handles.DrawWireDisc(worldCursor - up * ip.brushHeight, up, ip.brushRadius);
                OverlapCapsule(worldCursor + hit.normal * 10, worldCursor - hit.normal * 10, ip.brushRadius, ip.layerMask);
                if (isErasing)
                    DrawEraser(worldCursor, hit.normal);
                else
                    DrawStamp(worldCursor, hit.normal);
            }

            switch (Event.current.type)
            {
                case EventType.ScrollWheel:
                    if (Event.current.shift)
                    {
                        RotateStamp(Event.current.delta);
                        Event.current.Use();
                    }
                    if (Event.current.alt)
                    {
                        ip.brushRadius *= Event.current.delta.y < 0 ? 0.9f : 1.1f;
                        CreateNewStamp();
                        Event.current.Use();
                    }
                    break;
                case EventType.KeyDown:
                    HandleKey(Event.current.keyCode);
                    break;
                case EventType.MouseDown:
                    //If not using the default orbit mode...
                    if (Event.current.button == 0 && !Event.current.alt)
                    {
                        if (isErasing)
                            PerformErase();
                        else
                            PerformStamp();
                        GUIUtility.hotControl = controlId;
                        Event.current.Use();
                    }
                    break;
            }
        }

        private void OverlapCapsule(Vector3 top, Vector3 bottom, float brushRadius, LayerMask layerMask)
        {
            overlaps.Clear();
            overlappedGameObjects.Clear();
            if (ip.collisionTest == InstancePainter.CollisionTest.ColliderBounds)
            {
                foreach (var c in Physics.OverlapCapsule(top, bottom, brushRadius))
                {
                    if (c.transform.parent == ip.rootTransform)
                    {
                        overlaps.Add(c.bounds);
                        overlappedGameObjects.Add(c.gameObject);
                    }
                }
            }
            if (ip.collisionTest == InstancePainter.CollisionTest.RendererBounds)
            {
                //TODO: This might need an oct-tree later. Brute force for now.
                var capsule = new Bounds(Vector3.Lerp(top, bottom, 0.5f), new Vector3(brushRadius * 2, brushRadius * 2 + (top - bottom).magnitude, brushRadius * 2));
                for (var i = 0; i < ip.rootTransform.childCount; i++)
                {
                    var child = ip.rootTransform.GetChild(i);
                    var bounds = child.gameObject.GetRendererBounds();
                    if (capsule.Intersects(bounds))
                    {
                        overlaps.Add(bounds);
                        overlappedGameObjects.Add(child.gameObject);
                    }
                }
            }

        }

        void HandleKey(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Period:
                    AdjustMaxScale(0.9f);
                    break;
                case KeyCode.Slash:
                    AdjustMaxScale(1.1f);
                    break;
                case KeyCode.Minus:
                    ip.brushDensity *= 0.9f;
                    CreateNewStamp();
                    Event.current.Use();
                    break;
                case KeyCode.Equals:
                    ip.brushDensity *= 1.1f;
                    CreateNewStamp();
                    Event.current.Use();
                    break;

                case KeyCode.Space:
                    CreateNewStamp();
                    Event.current.Use();
                    break;
                case KeyCode.LeftBracket:
                    ip.brushRadius *= 0.9f;
                    CreateNewStamp();
                    Event.current.Use();
                    break;
                case KeyCode.RightBracket:
                    ip.brushRadius *= 1.1f;
                    CreateNewStamp();
                    Event.current.Use();
                    break;

            }
        }

        void DrawStamp(Vector3 center, Vector3 normal)
        {
            stamp.transform.position = center;

            if (ip.followOnSurface)
            {
                var tangent = Vector3.Cross(normal, Vector3.forward);
                if (tangent.magnitude == 0)
                    tangent = Vector3.Cross(normal, Vector3.up);
                if (normal.magnitude < Mathf.Epsilon || tangent.magnitude < Mathf.Epsilon) return;
                stamp.transform.rotation = Quaternion.LookRotation(tangent, normal);
            }
            else
            {
                stamp.transform.rotation = Quaternion.identity;
            }

            for (var i = 0; i < stamp.transform.childCount; i++)
            {
                var child = stamp.transform.GetChild(i);
                child.localPosition = Vector3.Scale(child.localPosition, new Vector3(1, 0, 1));
                RaycastHit hit;
                if (Physics.Raycast(child.position + (child.up * ip.brushHeight), -child.up, out hit, ip.brushHeight * 2, ip.layerMask))
                {
                    var slope = Vector3.Angle(normal, hit.normal);
                    if (slope > ip.maxSlope)
                    {
                        child.gameObject.SetActive(false);
                        continue;
                    }
                    child.gameObject.SetActive(true);
                    var dummy = child.GetChild(0);
                    dummy.position = hit.point;
                    if (ip.alignToNormal)
                    {
                        var tangent = Vector3.Cross(hit.normal, child.forward);
                        if (tangent.magnitude == 0)
                            tangent = Vector3.Cross(hit.normal, child.up);
                        dummy.rotation = Quaternion.LookRotation(tangent, hit.normal);
                    }
                    else
                        dummy.rotation = Quaternion.LookRotation(child.forward, child.up);

                    var bounds = child.gameObject.GetRendererBounds();
                    var childVolume = bounds.size.x * bounds.size.y * bounds.size.z;
                    foreach (var b in overlaps)
                    {
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
                                child.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    child.gameObject.SetActive(false);
                }

            }

        }

        Bounds Intersection(Bounds A, Bounds B)
        {
            var min = new Vector3(Mathf.Max(A.min.x, B.min.x), Mathf.Max(A.min.y, B.min.y), Mathf.Max(A.min.z, B.min.z));
            var max = new Vector3(Mathf.Min(A.max.x, B.max.x), Mathf.Min(A.max.y, B.max.y), Mathf.Min(A.max.z, B.max.z));
            return new Bounds(Vector3.Lerp(min, max, 0.5f), max - min);
        }

        void DrawEraser(Vector3 center, Vector3 normal)
        {
            erase.Clear();
            for (var i = 0; i < stamp.transform.childCount; i++)
                stamp.transform.GetChild(i).gameObject.SetActive(false);

            stamp.transform.position = center;
            if (ip.followOnSurface)
            {
                var tangent = Vector3.Cross(normal, Vector3.forward);
                if (tangent.magnitude == 0)
                    tangent = Vector3.Cross(normal, Vector3.up);
                if (normal.magnitude < Mathf.Epsilon || tangent.magnitude < Mathf.Epsilon) return;
                stamp.transform.rotation = Quaternion.LookRotation(tangent, normal);
            }
            else
            {
                stamp.transform.rotation = Quaternion.identity;
            }

            for (var i = 0; i < overlaps.Count; i++)
            {
                var h = overlaps[i];
                Handles.color = Color.red;
                Handles.DrawWireDisc(h.center, Vector3.up, h.extents.magnitude);
                erase.Add(overlappedGameObjects[i]);
            }

        }

        public override bool RequiresConstantRepaint() { return true; }
    }

}