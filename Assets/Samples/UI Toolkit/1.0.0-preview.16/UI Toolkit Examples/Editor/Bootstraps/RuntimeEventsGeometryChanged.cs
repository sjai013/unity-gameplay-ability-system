using Samples.Runtime.Events;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Samples.Utils
{
    public static partial class MenuItems
    {
        [MenuItem("Window/UI Toolkit/Examples/Events/GeometryChanged (Runtime)")]
        public static void StartRuntimeEventsGeometryChanged()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            var go = new GameObject("Sample Object");
            var component = go.AddComponent<GeometryChanged>();
            EditorApplication.EnterPlaymode();
            EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour(component));
        }
    }
}
