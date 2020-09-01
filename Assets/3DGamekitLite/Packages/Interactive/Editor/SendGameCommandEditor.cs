using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Gamekit3D.GameCommands
{

    [SelectionBase]
    [CustomEditor(typeof(SendGameCommand), true)]
    public class SendGameCommandEditor : Editor
    {
        void OnSceneGUI()
        {
            var si = target as SendGameCommand;
            if (si.interactiveObject != null)
            {
                DrawInteraction(si);
            }
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Pickable | GizmoType.NotInSelectionHierarchy)]
        static void DrawConnectionGizmo(SendGameCommand sgc, GizmoType gizmoType)
        {
            if (sgc.interactiveObject != null)
            {
                var start = sgc.transform.position;
                var end = sgc.interactiveObject.transform.position;
                if (end == start) end += sgc.interactiveObject.transform.forward * 1;
                var dir = (end - start).normalized;
                if (Application.isPlaying)
                    Handles.color = Color.Lerp(Color.white, Color.green, sgc.Temperature);
                else
                    Handles.color = new Color(1, 1, 1, 0.25f);
                Handles.DrawDottedLine(start, end, 5);
                Handles.ArrowHandleCap(0, start + (dir * 2), Quaternion.LookRotation(dir), 1, EventType.Repaint);
            }
        }

        public static void DrawInteraction(SendGameCommand si)
        {
            var start = si.transform.position;
            var end = si.interactiveObject.transform.position;
            var dir = (end - start).normalized;
            
            if (Application.isPlaying)
                Handles.color = Color.Lerp(Color.white, Color.green, si.Temperature);
            var steps = Mathf.FloorToInt((end - start).magnitude);
            for (var i = 0; i < steps; i++)
            {
                Handles.ArrowHandleCap(0, start + (dir * i), Quaternion.LookRotation(dir), 1, EventType.Repaint);
            }
        }
    }
}
