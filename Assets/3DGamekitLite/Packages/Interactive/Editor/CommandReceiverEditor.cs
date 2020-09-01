using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gamekit3D.GameCommands
{
    [SelectionBase]
    [CustomEditor(typeof(GameCommandReceiver), true)]
    public class CommandReceiverEditor : Editor
    {
        List<SendGameCommand> senders = new List<SendGameCommand>();

        void OnEnable()
        {
            var interactive = target as GameCommandReceiver;
            senders.Clear();
            foreach (SendGameCommand si in Resources.FindObjectsOfTypeAll(typeof(SendGameCommand)))
            {
                if (si.interactiveObject == interactive) senders.Add(si);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginVertical("box");
            GUILayout.Label("Senders");
            foreach (var i in senders)
                EditorGUILayout.ObjectField(i, typeof(SendGameCommand), true);
            GUILayout.EndVertical();
        }

        void OnSceneGUI()
        {
            foreach (var i in senders)
            {
                SendGameCommandEditor.DrawInteraction(i);
            }
        }



    }

}
