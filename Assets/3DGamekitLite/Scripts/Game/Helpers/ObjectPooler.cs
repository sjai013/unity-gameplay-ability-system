using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class ObjectPooler<T> where T : UnityEngine.MonoBehaviour, IPooled<T>
    {
        public T[] instances;

        protected Stack<int> m_FreeIdx;

        public void Initialize(int count, T prefab)
        {
            instances = new T[count];
            m_FreeIdx = new Stack<int>(count);

            for (int i = 0; i < count; ++i)
            {
                instances[i] = Object.Instantiate(prefab);
                instances[i].gameObject.SetActive(false);
                instances[i].poolID = i;
                instances[i].pool = this;

                m_FreeIdx.Push(i);
            }
        }

        public T GetNew()
        {
            int idx = m_FreeIdx.Pop();
            instances[idx].gameObject.SetActive(true);

            return instances[idx];
        }

        public void Free(T obj)
        {
            m_FreeIdx.Push(obj.poolID);
            instances[obj.poolID].gameObject.SetActive(false);
        }
    }

    public interface IPooled<T> where T : MonoBehaviour, IPooled<T>
    {
        int poolID { get; set; }
        ObjectPooler<T> pool { get; set; }
    } 
}