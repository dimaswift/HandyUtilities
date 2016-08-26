using UnityEngine;
using System.Collections;

public abstract class PooledObject<T> : MonoBehaviour
{
    Transform m_transform;
    bool m_isActive = true;

    public abstract T Object { get; }

    public Transform cachedTransform
    {
        get
        {
            return m_transform;
        }
    }

    public abstract bool isVisible { get; }

    public bool isActive { get { return m_isActive; } }

    public virtual void Init()
    {
        m_transform = transform;
    }

    public abstract void Pick();

    public void SetActive(bool active)
    {
        if (m_isActive != active)
        {
            gameObject.SetActive(active);
            m_isActive = active;
        }
    }


    public void Return()
    {

    }

    public abstract bool IsReadyToPick();

    public abstract void ResetObject();
}
