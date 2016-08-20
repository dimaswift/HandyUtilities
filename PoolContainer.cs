using UnityEngine;

namespace HandyUtilities.PoolSystem
{
    public abstract class PoolContainer : ScriptableObject
    {
        public int size = 10;
        public abstract void Init();
        public abstract void Reset();

#if UNITY_EDITOR

        public static T CreateScriptableObjectAsset<T>(string name) where T : ScriptableObject
        {
            var path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Instance", name, "asset", "Choose Folder", Application.dataPath);
            var ins = CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(CreateInstance<T>(), path);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
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
