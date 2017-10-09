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

        Transform m_effectTransform;

        ParticleSystem[] m_effects;

        int m_effectsLength;

        public void Init()
        {
            if (m_initialized) return;
            m_initialized = true;
            var effect = Instantiate(m_effectPrefab);
            m_effects = effect.GetComponentsInChildren<ParticleSystem>(true);
            m_effectTransform = effect.transform;
            m_effectsLength = m_effects.Length;
        }

        public void Play()
        {
            if (!m_initialized)
                Init();
            for (int i = 0; i < m_effectsLength; i++)
            {
                m_effects[i].Play(false);
            }
        }

        public void Play(Vector3 position)
        {
            if (!m_initialized)
                Init();
            m_effectTransform.position = position;
            for (int i = 0; i < m_effectsLength; i++)
            {
                m_effects[i].Play(false);
            }
        }

        public void Play(Vector3 position, Vector3 euler)
        {
            if (!m_initialized)
                Init();
            m_effectTransform.position = position;
            m_effectTransform.eulerAngles = euler;
            for (int i = 0; i < m_effectsLength; i++)
            {
                m_effects[i].Play(false);
            }
        }
    }

}
