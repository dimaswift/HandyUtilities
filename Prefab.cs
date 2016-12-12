using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandyUtilities
{
    public class Prefab : MonoBehaviour
    {
        [SerializeField]
        GameObject m_prefab;

        public bool spawned { get; private set; }
        public GameObject prefab { get { return m_prefab; } }
        public GameObject instance { get; private set; }

        public virtual void Spawn()
        {
            if (spawned) return;
            var t = transform;
            instance = Instantiate(m_prefab);
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(t.parent);
            instanceTransform.position = t.position;
            instanceTransform.rotation= t.rotation;
            instanceTransform.localScale = t.localScale;
            spawned = true;
        }
    }
}
