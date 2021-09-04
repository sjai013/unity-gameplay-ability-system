using Samples.Runtime.Events;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Samples.Utils
{
    public static partial class MenuItems
    {
        [MenuItem("Window/UI Toolkit/Examples/Events/Keyboard Events (Runtime)")]
        public static void StartRuntimeEventsKeyboardEvents()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            var go = new GameObject("Sample Object");
            var component = go.AddComponent<KeyboardEvents>();
            EditorApplication.EnterPlaymode();
            EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour(component));
        }
    }
}
