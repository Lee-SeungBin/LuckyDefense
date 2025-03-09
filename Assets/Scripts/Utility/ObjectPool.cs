using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ObjectPool<TPooledObject> where TPooledObject : MonoBehaviour
    {
        public GameObject gameObject;
        public Transform transform;
        public TPooledObject prefab;
        
        private readonly List<TPooledObject> m_PooledObjects = new();

        public int Capacity
        {
            get => m_PooledObjects.Capacity;
            set => m_PooledObjects.Capacity = value;
        }

        public TPooledObject GetOrCreate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return Object.Instantiate(prefab);
            }
#endif
            
            TPooledObject obj;
            if (m_PooledObjects.Count > 0)
            {
                obj = m_PooledObjects[^1];
                m_PooledObjects.RemoveAt(m_PooledObjects.Count - 1);
                return obj;
            }

            obj = Object.Instantiate(prefab);
            return obj;
        }

        public void ReturnObject(TPooledObject garbage)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(garbage);
                return;
            }
#endif
            
            if (m_PooledObjects.Capacity <= m_PooledObjects.Count)
            {
                Object.Destroy(garbage.gameObject);
                return;
            }
            
            m_PooledObjects.Add(garbage);
            garbage.transform.SetParent(gameObject.transform);
        }

        public void ReturnObjects(IEnumerable<TPooledObject> garbages)
        {
            foreach (var e in garbages)
            {
                ReturnObject(e);
            }
        }

        public static ObjectPool<TPooledObject> CreateObjectPool(TPooledObject prefab, Transform holder = null)
        {
            var objectPoolGameObject = new GameObject($"Object Pool : {nameof(TPooledObject)}");
            objectPoolGameObject.transform.SetParent(holder);
            objectPoolGameObject.SetActive(false); // the object pool should always inactive state.

            var objectPool = new ObjectPool<TPooledObject>
            {
                gameObject = objectPoolGameObject,
                transform = objectPoolGameObject.transform,
                prefab = prefab
            };

            return objectPool;
        }
    }
}