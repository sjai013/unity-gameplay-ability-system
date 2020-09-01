using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gamekit3D.GameCommands
{

    [CustomEditor(typeof(SimpleTranslator), true)]
    public class SimpleTranslatorEditor : SimpleTransformerEditor
    {

        void OnSceneGUI()
        {
            var t = target as SimpleTranslator;
            var start = t.transform.TransformPoint(t.start);
            var end = t.transform.TransformPoint(t.end);
            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                start = Handles.PositionHandle(start, Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);
                Handles.Label(start, "Start", "button");
                Handles.Label(end, "End", "button");
                end = Handles.PositionHandle(end, t.transform.rotation);
                if (cc.changed)
                {
                    Undo.RecordObject(t, "Move Handles");
                    t.start = t.transform.InverseTransformPoint(start);
                    t.end = t.transform.InverseTransformPoint(end);
                }
            }
            Handles.color = Color.yellow;
            Handles.DrawDottedLine(start, end, 5);
            Handles.Label(Vector3.Lerp(start, end, 0.5f), "Distance:" + (end - start).magnitude);
        }

    }

}
