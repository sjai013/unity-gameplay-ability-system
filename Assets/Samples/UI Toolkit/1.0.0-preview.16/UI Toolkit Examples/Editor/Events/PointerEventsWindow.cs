using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Editor.Events
{
    public class PointerEventsWindow : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/Examples/Events/Pointer Events (Editor)")]
        public static void OpenWindow()
        {
            var window = GetWindow<PointerEventsWindow>("Pointer Events");
            window.minSize = window.maxSize = new Vector2(460, 580);
            EditorGUIUtility.PingObject(MonoScript.FromScriptableObject(window));
        }

        [SerializeField] private VisualTreeAsset uxmlAsset = default;

        private VisualElement mainArea = default;

        private Label tooltip;
        private readonly Vector2 tooltipOffset = new Vector2(10, 10);

        private Label coordinatesLabel;

        public void OnEnable()
        {
            uxmlAsset.CloneTree(rootVisualElement);

            mainArea = rootVisualElement.Q<VisualElement>(className: "main-area");
            tooltip = rootVisualElement.Q<Label>(className: "main-area__tooltip");
            coordinatesLabel = rootVisualElement.Q<Label>(className: "info-block__label--data");
            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            mainArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
            mainArea.RegisterCallback<PointerUpEvent>(OnPointerUp);
            mainArea.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            tooltip.text = "PointerDown!";
            UpdateTooltipPosition(evt.localPosition);
            mainArea.AddToClassList("main-area--active");
            mainArea.CapturePointer(evt.pointerId);
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            mainArea.CapturePointer(evt.pointerId);
            mainArea.ReleaseMouse();
            tooltip.text = "PointerUp!";
            UpdateTooltipPosition(evt.localPosition);
            mainArea.RemoveFromClassList("main-area--active");
            mainArea.schedule.Execute(() =>
            {
                if (string.CompareOrdinal(tooltip.text, "PointerUp!") == 0)
                {
                    tooltip.text = string.Empty;
                }
            }).StartingIn(1000);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            coordinatesLabel.text = $"({(int)evt.localPosition.x}, {(int)evt.localPosition.y})";
            if (mainArea.panel.GetCapturingElement(evt.pointerId) == evt.target)
            {
                UpdateTooltipPosition(evt.localPosition);
            }
        }

        private void UpdateTooltipPosition(Vector2 localPosition)
        {
            tooltip.style.left = Mathf.Clamp(localPosition.x + tooltipOffset.x,
                0, mainArea.contentRect.width - tooltip.contentRect.width);
            tooltip.style.top = Mathf.Clamp(localPosition.y + tooltipOffset.y,
                0, mainArea.contentRect.height - tooltip.contentRect.height);
        }
    }
}
