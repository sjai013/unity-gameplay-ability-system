using Samples.Runtime.Events;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Samples.Utils
{
    public static partial class MenuItems
    {
        [MenuItem("Window/UI Toolkit/Examples/Events/ClickEvent (Runtime)")]
        public static void StartRuntimeEventClickEvent()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            var go = new GameObject("Whack-A-Button");
            var component = go.AddComponent<ClickEventSample>();
            EditorApplication.EnterPlaymode();
            EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour(component));
        }
    }
}
