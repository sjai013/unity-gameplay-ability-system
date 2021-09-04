using Samples.Runtime.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Utils
{
    public static partial class MenuItems
    {
        [MenuItem("Window/UI Toolkit/Examples/Text/Alignment Tags (Runtime)")]
        public static void StartRuntimeTextAlignmentTags()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            InitializeTextAlignmentTagsScene();
        }

        static void InitializeTextAlignmentTagsScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            var go = new GameObject("Sample Object");
            var component = go.AddComponent<AlignmentTags>();
            EditorApplication.EnterPlaymode();
            EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour(component));
        }
    }
}
