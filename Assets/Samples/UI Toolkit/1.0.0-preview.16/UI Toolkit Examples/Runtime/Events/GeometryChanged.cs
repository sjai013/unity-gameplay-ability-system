using UnityEngine.UIElements;
using UnityEngine;

namespace Samples.Runtime.Events
{
    [RequireComponent(typeof(UIDocument))]
    public class GeometryChanged : MonoBehaviour
    {
        [SerializeField] private PanelSettings panelSettings = default;
        [SerializeField] private VisualTreeAsset sourceAsset = default;
        [SerializeField] private StyleSheet styleAsset = default;

        private Label label;

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;
            uiDocument.visualTreeAsset = sourceAsset;
        }

        void OnEnable()
        {
            if (label == null)
            {
                //After a domain reload, we need to re-cache our VisualElements and hook our callbacks
                InitializeVisualTree(GetComponent<UIDocument>());
            }
        }

        private void InitializeVisualTree(UIDocument uiDocument)
        {
            VisualElement root = uiDocument.rootVisualElement;
            var menu = root.Q<VisualElement>(className: "menu");
            label = root.Q<Label>(className: "container__label");
            menu.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            root.styleSheets.Add(styleAsset);
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
