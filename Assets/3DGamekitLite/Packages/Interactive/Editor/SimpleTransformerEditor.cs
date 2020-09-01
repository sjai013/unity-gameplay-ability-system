using UnityEditor;

namespace Gamekit3D.GameCommands
{
    [CustomEditor(typeof(SimpleTransformer), true)]
    public class SimpleTransformerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (cc.changed)
                {
                    var pt = target as SimpleTransformer;
                    pt.PerformTransform(pt.previewPosition);
                }
            }
        }

    }

}
