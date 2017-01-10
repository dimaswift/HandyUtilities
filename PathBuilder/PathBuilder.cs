using UnityEngine;
using System.Collections.Generic;
using HandyUtilities.PoolSystem;

namespace HandyUtilities
{
    public interface IPathChunk
    {
        void OnConnect(IPathChunk next);
        Transform connector { get; }
        void SetActive(bool active);
        void Init();
        Transform cachedTransform { get; }
        void Prepare();
    }

    public class PathBuilder<T> : MonoBehaviour where T : MonoBehaviour, IPathChunk
    {
        [SerializeField]
        int m_poolSize = 10;

        [SerializeField]
        T[] m_prefabs;

        protected List<T> m_chunkPool = new List<T>();

        public int currentChunkIndex { get { return m_currentChunkIndex; } }

        T m_lastChunk;

        int m_currentChunkIndex = 0;

        int m_chunkIndex = 0;

        public virtual void Init()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                var pool = m_prefabs.Random();
                var c = Instantiate(m_prefabs.Random());
                c.Init();
                c.SetActive(false);
                c.cachedTransform.SetParent(transform);
                m_chunkPool.Add(c);
            }
        }

        public T GetClosestChunk()
        {
            return m_chunkPool[m_currentChunkIndex];
        }

        public virtual void Build()
        {
            m_chunkIndex = 0;
            m_chunkPool.Shuffle();
            m_currentChunkIndex = 0;
            var first = m_chunkPool[0];

            first.cachedTransform.localEulerAngles = Vector3.zero;
            first.cachedTransform.localPosition = new Vector3(0, 0, 0);
            first.Prepare();

            first.SetActive(true);

            m_chunkIndex = 0;

            for (int i = 0; i < m_poolSize; i++)
            {
                if (i < m_chunkPool.Count)
                {
                    var chunk = m_chunkPool[i];
                    chunk.SetActive(true);
                    if (i < m_poolSize - 1 && i < m_chunkPool.Count - 1)
                    {
                        var next = m_chunkPool.NextItem(i);
                        Connect(chunk, next);
                    }
                    m_chunkIndex++;
                }
            }
            var pos = first.connector.position;
        }

        void Connect(T connector, T connected)
        {
            connected.Prepare();
            connector.OnConnect(connected);
            connected.cachedTransform.position = connector.connector.position;
            connected.cachedTransform.rotation = connector.connector.rotation;
        }

        void ConnectLast()
        {
            var first = m_chunkPool[0];
            var last = m_chunkPool.LastItem();
            Connect(last, first);
            m_chunkPool.RemoveAt(0);
            m_chunkPool.Add(first);
        }

        public virtual void OnChunkPassed()
        {
            if (m_currentChunkIndex >= m_poolSize / 2)
            {
                ConnectLast();
            }
            else m_currentChunkIndex++;
        }
    }
}
