using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TransformTweenBehaviour))]
public class TransformTweenDrawer : PropertyDrawer
{
    GUIContent m_TweenPositionContent = new GUIContent("Tween Position", "This should be true if the transformToMove to change position.  This causes recalulations each frame which are more CPU intensive.");
    GUIContent m_TweenRotationContent = new GUIContent("Tween Rotation", "This should be true if the transformToMove to change rotation.");
    GUIContent m_TweenTypeContent = new GUIContent("Tween Type", "Linear - the transform moves the same amount each frame (assuming static start and end locations).\n"
        + "Deceleration - the transform moves slower the closer to the end location it is.\n"
        + "Harmonic - the transform moves faster in the middle of its tween.\n"
        + "Custom - uses the customStartingSpeed and customEndingSpeed to create a curve for the desired tween.");
    GUIContent m_StartingSpeedContent = new GUIContent("Starting Speed", "This is used when the Tween Type is set to Custom.  It determines how fast the transform will be moving near the Start Location.");
    GUIContent m_EndingSpeedContent = new GUIContent("Ending Speed", "This is used when the Tween Type is set to Custom.  It determines how fast the transform will be moving near the End Location.");

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = property.FindPropertyRelative ("tweenType").enumValueIndex == (int)TransformTweenBehaviour.TweenType.Custom ? 5 : 3;
        return fieldCount * (EditorGUIUtility.singleLineHeight);
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty tweenPositionProp = property.FindPropertyRelative ("tweenPosition");
        SerializedProperty tweenRotationProp = property.FindPropertyRelative("tweenRotation");
        SerializedProperty tweenTypeProp = property.FindPropertyRelative ("tweenType");
        
        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField (singleFieldRect, tweenPositionProp, m_TweenPositionContent);
        
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField (singleFieldRect, tweenRotationProp, m_TweenRotationContent);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, tweenTypeProp, m_TweenTypeContent);

        if (tweenTypeProp.enumValueIndex == (int)TransformTweenBehaviour.TweenType.Custom)
        {
            SerializedProperty startingSpeedProp = property.FindPropertyRelative ("customStartingSpeed");
            SerializedProperty endingSpeedProp = property.FindPropertyRelative ("customEndingSpeed");

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider(singleFieldRect, startingSpeedProp, 0f, 1f, m_StartingSpeedContent);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider (singleFieldRect, endingSpeedProp, 0f, 1f, m_EndingSpeedContent);
        }
    }
}
