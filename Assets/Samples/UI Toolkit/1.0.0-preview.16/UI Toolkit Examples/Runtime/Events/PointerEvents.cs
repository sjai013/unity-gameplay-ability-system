using UnityEngine.UIElements;
using UnityEngine;

namespace Samples.Runtime.Events
{
    [RequireComponent(typeof(UIDocument))]
    public class PointerEvents : MonoBehaviour
    {
        [SerializeField] private PanelSettings panelSettings = default;
        [SerializeField] private VisualTreeAsset sourceAsset = default;
        [SerializeField] private StyleSheet ussAsset = default;

        private VisualElement mainArea;

        private Label tooltip;
        private readonly Vector2 tooltipOffset = new Vector2(10, 10);

        private Label coordinatesLabel;

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;
            uiDocument.visualTreeAsset = sourceAsset;
        }

        void OnEnable()
        {
            if (mainArea == null)
            {
                //After a domain reload, we need to re-cache our VisualElements and hook our callbacks
                InitializeVisualTree(GetComponent<UIDocument>());
            }
        }

        private void InitializeVisualTree(UIDocument doc)
        {
            var root = doc.rootVisualElement;

            root.styleSheets.Add(ussAsset);

            mainArea = root.Q<VisualElement>(className: "main-area");
            tooltip = root.Q<Label>(className: "main-area__tooltip");
            coordinatesLabel = root.Q<Label>(className: "info-block__label--data");
            RegisterCallbacks();

            root.styleSheets.Add(ussAsset);
        }

        private void RegisterCallbacks()
        {
            mainArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
            mainArea.RegisterCallback<PointerUpEvent>(OnPointerUp);
            mainArea.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            Debug.Log("PDE");
            tooltip.text = "PointerDown!";
            UpdateTooltipPosition(evt.localPosition);
            mainArea.AddToClassList("main-area--active");
            mainArea.CapturePointer(evt.pointerId);
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            mainArea.ReleasePointer(evt.pointerId);
            tooltip.text = "PointerUp!";
            UpdateTooltipPosition(evt.localPosition);
            mainArea.RemoveFromClassList("main-area--active");
            mainArea.schedule.Execute(() =>
            {
                if (tooltip.text == "PointerUp!")
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
