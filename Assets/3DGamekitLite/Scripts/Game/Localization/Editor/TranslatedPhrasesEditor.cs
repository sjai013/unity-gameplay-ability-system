using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    [CustomEditor(typeof(TranslatedPhrases))]
    public class TranslatedPhrasesEditor : Editor
    {
        SerializedProperty m_LanguageProp;
        SerializedProperty m_OriginalPhrasesProp;
        SerializedProperty m_PhrasesProp;
        TranslatedPhrases m_TranslatedPhrases;
        OriginalPhrases m_OriginalPhrases;

        void OnEnable ()
        {
            m_LanguageProp = serializedObject.FindProperty ("language");
            m_OriginalPhrasesProp = serializedObject.FindProperty("originalPhrases");
            m_PhrasesProp = serializedObject.FindProperty("phrases");

            m_TranslatedPhrases = (TranslatedPhrases)target;

            if (m_OriginalPhrasesProp.objectReferenceValue != null)
            {
                OriginalPhrasesSetup ();
            }
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            EditorGUILayout.PropertyField (m_LanguageProp);

            EditorGUI.BeginChangeCheck ();
            EditorGUILayout.PropertyField (m_OriginalPhrasesProp);
            if (EditorGUI.EndChangeCheck () && m_OriginalPhrasesProp.objectReferenceValue != null)
            {
                OriginalPhrasesSetup ();
            }

            for (int i = 0; i < m_PhrasesProp.arraySize; i++)
            {
                SerializedProperty elementProp = m_PhrasesProp.GetArrayElementAtIndex (i);
                SerializedProperty keyProp = elementProp.FindPropertyRelative ("key");
                SerializedProperty valueProp = elementProp.FindPropertyRelative ("value");

                Rect propertyRect = EditorGUILayout.GetControlRect (GUILayout.Height (EditorGUIUtility.singleLineHeight * 2f));
                GUI.Box (propertyRect, GUIContent.none);
                propertyRect.height = EditorGUIUtility.singleLineHeight;
                propertyRect.width *= 0.25f;
                EditorGUI.LabelField (propertyRect, new GUIContent(keyProp.stringValue), GUIContent.none);

                propertyRect.x += propertyRect.width;
                propertyRect.width *= 3f;
                EditorGUI.LabelField (propertyRect, new GUIContent(m_OriginalPhrases.phrases[i].value));

                propertyRect.y += EditorGUIUtility.singleLineHeight;
                valueProp.stringValue = EditorGUI.TextField(propertyRect, GUIContent.none, valueProp.stringValue);
            }

            serializedObject.ApplyModifiedProperties ();
        }

        void OriginalPhrasesSetup ()
        {
            m_OriginalPhrases = m_OriginalPhrasesProp.objectReferenceValue as OriginalPhrases;
            m_PhrasesProp.arraySize = m_OriginalPhrases.phrases.Count;

            serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < m_PhrasesProp.arraySize; i++)
            {
                m_TranslatedPhrases.phrases[i].key = m_OriginalPhrases.phrases[i].key;
            }
        }
    }
}