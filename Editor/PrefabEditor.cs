using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HandyUtilities
{
    public class NestedPrefabEditor : Editor
    {
        public static void CreateNestedPrefabScript(Component component)
        {
            var path = Helper.ConvertToAbsolutePath("Assets/Scripts/" + component.GetType().Name + "Prefab.cs");
            if(System.IO.File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog("Warning", "File already exists. Replace?", "Yes", "Cancel"))
                    return;
            }

            var scriptString = string.Format(@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyUtilities;

public class {0}Prefab : PrefabComponent<{0}> 
{{
    
}}
", component.GetType().Name);

            System.IO.File.WriteAllText(path, scriptString);
            AssetDatabase.Refresh();
        }

        [MenuItem("CONTEXT/Component/Create PrefabComponent Script")]
        static void CreateNestedPrefabScript(MenuCommand command)
        {
            CreateNestedPrefabScript(command.context as Component);
        }
    }
}