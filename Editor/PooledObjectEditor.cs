using UnityEngine;
using System.Collections;
using UnityEditor;
namespace HandyUtilities
{
    public class PooledObjectEditor
    {
        [MenuItem("Handy Utilities/Create Pooled Object Script")]
        static void Open()
        {
            ConfirmationTool.OpenWithArguments("Enter Pooled Object name:", "Create", Create, new ConfirmationTool.Label("", "MyPooledObject"));
        }

        static void Create(object[] args)
        {
            string name = (string) args[0];
            var script = string.Format(@"using UnityEngine;
using System.Collections;
using HandyUtilities;
using HandyUtilities.PoolSystem;

public class {0} : PooledObject<{0}>
{{
    [SerializeField]
    {0}Pool m_pool;

    public override {0} Object {{ get {{ return this; }} }}

    public override PoolContainer<{0}> pool
    {{
        get
        {{
            return m_pool;
        }}
        set
        {{
            m_pool = value as {0}Pool;
        }}
    }}
}}", name);

            var pool = string.Format(@"using UnityEngine;
using System.Collections;
using HandyUtilities;
using HandyUtilities.PoolSystem;

public class {0}Pool : PoolContainer<{0}>
{{
#if UNITY_EDITOR
    [UnityEditor.MenuItem(""CONTEXT/{0}/Create Pool"")]
    static void Create(UnityEditor.MenuCommand command)
    {{
        PoolContainerHelper.CreatePool<{0}, {0}Pool>(command.context as {0});
    }}
#endif
}}", name);
            var dir = Application.dataPath + "/Scripts";
            if (!System.IO.Directory.Exists(dir))
            {
                dir = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, Application.dataPath);
            }

            if(!string.IsNullOrEmpty(dir))
            {
                var scriptPath = dir + "/" + name + ".cs";
                var poolPath = dir + "/" + name + "Pool" + ".cs";

                if(System.IO.File.Exists(scriptPath))
                {
                    if (!EditorUtility.DisplayDialog("Warning!", "Overwrite File?", "Yes", "Cancel"))
                        return;
                }
                if (System.IO.File.Exists(poolPath))
                {
                    if (!EditorUtility.DisplayDialog("Warning!", "Overwrite File?", "Yes", "Cancel"))
                        return;
                }
                System.IO.File.WriteAllText(scriptPath, script);
                System.IO.File.WriteAllText(poolPath, pool);
                AssetDatabase.Refresh();
            }
        }
    }
}
