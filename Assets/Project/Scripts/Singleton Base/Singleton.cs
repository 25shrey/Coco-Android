using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.BaseFramework
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        [SerializeField] private bool isDontDestroyOnLoad;
        
        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                if (isDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(this);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}