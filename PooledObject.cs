namespace HandyUtilities.PoolSystem
{
    using UnityEngine;

    public abstract class PooledObject<T> : MonoBehaviour where T : PooledObject<T>
    {
        Transform m_transform;

        public abstract PoolContainer<T> pool { get; set; }

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

        public virtual void Prepare()
        {

        }

        public virtual void ResetObject()
        {
            SetActive(false);
        }
    }
}