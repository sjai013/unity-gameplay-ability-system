using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D.WorldBuilding
{
    [System.Serializable]
    public class Variations : MonoBehaviour
    {
        public float minScale = 1;
        public float maxScale = 1;
        public List<GameObject> gameObjects = new List<GameObject>();

        void Reset()
        {
            gameObjects.Add(gameObject);
        }


    }
}