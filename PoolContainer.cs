﻿using UnityEngine;

namespace HandyUtilities.PoolSystem
{
    public abstract class PoolContainer<T> : SOContainer where T : PooledObject<T>
    {
        [SerializeField]
        protected T m_prefab;
        [SerializeField]
        protected int m_size = 10;

        public override void Init()
        {
            m_pool = new Pool<T>(m_prefab, m_size);
        }

        protected Pool<T> m_pool;

        public Pool<T> pool { get { return m_pool; } }

#if UNITY_EDITOR

        public static SO CreateScriptableObjectAsset<SO>(string name) where SO : ScriptableObject
        {
            var path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Instance", name, "asset", "Choose Folder", Application.dataPath);
            UnityEditor.AssetDatabase.CreateAsset(CreateInstance<SO>(), path);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<SO>(path);
        }

        /**********************************************************************************
        Use this code to create instances of pool containers: 
        ***********************************************************************************
        
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("HandyUtilities/PoolSystem/Create Instance of <name>")]
        static void Create()
        {
            CreateScriptableObjectAsset<PoolContainer>("<name>");
        }
        #endif

        ***********************************************************************************/
#endif

    }

}
