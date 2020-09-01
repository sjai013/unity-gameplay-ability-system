using UnityEngine;
using UnityEditor;


namespace Gamekit3D
{
    public class AddObjectiveSubEditor : SubEditor<ScenarioController>
    {
        string newObjectiveName = "";
        bool showAdd = false;
        string addError = "";

        public override void OnInspectorGUI(ScenarioController sc)
        {

            if (!showAdd && GUILayout.Button("Add New Objective"))
            {
                showAdd = true;
                GUI.FocusControl("ObjectiveName");
            }
            if (showAdd)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.SetNextControlName("ObjectiveName");
                    GUILayout.Label("Objective Name");
                    newObjectiveName = EditorGUILayout.TextField(newObjectiveName).ToUpper();
                    using (new EditorGUI.DisabledScope(newObjectiveName == ""))
                    {
                        if (GUILayout.Button("Add"))
                        {
                            if (sc.AddObjective(newObjectiveName, 1))
                            {
                                Close();
                            }
                            else
                            {
                                addError = "This objective name already exists.";
                            }
                        }
                    }
                }
                if (addError != "")
                {
                    EditorGUILayout.HelpBox(addError, MessageType.Error);
                }
                if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
                {
                    Close();
                }
            }
        }

        void Close()
        {
            newObjectiveName = "";
            addError = "";
            showAdd = false;
            EditorGUIUtility.editingTextField = false;
            Repaint();
        }

    } 
}
