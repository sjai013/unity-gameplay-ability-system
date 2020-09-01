using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    [CustomEditor(typeof(CheckpointGroup))]
    public class CheckpointGroupEditor : Editor
    {
        public void OnSceneGUI()
        {
            var cpg = target as CheckpointGroup;
            var children = cpg.GetComponentsInChildren<Checkpoint>();
            for (var i = 0; i < children.Length; i++)
            {
                if (Tools.current == Tool.Scale)
                {
                    var sc = children[i].GetComponent<SphereCollider>();
                    if (sc != null)
                    {
                        var pos = children[i].transform.position;
                        using (var cc = new EditorGUI.ChangeCheckScope())
                        {
                            var radius = Handles.RadiusHandle(Quaternion.identity, pos, sc.radius);
                            if (cc.changed)
                            {
                                Undo.RecordObject(sc, "Change Radius");
                                sc.radius = radius;
                            }
                        }
                    }
                }
                else
                {
                    var pos = children[i].transform.position;
                    using (var cc = new EditorGUI.ChangeCheckScope())
                    {
                        pos = Handles.PositionHandle(pos, Quaternion.identity);
                        if (cc.changed)
                        {
                            Undo.RecordObject(children[i].transform, "Change Position");
                            children[i].transform.position = pos;
                        }
                    }
                }
            }

            if (Event.current.control && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                var sp = Event.current.mousePosition;
                sp.y = Screen.height - sp.y;
                var ray = Camera.current.ScreenPointToRay(sp);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var g = new GameObject("Checkpoint (" + children.Length + ")", typeof(SphereCollider), typeof(Checkpoint));
                    Undo.RegisterCreatedObjectUndo(g, "Create Checkpoint");
                    g.transform.position = hit.point;
                    g.transform.parent = cpg.transform;
                    var sc = g.GetComponent<SphereCollider>();
                    sc.radius = 5;
                    sc.isTrigger = true;
                }
                Event.current.Use();
            }
        }

    } 
}
