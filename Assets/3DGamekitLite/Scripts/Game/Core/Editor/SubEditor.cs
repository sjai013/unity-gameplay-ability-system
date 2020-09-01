using UnityEditor;

namespace Gamekit3D
{
    public abstract class SubEditor<T>
    {
        public abstract void OnInspectorGUI(T instance);

        public void Init(Editor editor)
        {
            this.editor = editor;
        }

        public void Update()
        {
            if (defer != null) defer();
            defer = null;
        }

        protected void Defer(System.Action fn)
        {
            defer += fn;
        }

        protected void Repaint()
        {
            editor.Repaint();
        }

        Editor editor;
        System.Action defer;
    } 
}
