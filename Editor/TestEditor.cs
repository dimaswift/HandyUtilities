using UnityEngine;
using UnityEditor;

namespace HandyUtilities.Tests
{
    public class TestEditor
    {
        [MenuItem("CONTEXT/Component/Create Test")]
        public static void CreateTest(MenuCommand command)
        {
            var type = command.context.GetType().Name;
     
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

            AssetDatabase.ImportAsset(Helper.ConvertLoRelativePath(scriptPath));
        }
    }
}

