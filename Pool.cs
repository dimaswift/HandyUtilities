using UnityEngine;
using System.Collections.Generic;


namespace HandyUtilities.PoolSystem
{

    using System.Collections;
    using Events;

    public abstract class Pool
    {
        protected int m_order = 0;
        protected int m_size;

        public int order { get { return m_order; } }
        public bool isEmpty { get; protected set; }
        public readonly GameEvent onOverUse = new GameEvent();
    }
    public sealed class InterfacePool<T> : Pool, IEnumerable<T> where T : Component
    {

        T[] m_objects;
        IPoolable[] m_interfaces;

        public T[] objects { get { return m_objects; } }

        public InterfacePool(T source, int size)
        {
            var container = new GameObject(source.name + "_pool").transform;
            m_objects = new T[size];
            m_interfaces = new IPoolable[size];
            m_size = size;
            for (var i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(source.gameObject);
                obj.transform.SetParent(container);
                var ip = obj.GetComponent<T>();
                m_interfaces[i] = ip.GetComponent<IPoolable>();
                m_objects[i] = ip;
                m_interfaces[i].Init();
                ip.gameObject.SetActive(false);
            }
        }

        public InterfacePool(T[] source, int size)
        {
            var container = new GameObject(source[0].name + "_pool").transform;
            m_objects = new T[size];
            m_interfaces = new IPoolable[size];
            m_size = size;
            int index = 0;
            for (var i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(source[index].gameObject);
                obj.transform.SetParent(container);
                var ip = obj.GetComponent<T>();
                m_objects[i] = ip;
                m_interfaces[i] = ip.GetComponent<IPoolable>();
                m_interfaces[i].Init();
                ip.gameObject.SetActive(false);
                index++;
                if (index >= source.Length)
                    index = 0;
            }
        }

        public IEnumerable<T> GetObjects()
        {
            foreach (var o in objects)
                yield return o;
        }

        public T this[int i]
        {
            get
            {
                return objects[i];
            }
        }

        public IEnumerable<T> GetObjects(bool readyOnly)
        {
            for (int i = 0; i < m_size; i++)
            {
                var o = m_objects[i];
                var inter = m_interfaces[i];
                if (readyOnly)
                {
                    if (inter.IsReadyToPick())
                    {
                        yield return o;
                    }
                }
                else
                {
                    if (!inter.IsReadyToPick())
                        yield return o;
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < m_size; i++)
            {
                m_interfaces[i].Prepare();
            }
            isEmpty = false;
            m_order = 0;
        }

        public T PickReadyOne()
        {
            var inter = m_interfaces[m_order];
            var obj = m_objects[m_order];
            SkipNext();
            int c = 0;
            while (!inter.IsReadyToPick())
            {
                obj = m_objects[m_order];
                inter = m_interfaces[m_order];
                SkipNext();
                c++;
                if (c >= m_size)
                {
                    onOverUse.RaiseEvent();
                    return null;
                }
            }
            inter.Pick();
            return obj;
        }


        public T Pick()
        {
            var obj = m_objects[m_order];
            m_interfaces[m_order].Pick();
            SkipNext();
            return obj;
        }

        public void SkipNext()
        {
            m_order++;
            if (m_order > m_size - 1)
            {
                m_order = 0;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < m_size; i++)
            {
                yield return objects[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < m_size; i++)
            {
                yield return objects[i];
            }
        }
    }


    public sealed class TransformPool : Pool
    {
        Transform[] m_objects;

        public TransformPool(Transform source, int size)
        {
            var container = new GameObject(source.name + "_pool").transform;
            m_objects = new Transform[size];
            m_size = size;
            for (var i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(source);
                obj.SetParent(container);
                m_objects[i] = obj;
                obj.gameObject.SetActive(false);
            }
        }

        public TransformPool(Transform[] sources, int size)
        {
            var container = new GameObject(sources[0].name + "_pool").transform;
            m_objects = new Transform[size];
            m_size = size;
            int index = 0;
            for (var i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(sources[index]);
                obj.SetParent(container);
                m_objects[i] = obj;
                obj.gameObject.SetActive(false);
                index++;
                if (index >= sources.Length)
                    index = 0;
            }
        }


        public void Reset()
        {
            for (int i = 0; i < m_objects.Length; i++)
            {
                m_objects[i].gameObject.SetActive(false);
            }
            isEmpty = false;
            m_order = 0;
        }

        public Transform PickSafe()
        {
            var obj = NextItemToPick();
            SkipNext();
            int c = 0;
            while (obj.gameObject.activeSelf)
            {
                obj = NextItemToPick();
                SkipNext();
                c++;
                if (c >= m_size)
                {
                    onOverUse.RaiseEvent();
                    break;
                }
            }
            return obj;
        }

        public Transform Pick()
        {
            var obj = NextReadyItemToPick();
            obj.gameObject.SetActive(true);
            SkipNext();
            return obj;
        }

        public Transform NextItemToPick()
        {
            return m_objects[m_order];
        }

        public void SkipNext()
        {
            m_order++;
            if (m_order > m_size - 1)
            {
                m_order = 0;
            }
        }

        public Transform NextReadyItemToPick()
        {
            var obj = NextItemToPick();
            var c = 0;
            while (obj.gameObject.activeSelf)
            {
                SkipNext();
                obj = NextItemToPick();
                c++;
                if (c >= m_size)
                {
                    onOverUse.RaiseEvent();
                    break;
                }
            }
            return obj;
        }
    }

    public sealed class ParticlePool : Pool
    {
        Transform[] m_objects;
        ParticleSystem[] m_particles;

        public Transform[] objects { get { return m_objects; } }

        public ParticlePool(ParticleSystem source, int size)
        {
            var container = new GameObject(source.name + "_pool").transform;
            m_objects = new Transform[size];
            m_particles = new ParticleSystem[size];
            m_size = size;
            for (var i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(source.transform);
                obj.SetParent(container);
                m_objects[i] = obj;
                m_particles[i] = obj.GetComponentInChildren<ParticleSystem>();
                obj.gameObject.SetActive(false);
            }
        }

        public ParticlePool(ParticleSystem[] sources, int size)
        {
            var container = new GameObject(sources[0].name + "_pool").transform;
            m_objects = new Transform[size];
            m_particles = new ParticleSystem[size];
            m_size = size;
            int index = 0;
            for (var i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(sources[index].transform);
                obj.SetParent(container);
                m_objects[i] = obj;
                m_particles[i] = obj.GetComponent<ParticleSystem>();
                obj.gameObject.SetActive(false);
                index++;
                if (index >= sources.Length)
                    index = 0;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < m_objects.Length; i++)
            {
                m_objects[i].gameObject.SetActive(false);
            }
            isEmpty = false;
            m_order = 0;
        }

        public Transform Pick()
        {
            var obj = NextReadyItemToPick();
            return obj;
        }

        public Transform NextItemToPick()
        {
            return m_objects[m_order];
        }
        public void SkipNext()
        {
            m_order++;
            if (m_order > m_size - 1)
            {
                m_order = 0;
            }
        }
        public Transform NextReadyItemToPick()
        {
            var obj = NextItemToPick();
            SkipNext();
            return obj;
        }
    }

    public sealed class Pool<T> : Pool, IEnumerable<T> where T : PooledObject<T>
    {

        PooledObject<T>[] m_objects;

        public PooledObject<T>[] objects { get { return m_objects; } }

        public Pool(T source, int size)
        {
            var container = new GameObject(source.name + "_pool").transform;
            m_objects = new PooledObject<T>[size];
            m_size = size;
            for (var i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(source.gameObject);
                obj.transform.SetParent(container);
                var ip = obj.GetComponent<PooledObject<T>>();
                m_objects[i] = ip;
                ip.Init();
                ip.SetActive(false);
            }
        }

        public Pool(T[] source, int size)
        {
            var container = new GameObject(source[0].name + "_pool").transform;
            m_objects = new PooledObject<T>[size];
            m_size = size;
            int index = 0;
            for (var i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(source[index].gameObject);
                obj.transform.SetParent(container);
                var ip = obj.GetComponent<PooledObject<T>>();
                m_objects[i] = ip;
                ip.Init();
                ip.SetActive(false);
                index++;
                if (index >= source.Length)
                    index = 0;
            }
        }

        public IEnumerable<T> GetObjects()
        {
            foreach (var o in objects)
                yield return o.Object;
        }
    
        public T this [int i]
        {
            get
            {
                return objects[i].Object;
            }
        }

        public IEnumerable<T> GetObjects(bool readyOnly)
        {
            foreach (var o in objects)
            {
                if (readyOnly)
                {
                    if (o.IsReadyToPick())
                    {
                        yield return o.Object;
                    }
                }
                else
                {
                    if (!o.IsReadyToPick())
                        yield return o.Object;
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].ResetObject();
            }
            isEmpty = false;
            m_order = 0;
        }

        public T PickReadyOne()
        {
            var obj = NextItemToPick();
            SkipNext();
            int c = 0;
            while (!obj.IsReadyToPick())
            {
                obj = NextItemToPick();
                SkipNext();
                c++;
                if (c >= m_size)
                {
                    onOverUse.RaiseEvent();
                    return null;
                }
            }
            obj.Pick();
            return obj.Object;
        }


        public T Pick()
        {
            var obj = NextItemToPick();
            SkipNext();
            obj.Pick();
            return obj.Object;
        }

        public T NextItemToPick()
        {
            return m_objects[m_order].Object;
        }

        public void SkipNext()
        {
            m_order++;
            if (m_order > m_size - 1)
            {
                m_order = 0;
            }
        }

        public T NextReadyItemToPick()
        {
            var obj = NextItemToPick();
            var c = 0;
            while (!obj.IsReadyToPick())
            {
                SkipNext();
                obj = NextItemToPick();
                c++;
                if (c >= m_size)
                {
                    onOverUse.RaiseEvent();
                    break;
                }
            }
            return obj;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                yield return objects[i].Object;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                yield return objects[i].Object;
            }
        }
    }

    public interface IPoolable
    {
        Transform cachedTransform { get; }

        void Init();

        void Pick();

        bool IsReadyToPick();

        void Prepare();

        void SetActive(bool active);
    }


}
