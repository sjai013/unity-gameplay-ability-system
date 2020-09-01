using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D.GameCommands
{
    public class Collector : MonoBehaviour
    {
        public bool attachCollectables = false;

        Dictionary<string, int> collections = new Dictionary<string, int>();
        public virtual void OnCollect(Collectable collectable)
        {
            if (attachCollectables)
                collectable.transform.parent = transform;
            var count = 0;
            if (collections.TryGetValue(collectable.name, out count))
                collections[collectable.name] = count + 1;
            else
                collections[collectable.name] = 1;
        }

        public bool HasCollectable(string name)
        {
            return collections.ContainsKey(name);
        }

        public bool HasCollectableQuantity(string name, int requiredCount)
        {
            int count;
            if (collections.TryGetValue(name, out count))
                return count >= requiredCount;
            return false;
        }
    }


}