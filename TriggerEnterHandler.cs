using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
namespace HandyUtilities
{
    public class TriggerEnterHandler : MonoBehaviour
    {
        [SerializeField]
        TriggerAction m_onTriggerEnter;

        public TriggerAction onTriggerEnter { get { return m_onTriggerEnter; } }

        void OnTriggerEnter(Collider c)
        {
            m_onTriggerEnter.Invoke(c);
        }
    }

    [System.Serializable]
    public class TriggerAction : UnityEvent<Collider>
    {
        public TriggerAction() { }
    }
}
