using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace Samples.Editor.General
{
    public class DataPersistenceWindow : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/Examples/General/Data Persistence (Editor)")]
        public static void OpenWindow()
        {
            var window = GetWindow<DataPersistenceWindow>("Data Persistence");
            window.minSize = window.maxSize = new Vector2(1026, 372);
            EditorGUIUtility.PingObject(MonoScript.FromScriptableObject(window));
        }

        [SerializeField] private VisualTreeAsset uxmlAsset = default;
        [SerializeField] private Texture backgroundImage = default;

        public void OnEnable()
        {
            uxmlAsset.CloneTree(rootVisualElement);

            var backgrounds = rootVisualElement.Query<ScrollView>(className: "scrollview");
            backgrounds.ForEach(scrollview => scrollview.Add(new Image {image = backgroundImage}));

            var csScrollview = rootVisualElement.Q<ScrollView>("cs-persistence-scrollview");
            csScrollview.viewDataKey = "csPersistence";
        }
    }
}
