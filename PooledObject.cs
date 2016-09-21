﻿namespace HandyUtilities.PoolSystem
{
    using UnityEngine;

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

        public virtual bool isVisible
        {
            get
            {
                return true;
            }
        }

        public bool isActive { get { return m_isActive; } }

        public virtual void Init()
        {
            m_transform = transform;
        }

        public virtual void Pick()
        {
            SetActive(true);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            m_isActive = active;
        }

        public virtual bool IsReadyToPick()
        {
            return !isActive;
        }

        public virtual void ResetObject()
        {
            SetActive(false);
        }
    }
}