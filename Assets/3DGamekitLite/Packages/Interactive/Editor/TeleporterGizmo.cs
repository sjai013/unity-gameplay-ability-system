using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamekit3D.GameCommands
{
    public static class CommandGizmos
    {
        static GUIStyle sceneNote;

        static CommandGizmos()
        {
            sceneNote = new GUIStyle("box");
            sceneNote.fontStyle = FontStyle.Bold;
            sceneNote.normal.textColor = Color.white;
            sceneNote.margin = sceneNote.overflow = sceneNote.padding = new RectOffset(3, 3, 3, 3);
            sceneNote.richText = true;
            sceneNote.alignment = TextAnchor.MiddleLeft;
        }

        static void DrawNote(Vector3 position, string text, string warning = "", float distance = 10)
        {
            if (!string.IsNullOrEmpty(warning))
                text = text + "<color=red>" + warning + "</color>";
            if ((Camera.current.transform.position - position).magnitude <= distance)
                Handles.Label(position, text, sceneNote);
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy, typeof(Teleporter))]
        static void DrawTeleporterGizmos(Teleporter teleporter, GizmoType gizmoType)
        {
            if (teleporter.destinationTransform)
            {
                DrawNote(teleporter.transform.position, "Teleport Enter");
                Handles.color = Color.yellow * 0.5f;
                Handles.DrawDottedLine(teleporter.transform.position, teleporter.destinationTransform.position, 5);
                DrawNote(teleporter.destinationTransform.position, "Teleport Exit");
            }
            else
            {
                DrawNote(teleporter.transform.position, "Teleport Enter", "(No Destination!)");
            }
        }
    }
}