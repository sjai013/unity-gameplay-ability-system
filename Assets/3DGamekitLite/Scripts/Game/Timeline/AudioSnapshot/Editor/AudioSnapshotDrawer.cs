using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AudioSnapshotBehaviour))]
public class AudioSnapshotDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 5;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty audioClipProp = property.FindPropertyRelative("audioClip");
        SerializedProperty snapshotProp = property.FindPropertyRelative("snapshot");
        SerializedProperty volumeProp = property.FindPropertyRelative ("volume");
        SerializedProperty weightedVolumeProp = property.FindPropertyRelative ("weightedVolume");
        SerializedProperty audioPlayModeProp = property.FindPropertyRelative("audioPlayMode");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, audioClipProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, snapshotProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, volumeProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, weightedVolumeProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, audioPlayModeProp);
    }
}
