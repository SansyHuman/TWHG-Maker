using UnityEngine;

namespace SansyHuman.TWHG.Core
{
    /// <summary>
    /// Base class of singleton classes which inherits MonoBehaviour.
    /// </summary>
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<T>();
                    if (!_instance)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        _instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);
                    }
                }

                return _instance;
            }
        }
    }
}
