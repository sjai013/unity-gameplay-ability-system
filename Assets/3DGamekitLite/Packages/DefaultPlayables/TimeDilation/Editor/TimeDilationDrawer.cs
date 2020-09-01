using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TimeDilationBehaviour))]
public class TimeDilationDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 1;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty timeScaleProp = property.FindPropertyRelative("timeScale");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, timeScaleProp);
    }
}
