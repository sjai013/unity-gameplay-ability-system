using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    [CustomPropertyDrawer(typeof(Phrase))]
    public class PhraseDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty keyProp = property.FindPropertyRelative ("key");
            SerializedProperty valueProp = property.FindPropertyRelative ("value");

            Rect propertyRect = position;
            propertyRect.width *= 0.25f;
            EditorGUI.PropertyField(propertyRect, keyProp, GUIContent.none);

            propertyRect.x += propertyRect.width;
            propertyRect.width *= 3f;
            EditorGUI.PropertyField(propertyRect, valueProp, GUIContent.none);
        }
    }
}