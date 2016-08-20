using UnityEngine;
using System.Collections.Generic;

namespace HandyUtilities
{
    public sealed class PoolManager : MonoBehaviour
    {
        public bool initOnAwake = true;
        static PoolManager m_instance;
        public static PoolManager instance { get { return m_instance; } }
        public List<SOContainer> pools = new List<SOContainer>();

        public void Init()
        {
            m_instance = this;
            for (int i = 0; i < pools.Count; i++)
            {
                pools[i].Init();
            }
        }

        void Awake()
        {
            if(initOnAwake)
                Init();
        }
    }
}

