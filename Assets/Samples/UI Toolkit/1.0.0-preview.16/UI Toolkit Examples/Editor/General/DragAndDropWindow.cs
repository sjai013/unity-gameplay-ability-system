using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Editor.General
{
    public class DragAndDropWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset uxmlAsset = default;

        private VisualElement dropArea;
        private Label dropLabel;
        private Object droppedObject = null;
        private string assetPath = string.Empty;

        [MenuItem("Window/UI Toolkit/Examples/General/Drag And Drop (Editor)")]
        public static void OpenDragAndDropWindows()
        {
            var windowA = CreateInstance<DragAndDropWindow>();
            var windowB = CreateInstance<DragAndDropWindow>();

            windowA.minSize = windowB.minSize = new Vector2(300f, 180f);
            windowA.Show();
            windowB.Show();
            windowA.titleContent = new GUIContent("Drag and Drop A");
            windowB.titleContent = new GUIContent("Drag and Drop B");
            windowA.position = new Rect(new Vector2(50f, 50), windowA.minSize);
            windowB.position = new Rect(new Vector2(450f, 100), windowA.minSize);
        }

        private void OnEnable()
        {
            if (uxmlAsset != null) uxmlAsset.CloneTree(rootVisualElement);
            RegisterCallbacks();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            dropArea = rootVisualElement.Q<VisualElement>(className: "drop-area");
            dropLabel = rootVisualElement.Q<Label>(className: "drop-area__label");
            dropArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
            dropArea.RegisterCallback<DragEnterEvent>(OnDragEnter);
            dropArea.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            dropArea.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            dropArea.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        private void UnregisterCallbacks()
        {
            dropArea.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            dropArea.UnregisterCallback<DragEnterEvent>(OnDragEnter);
            dropArea.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            dropArea.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            dropArea.UnregisterCallback<DragPerformEvent>(OnDragPerform);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (droppedObject != null)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new[] { droppedObject };
                if (assetPath != string.Empty)
                {
                    DragAndDrop.paths = new[] { assetPath };
                }
                else
                {
                    DragAndDrop.paths = new string[] {};
                }
                DragAndDrop.StartDrag(string.Empty);
            }
        }

        private void OnDragEnter(DragEnterEvent evt)
        {
            string draggedName = "";
            if (DragAndDrop.paths.Length > 0)
            {
                assetPath = DragAndDrop.paths[0];
                var splitPath = assetPath.Split('/');
                draggedName = splitPath[splitPath.Length - 1];
            }
            else if (DragAndDrop.objectReferences.Length > 0)
            {
                draggedName = DragAndDrop.objectReferences[0].name;
            }
            dropLabel.text = $"Dropping '{draggedName}'...";
            dropArea.AddToClassList("drop-area--dropping");
        }

        private void OnDragLeave(DragLeaveEvent evt)
        {
            assetPath = string.Empty;
            droppedObject = null;
            dropLabel.text = "Drag an asset here...";
            dropArea.RemoveFromClassList("drop-area--dropping");
        }

        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            droppedObject = DragAndDrop.objectReferences[0];
            var draggedName = "";
            if (assetPath != string.Empty)
            {
                var splitPath = assetPath.Split('/');
                draggedName = splitPath[splitPath.Length - 1];
            }
            else
            {
                draggedName = droppedObject.name;
            }
            dropLabel.text = $"Containing '{draggedName}'...\n\n" +
                $"(You can also drag from here)";
            dropArea.RemoveFromClassList("drop-area--dropping");
        }
    }
}
