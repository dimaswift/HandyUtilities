using UnityEngine;

namespace HandyUtilities.PoolSystem
{
    public static class PoolContainerHelper
    {

        public static void CreatePool<T, T2>(T target) where T2 : PoolContainer<T> where T : PooledObject<T>
        {
#if UNITY_EDITOR
            var pool = ScriptableObject.CreateInstance<T2>();
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(target.gameObject);

            var dirPath = System.IO.Path.GetDirectoryName(assetPath);
            assetPath = dirPath + "/" + "pool_" + target.name + ".asset";
            pool.prefab = target;
            UnityEditor.EditorUtility.SetDirty(pool);
            UnityEditor.AssetDatabase.CreateAsset(pool, assetPath);
            target.pool = UnityEditor.AssetDatabase.LoadAssetAtPath<T2>(assetPath);
            UnityEditor.EditorUtility.SetDirty(target);
#endif
        }


        public static SO CreateScriptableObjectAsset<SO>(string name) where SO : ScriptableObject
        {
#if UNITY_EDITOR
            var path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Instance", name, "asset", "Choose Folder", Application.dataPath);
            UnityEditor.AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SO>(), path);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<SO>(path);
#else
            return null;
#endif
        }

    }

    public abstract class PoolContainer<T> : SOContainer where T : PooledObject<T>
    {
        [SerializeField]
        protected T m_prefab;

        [SerializeField]
        bool m_adjustCapacity = false;

        public T prefab
        {
            get { return m_prefab; }
            set { m_prefab = value; }
        }

        [SerializeField]
        protected int m_size = 10;
        [System.NonSerialized]
        bool m_initialized = false;

        System.Action onCapacityExeeded;

        public override void Init()
        {
            if (m_initialized) return;
            m_pool = new Pool<T>(m_prefab, m_size, OnCapacityExceeded);
            m_initialized = true;
        }

        void OnCapacityExceeded()
        {
            if(m_adjustCapacity)
            {
                pool.IncreaseCapacity(1);
                m_size++;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);  
#endif
            }
        }

        protected Pool<T> m_pool;

        public Pool<T> pool { get { return m_pool; } }

#if UNITY_EDITOR


        /**********************************************************************************
        Use this code to create instances of pool containers: 
        ***********************************************************************************
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/PooledObjectType/Create Pool")]
        static void Create(UnityEditor.MenuCommand command) 
        {
            PoolContainerHelper.CreatePool<PooledObjectType, PoolType>(command.context as PooledObjectType);
        }
#endif

#if UNITY_EDITOR
    [UnityEditor.MenuItem("HandyUtilities/PoolSystem/Create Instance of Space Pool")]
    static void Create()
    {
        PoolContainerHelper.CreateScriptableObjectAsset<PoolContainer<PooledObjectType>>("<name>");
    }
#endif

        ***********************************************************************************/
#endif

    }

}
