using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamekit3D
{
    public class Respawner : MonoBehaviour
    {
        [System.Serializable]
        public class SaveState
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        public GameObject player;
        public float savePeriod = 5;

        public List<SaveState> savedStates = new List<SaveState>();

        float lastCheck = 0f;
        bool paused = false;

        void Start()
        {
            lastCheck = Time.time - savePeriod;
        }

        public void Pause()
        {
            paused = true;
        }

        public void Resume()
        {
            paused = false;
        }

        public void RestoreLast()
        {
            if (savedStates.Count > 0)
            {
                var ss = savedStates[savedStates.Count - 1];
                savedStates.RemoveAt(savedStates.Count - 1);
                player.transform.position = ss.position;
                player.transform.rotation = ss.rotation;
            }
        }

        void Update()
        {
            if (!paused && Time.time - lastCheck > savePeriod)
            {
                lastCheck = Time.time;
                savedStates.Add(new SaveState() { position = player.transform.position, rotation = player.transform.rotation });
                savedStates.RemoveRange(0, Mathf.Max(0, savedStates.Count - 8));
            }
        }
    }

}