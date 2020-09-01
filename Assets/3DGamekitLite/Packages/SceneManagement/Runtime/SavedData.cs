using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class SavedData : MonoBehaviour
    {
        public static SavedData Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<SavedData>();

                if (instance != null)
                    return instance;

                GameObject spawnManagerGameObject = new GameObject("SpawnManager");
                instance = spawnManagerGameObject.AddComponent<SavedData>();

                return instance;
            }
        }

        protected static SavedData instance;

        protected Dictionary<string, string> m_StringSavedData;
        protected Dictionary<string, bool> m_BoolSavedData;
        protected Dictionary<string, int> m_IntSavedData;
        protected Dictionary<string, float> m_FloatSavedData;
        protected Dictionary<string, Vector2> m_Vector2SavedData;

        void Start()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public static string Get(string key, out string value)
        {
            if (Instance.m_StringSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No string data was found with the key - " + key);
        }

        public static bool Get(string key, out bool value)
        {
            if (Instance.m_BoolSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No bool data was found with the key - " + key);
        }

        public static int Get(string key, out int value)
        {
            if (Instance.m_IntSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No int data was found with the key - " + key);
        }

        public static float Get(string key, out float value)
        {
            if (Instance.m_FloatSavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No float data was found with the key - " + key);
        }

        public static Vector2 Get(string key, out Vector2 value)
        {
            if (Instance.m_Vector2SavedData.TryGetValue(key, out value))
                return value;
            throw new UnityException("No Vector2 data was found with the key - " + key);
        }

        public static void Set(string key, string value)
        {
            if (Instance.m_StringSavedData.ContainsKey(key))
                Instance.m_StringSavedData[key] = value;
            else
                Instance.m_StringSavedData.Add(key, value);
        }

        public static void Set(string key, bool value)
        {
            if (Instance.m_BoolSavedData.ContainsKey(key))
                Instance.m_BoolSavedData[key] = value;
            else
                Instance.m_BoolSavedData.Add(key, value);
        }

        public static void Set(string key, int value)
        {
            if (Instance.m_IntSavedData.ContainsKey(key))
                Instance.m_IntSavedData[key] = value;
            else
                Instance.m_IntSavedData.Add(key, value);
        }

        public static void Set(string key, float value)
        {
            if (Instance.m_FloatSavedData.ContainsKey(key))
                Instance.m_FloatSavedData[key] = value;
            else
                Instance.m_FloatSavedData.Add(key, value);
        }

        public static void Set(string key, Vector2 value)
        {
            if (Instance.m_Vector2SavedData.ContainsKey(key))
                Instance.m_Vector2SavedData[key] = value;
            else
                Instance.m_Vector2SavedData.Add(key, value);
        }
    }
}