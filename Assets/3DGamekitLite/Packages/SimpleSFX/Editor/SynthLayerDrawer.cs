using UnityEditor;
using UnityEngine;

namespace Gamekit3D.SimpleSFX
{
    [CustomPropertyDrawer(typeof(SynthLayer))]
    public class SynthLayerDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, UnityEngine.GUIContent label)
        {
            return 32;
        }

        public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
        {
            var rect = position;
            rect.height /= 2;
            rect.width /= 3;
            EditorGUIUtility.labelWidth = 32;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("oscType"), new GUIContent("OT", "Oscillator Type"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("oscillatorFrequency"), new GUIContent("F", "Oscillator Frequency"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("volumeEnvelope"), new GUIContent("V", "Oscillator Volume"));
            rect.y += rect.height;
            rect.x = position.x;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("filterType"), new GUIContent("FT", "Filter Type"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("cutoffEnvelope"), new GUIContent("C", "Filter Cutoff Frequency"));
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("resonanceEnvelope"), new GUIContent("Q", "Filter Resonance"));
        }
    }
}