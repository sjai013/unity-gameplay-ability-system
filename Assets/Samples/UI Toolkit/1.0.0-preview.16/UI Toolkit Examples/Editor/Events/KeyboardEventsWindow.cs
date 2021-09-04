using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Editor.Events
{
    public class KeyboardEventsWindow : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/Examples/Events/Keyboard Events (Editor)")]
        private static void OpenWindow()
        {
            var window = GetWindow<KeyboardEventsWindow>("Keyboard Events");
            window.minSize = new Vector2(700, 230);
            EditorGUIUtility.PingObject(MonoScript.FromScriptableObject(window));
        }

        [SerializeField] private VisualTreeAsset uxmlAsset = default;

        private Label label;

        private bool keyIsDown = false;
        private readonly int[] timesInMillis = new int[4];
        private int currentIndex;
        private int nextIndex;

        public void OnEnable()
        {
            uxmlAsset.CloneTree(rootVisualElement);

            label = rootVisualElement.Q<Label>(className: "display__bpm");

            var tapElement = rootVisualElement.Q<VisualElement>(className: "content-section__tap-box");
            tapElement.focusable = true;
            tapElement.pickingMode = PickingMode.Position;
            tapElement.RegisterCallback<AttachToPanelEvent>(_ => tapElement.Focus());
            tapElement.RegisterCallback<BlurEvent>(_ => tapElement.Focus());
            tapElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
            tapElement.RegisterCallback<KeyUpEvent>(OnKeyUp);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Space || keyIsDown) return;
            keyIsDown = true;
            evt.StopPropagation();

            ((VisualElement)evt.target).AddToClassList("content-section__tap-box--active");

            currentIndex = nextIndex;
            nextIndex = (nextIndex + 1) % timesInMillis.Length;

            timesInMillis[currentIndex] = (int)(evt.timestamp & 0x0000FFFF);

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
