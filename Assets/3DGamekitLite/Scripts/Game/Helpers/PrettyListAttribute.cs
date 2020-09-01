using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Gamekit3D
{
    public class EnforceTypeAttribute : PropertyAttribute
    {
        public System.Type type;

        public EnforceTypeAttribute(System.Type enforcedType)
        {
            type = enforcedType;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnforceTypeAttribute))]
    public class PrettyListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnforceTypeAttribute propAttribute = attribute as EnforceTypeAttribute;
            EditorGUI.BeginProperty(position, label, property);

            MonoBehaviour obj = EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(MonoBehaviour), true) as MonoBehaviour;
            if (obj != null && propAttribute.type.IsAssignableFrom(obj.GetType()) && !EditorGUI.showMixedValue)
            {
                property.objectReferenceValue = obj as MonoBehaviour;
            }
            EditorGUI.EndProperty();
        }
    }
#endif 
}