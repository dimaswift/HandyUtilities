using UnityEngine;
using System.Collections;

namespace HandyUtilities
{
    [CreateAssetMenu(menuName = "Handy Utilities/Effect")]
    public class Effect : ScriptableObject
    {
        [SerializeField]
        ParticleSystem m_effectPrefab;

        [System.NonSerialized]
        bool m_initialized;

        ParticleSystem m_effect;
        Transform m_effectTransform;

        public void Init()
        {
            if (m_initialized) return;
            m_initialized = true;
            m_effect = Instantiate(m_effectPrefab);
            m_effectTransform = m_effect.transform;
        }

        public void Play(Vector3 position)
        {
            Play(position, Vector3.zero);
        }

        public void Play(Vector3 position, Vector3 euler)
        {
            if (!m_initialized)
                Init();
            m_effectTransform.position = position;
            m_effectTransform.eulerAngles = euler;
            m_effect.Play();
        }
    }

}
