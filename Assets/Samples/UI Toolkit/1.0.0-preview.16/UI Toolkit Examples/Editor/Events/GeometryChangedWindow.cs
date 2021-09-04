using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Editor.Events
{
    public class GeometryChangedWindow : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/Examples/Events/Geometry Changed (Editor)")]
        public static void OpenWindow()
        {
            var window = GetWindow<GeometryChangedWindow>("Geometry Changed Sample");
            window.minSize = new Vector2(200, 170);
            EditorGUIUtility.PingObject(MonoScript.FromScriptableObject(window));
        }

        [SerializeField] private VisualTreeAsset uxmlAsset = default;

        private Label label;

        public void OnEnable()
        {
            uxmlAsset.CloneTree(rootVisualElement);

            var menu = rootVisualElement.Q<VisualElement>(className: "menu");
            label = rootVisualElement.Q<Label>(className: "container__label");
            menu.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            string oldWidthType = GetWidthType(evt.oldRect.width);
            string newWidthType = GetWidthType(evt.newRect.width);

            if (oldWidthType != newWidthType)
            {
                var menu = (VisualElement)evt.target;
                menu.RemoveFromClassList($"menu--{oldWidthType}");
                menu.AddToClassList($"menu--{newWidthType}");
                label.text = $"The format is {newWidthType.ToUpper()}.";
            }
        }

        private string GetWidthType(float width)
        {
            string widthType;
            if ((int)width <= 0)
            {
                widthType = "none";
            }
            else if ((int)width <= 300)
            {
                widthType = "narrow";
            }
            else if ((int)width <= 600)
            {
                widthType = "medium";
            }
            else
            {
                widthType = "wide";
            }

            return widthType;
        }
    }
}
