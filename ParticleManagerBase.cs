using UnityEngine;
using System.Collections;

namespace HandyUtilities
{
    public abstract class ParticleManagerBase<T> : MonoBehaviour
    {
        protected static T m_instance;
    }

    //[System.Serializable]
    //public class Effect
    //{
    //    [SerializeField]
    //    ParticleSystem m_particleSystem;

    //    Transform m_transfrom;

    //    [System.NonSerialized]
    //    bool m_inited;

    //    void Init()
    //    {
    //        if (m_inited) return;
    //        m_transfrom = m_particleSystem.transform;
    //        m_inited = true;
    //    }

    //    public void Play(Vector3 position)
    //    {
    //        if (!m_inited) Init();
    //        m_transfrom.position = position;
    //        m_particleSystem.Play();
    //    }

    //    public void Play(Vector3 position, Quaternion rotation)
    //    {
    //        if (!m_inited) Init();
    //        m_transfrom.position = position;
    //        m_transfrom.rotation = rotation;
    //        m_particleSystem.Play();
    //    }

    //    public void Emit(Vector3 position, Quaternion rotation, int count)
    //    {
    //        if (!m_inited) Init();
    //        m_transfrom.position = position;
    //        m_transfrom.rotation = rotation;
    //        m_particleSystem.Emit(count);
    //    }

    //    public void Emit(Vector3 position, int count)
    //    {
    //        if (!m_inited) Init();
    //        m_transfrom.position = position;
    //        m_particleSystem.Emit(count);
    //    }
    //}
}
