using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class Translator : MonoBehaviour
    {
        public static Translator Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;

                s_Instance = FindObjectOfType<Translator>();

                if (s_Instance != null)
                    return s_Instance;

                return CreateDefault();
            }
        }

        protected static Translator s_Instance;

        static Translator CreateDefault ()
        {
            Translator prefab = Resources.Load<Translator>("Translator");
            Translator defaultInstance = Instantiate(prefab);
            return defaultInstance;
        }

        public static string CurrentLanguage
        {
            get { return Instance.phrases[Instance.m_LanguageIndex].language; }
        }

        public List<OriginalPhrases> phrases = new List<OriginalPhrases> ();

        [SerializeField]
        protected int m_LanguageIndex;

        public string this [string key]
        {
            get { return phrases[m_LanguageIndex][key]; }
        }

        public static bool SetLanguage (int index)
        {
            if (index >= Instance.phrases.Count || index < 0)
                return false;

            Instance.m_LanguageIndex = index;
            return true;
        }

        public static bool SetLanguage (string language)
        {
            for (int i = 0; i < Instance.phrases.Count; i++)
            {
                if (Instance.phrases[i].language == language)
                {
                    Instance.m_LanguageIndex = i;
                    return true;
                }
            }
            return false;
        }

        public static void SetLanguage (TranslatedPhrases phrases)
        {
            for (int i = 0; i < Instance.phrases.Count; i++)
            {
                if (Instance.phrases[i] == phrases)
                {
                    Instance.m_LanguageIndex = i;
                    return;
                }
            }
            Instance.phrases.Add (phrases);
            Instance.m_LanguageIndex = Instance.phrases.Count - 1;
        }
    }
}