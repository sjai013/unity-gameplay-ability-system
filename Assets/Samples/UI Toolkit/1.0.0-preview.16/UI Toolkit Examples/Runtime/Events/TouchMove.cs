using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using PointerType = UnityEngine.UIElements.PointerType;

namespace Samples.Runtime.Events
{
    [RequireComponent(typeof(UIDocument))]
    public class TouchMove : MonoBehaviour
    {
        [SerializeField] private PanelSettings panelSettings = default;
        [SerializeField] private VisualTreeAsset sourceAsset = default;

        private UIDocument m_Document;
        private List<Vector2> m_StartPosition;
        private List<Vector2> m_PointerStartPosition;
        private VisualElement m_Container;

        // We support 4 simultaneous dragging, but we need to support
        // up to 8 fingers because when there is a rapid up/down sequence
        // the lifted finger id is not reused.
        private static readonly int s_MaxPointers = PointerId.maxPointers;

        void Awake()
        {
            m_Document = GetComponent<UIDocument>();
            m_Document.panelSettings = panelSettings;
            m_Document.visualTreeAsset = sourceAsset;
        }

        void Start()
        {
            m_StartPosition = new List<Vector2>();
            m_PointerStartPosition = new List<Vector2>();
            for (var i = 0; i < s_MaxPointers; i++)
            {
                m_StartPosition.Add(Vector2.zero);
                m_PointerStartPosition.Add(Vector2.zero);
            }
        }

        void OnEnable()
        {
            var visualTree = m_Document.rootVisualElement;

            m_Container = visualTree.Q(null, "container");
            visualTree.Query(null, "elem").ForEach(e =>
            {
                e.RegisterCallback<PointerDownEvent>(OnPointerDown);
                e.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                e.RegisterCallback<PointerUpEvent>(OnPointerUp);
            });
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.currentTarget == evt.target)
            {
                evt.target.CapturePointer(evt.pointerId);

                VisualElement ve = (VisualElement)evt.target;
                ve.AddToClassList("active");

                var pointerIndex = evt.pointerId;
                m_StartPosition[pointerIndex] = new Vector2(ve.resolvedStyle.left, ve.resolvedStyle.top);
                m_PointerStartPosition[pointerIndex] = evt.position;
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (evt.target.HasPointerCapture(evt.pointerId))
            {
                Debug.Assert(evt.currentTarget == evt.target);

                VisualElement ve = (VisualElement)evt.target;

                Vector2 size = new Vector2(m_Container.resolvedStyle.width, m_Container.resolvedStyle.height);
                size -= new Vector2(ve.resolvedStyle.width, ve.resolvedStyle.height);

                var pointerIndex = evt.pointerId;
                Vector2 p = m_StartPosition[pointerIndex] + new Vector2(evt.position.x, evt.position.y) - m_PointerStartPosition[pointerIndex];
                p = Vector2.Max(p, Vector2.zero);
                p = Vector2.Min(p, size);
                ve.style.left = p.x;
                ve.style.top = p.y;
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.target.HasPointerCapture(evt.pointerId))
            {
                Debug.Assert(evt.currentTarget == evt.target);

                VisualElement ve = (VisualElement)evt.target;
                ve.RemoveFromClassList("active");

                evt.target.ReleasePointer(evt.pointerId);
            }
        }
    }
}
