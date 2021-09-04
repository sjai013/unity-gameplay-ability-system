using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Runtime.Events
{
    [RequireComponent(typeof(UIDocument))]
    public class KeyboardEvents : MonoBehaviour
    {
        [SerializeField] private PanelSettings panelSettings = default;
        [SerializeField] private VisualTreeAsset sourceAsset = default;
        [SerializeField] private StyleSheet styleAsset = default;

        private Label label;

        private bool keyIsDown = false;
        private readonly uint[] timesInMillis = new uint[4];
        private int currentIndex;
        private int nextIndex;

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

        private void InitializeVisualTree(UIDocument doc)
        {
            var root = doc.rootVisualElement;
            label = root.Q<Label>(className: "display__bpm");

            var tapElement = root.Q<VisualElement>(className: "content-section__tap-box");
            tapElement.focusable = true;
            tapElement.pickingMode = PickingMode.Position;
            tapElement.RegisterCallback<BlurEvent>(_ => tapElement.Focus());
            tapElement.Focus();
            tapElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
            tapElement.RegisterCallback<KeyUpEvent>(OnKeyUp);

            root.styleSheets.Add(styleAsset);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Space || keyIsDown) return;
            keyIsDown = true;
            evt.StopPropagation();

            ((VisualElement)evt.target).AddToClassList("content-section__tap-box--active");

            currentIndex = nextIndex;
            nextIndex = (nextIndex + 1) % timesInMillis.Length;

            timesInMillis[currentIndex] = (uint)(evt.timestamp & 0x0000FFFF);

            label.text = GetBpm().ToString();
        }

        private void OnKeyUp(KeyUpEvent evt)
        {
            if (evt.keyCode != KeyCode.Space || !keyIsDown) return;
            keyIsDown = false;
            evt.StopPropagation();

            ((VisualElement)evt.target).RemoveFromClassList("content-section__tap-box--active");
        }

        private int GetBpm()
        {
            var min = timesInMillis[nextIndex];
            var max = timesInMillis[currentIndex];
            if (min == 0) return 0;

            var avgDelayInSecs = ((float)max - min) / 3 / 1000;

            var result = Mathf.RoundToInt(60 / avgDelayInSecs);
            return Mathf.Max(0, result);
        }
    }
}
