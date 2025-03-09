using UnityEngine;

namespace Utility
{
    public class SingletonObject<TComponent> : MonoBehaviour where TComponent : SingletonObject<TComponent>
    {
        public bool destroyOnLoad;

        public static TComponent Get { get; private set; }

        protected virtual void Awake()
        {
            if (!ReferenceEquals(Get, null) && Get != this)
            {
                Destroy(gameObject);
                return;
            }

            if (ReferenceEquals(Get, null))
            {
                Get = this as TComponent;
                if (!destroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            if (Get == this)
            {
                Get = null;
            }
        }
    }
}