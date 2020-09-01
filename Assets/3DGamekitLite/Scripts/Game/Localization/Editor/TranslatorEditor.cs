using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    [CustomEditor(typeof(Translator))]
    public class TranslatorEditor : Editor
    {
        SerializedProperty m_PhrasesProp;
        SerializedProperty m_LanguageIndexProp;
        Translator m_Translator;
        string[] m_AvailableLanguages;

        void OnEnable ()
        {
            m_PhrasesProp = serializedObject.FindProperty ("phrases");
            m_LanguageIndexProp = serializedObject.FindProperty ("m_LanguageIndex");

            m_Translator = (Translator)target;

            if(AllOriginalPhrasesNonNull ())
                SetupLanguages ();
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            if (m_PhrasesProp.arraySize > 0 && AllOriginalPhrasesNonNull ())
            {
                m_LanguageIndexProp.intValue = EditorGUILayout.Popup("Language", m_LanguageIndexProp.intValue, m_AvailableLanguages);

                OriginalPhrases selectedPhrases = m_Translator.phrases[m_LanguageIndexProp.intValue];
                for (int i = 0; i < selectedPhrases.phrases.Count; i++)
                {
                    Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    labelRect.width *= 0.25f;

                    EditorGUI.LabelField(labelRect, selectedPhrases.phrases[i].key);

                    labelRect.x += labelRect.width;
                    labelRect.width *= 3f;

                    EditorGUI.LabelField(labelRect, selectedPhrases.phrases[i].value);
                }
            }

            Rect phrasesNameRect = EditorGUILayout.GetControlRect (GUILayout.Height (EditorGUIUtility.singleLineHeight));
            phrasesNameRect.width *= 0.5f;

            m_PhrasesProp.isExpanded = EditorGUI.Foldout (phrasesNameRect, m_PhrasesProp.isExpanded, "Phrases");

            phrasesNameRect.x += phrasesNameRect.width;
            m_PhrasesProp.arraySize = EditorGUI.IntField (phrasesNameRect, GUIContent.none, m_PhrasesProp.arraySize);

            if (m_PhrasesProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < m_PhrasesProp.arraySize; i++)
                {
                    SerializedProperty element = m_PhrasesProp.GetArrayElementAtIndex (i);
                    EditorGUILayout.PropertyField (element);
                }
                if (EditorGUI.EndChangeCheck () && AllOriginalPhrasesNonNull ())
                {
                    SetupLanguages ();
                }
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties ();
        }

        void SetupLanguages ()
        {
            m_AvailableLanguages = new string[m_PhrasesProp.arraySize];
            for (int i = 0; i < m_AvailableLanguages.Length; i++)
            {
                SerializedProperty element = m_PhrasesProp.GetArrayElementAtIndex (i);
                OriginalPhrases originalPhrases = element.objectReferenceValue as OriginalPhrases;
                m_AvailableLanguages[i] = originalPhrases.language;
            }
        }

        bool AllOriginalPhrasesNonNull ()
        {
            for (int i = 0; i < m_PhrasesProp.arraySize; i++)
            {
                SerializedProperty element = m_PhrasesProp.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == null)
                    return false;
            }
            return true;
        }
    }
}