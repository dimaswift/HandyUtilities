using UnityEngine;
using System.Collections.Generic;

namespace HandyUtilities
{
    public abstract class BaseBehaviour : MonoBehaviour
    {
        // Inspector
        public bool initOnAwake = true;

        // Props
        public bool initialized { get; private set; }
        public Transform cachedTransform { get { return m_transfrom; } }
        public int ID { get { return m_id; } }
        public abstract State state { get; }

        // Fields
        Transform m_transfrom;
        bool m_activeSelf;
        int m_id;

        // Events
        protected void Awake()
        {
            if (initOnAwake && !initialized)
            {
                Init();
                initialized = true;
            }
        }

        public abstract void SaveState();

        public abstract void LoadState(State state);

        public static void InstantiateWithId(int id, BaseBehaviour prefab)
        {
            var behaviour = Instantiate(prefab);
            behaviour.Init();
            behaviour.m_id = id;
        }

        public virtual void Init()
        {
            m_transfrom = transform;
            m_activeSelf = gameObject.activeSelf;
        }

        public void SetActive(bool active)
        {
            if (active != m_activeSelf)
            {
                gameObject.SetActive(active);
                m_activeSelf = active;
            }
        }

    }

    [System.Serializable]
    public class ObjectGroup
    {
        public int id;
        public List<State> objectStates = new List<State>();

        public void Save()
        {

        }
    }

    [System.Serializable]
    public abstract class State
    {
        public abstract void Save(BaseBehaviour behaviour);
    }

    [System.Serializable]
    public class TransfromState : State
    {
        public Vector3 position;
        public Vector3 euler;

        public void Load(BaseBehaviour behaviour)
        {
            behaviour.cachedTransform.position = position;
            behaviour.cachedTransform.eulerAngles = euler;
        }

        public override void Save(BaseBehaviour behaviour)
        {
            position = behaviour.cachedTransform.position;
            euler = behaviour.cachedTransform.eulerAngles;
        }
    }

}
