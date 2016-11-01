using UnityEngine;
using System.Collections;

namespace HandyUtilities.Tests
{
    public abstract class Test<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        T m_target;

        public T target { get { return m_target; } }

        public abstract void Start();

        public abstract void Update();
    }

}
