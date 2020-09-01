using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(MaterialSwitcherBehaviour))]
// NOT WORKING, DO NOT ENABLE
public class MaterialSwitcherDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty materialIndexPairsProp = property.FindPropertyRelative("materialIndexPairs");
        int fieldCount = materialIndexPairsProp.isExpanded ? 2 : 1;
        fieldCount += 2 * materialIndexPairsProp.arraySize;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty materialIndexPairsProp = property.FindPropertyRelative("materialIndexPairs");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, materialIndexPairsProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        materialIndexPairsProp.arraySize = EditorGUI.IntField(singleFieldRect, "size", materialIndexPairsProp.arraySize);

        EditorGUI.indentLevel++;
        for (int i = 0; i < materialIndexPairsProp.arraySize; i++)
        {
            SerializedProperty pairProp = materialIndexPairsProp.GetArrayElementAtIndex(i);
            SerializedProperty materialProp = pairProp.FindPropertyRelative("replacementMaterial");
            SerializedProperty indexProp = pairProp.FindPropertyRelative("materialIndexToReplace");

            singleFieldRect.y += 5f;
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, materialProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, indexProp);
        }
        EditorGUI.indentLevel--;
    }
}
