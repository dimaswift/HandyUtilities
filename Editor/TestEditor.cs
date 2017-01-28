using UnityEngine;
using UnityEditor;

namespace HandyUtilities.Tests
{
    public class TestEditor : SerializedSingleton<TestEditor>
    {
        public string typeName;
        public int targetID;
        public bool pendingTest;

        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptsReloaded()
        {
            if(Instance.pendingTest)
            {
                Instance.pendingTest = false;
                var testObject = new GameObject(Instance.typeName).AddComponent(Helper.GetType(Instance.typeName));
                var so = new SerializedObject(testObject);
                so.FindProperty("m_target").objectReferenceInstanceIDValue = Instance.targetID;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(testObject);
                Selection.activeGameObject = testObject.gameObject;
                EditorUtility.SetDirty(Instance);  
            }
        }

        [MenuItem("CONTEXT/Component/Create Test")]
        public static void CreateTest(MenuCommand command)
        {
            var type = command.context.GetType().FullName;
     
            var scriptsFolder = Application.dataPath + "/Scripts";
       
            if(!System.IO.Directory.Exists(scriptsFolder))
            {
                System.IO.Directory.CreateDirectory(scriptsFolder);
                AssetDatabase.Refresh();
            }

           

            var scriptPath = scriptsFolder + "/" + type + "Test" + ".cs";

            var script = System.IO.File.CreateText(scriptPath);

            script.Write(string.Format(@"using UnityEngine;
using HandyUtilities.Tests;

public class {0}Test : Test<{0}>
{{
    public override void Start()
    {{
        
    }}

    public override void Update()
    {{
            
    }}
}}
", type));

            script.Close();
            Instance.targetID = command.context.GetInstanceID();
            Instance.typeName = type + "Test";
            Instance.pendingTest = true;
            EditorUtility.SetDirty(Instance);  
            AssetDatabase.ImportAsset(Helper.ConvertLoRelativePath(scriptPath));
        }
    }
}

