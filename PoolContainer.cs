using UnityEngine;

namespace HandyUtilities.PoolSystem
{
    public static class PoolContainerHelper
    {
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
        protected int m_size = 10;
        [System.NonSerialized]
        bool m_initialized = false;
        public override void Init()
        {
            if (m_initialized) return;
            m_pool = new Pool<T>(m_prefab, m_size);
            m_initialized = true;
        }

        protected Pool<T> m_pool;

        public Pool<T> pool { get { return m_pool; } }

#if UNITY_EDITOR


        /**********************************************************************************
        Use this code to create instances of pool containers: 
        ***********************************************************************************
        
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
