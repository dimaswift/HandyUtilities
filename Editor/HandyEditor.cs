using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HandyUtilities
{
    public static class HandyEditor
    {
        public static Tool lastTool { get; private set; }

        public static RandomIntRange DrawRandomRange(Rect rect, RandomIntRange range, string label)
        {
            GUI.Label(rect, label);
            var r = rect;
            r.width = rect.width * .125f;
            r.x = rect.x + rect.width * .5f;
            GUI.Label(new Rect(r.x, r.y, r.width, r.height), "min:");
            range.min = EditorGUI.IntField(new Rect(r.x + r.width, r.y, r.width, r.height), range.min);
            GUI.Label(new Rect(r.x + r.width * 2, r.y, r.width, r.height), "max:");
            range.max = EditorGUI.IntField(new Rect(r.x + r.width * 3f, r.y, r.width, r.height), range.max);
            return range;
        }

        public static RandomFloatRange DrawRandomRange(Rect rect, RandomFloatRange range, string label)
        {
            GUI.Label(rect, label);
            var r = rect;
            r.width = rect.width * .125f;
            r.x = rect.x + rect.width * .5f;
            GUI.Label(new Rect(r.x, r.y, r.width, r.height), "min:");
            range.min = EditorGUI.FloatField(new Rect(r.x + r.width, r.y, r.width, r.height), range.min);
            GUI.Label(new Rect(r.x + r.width * 2, r.y, r.width, r.height), "max:");
            range.max = EditorGUI.FloatField(new Rect(r.x + r.width * 3f, r.y, r.width, r.height), range.max);
            return range;
        }

        public static Vector3 mousePosition
        {
            get
            {
                if (SceneView.currentDrawingSceneView == null) return Vector3.zero;
                var scene = SceneView.currentDrawingSceneView.camera;
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y = scene.pixelHeight - mousePos.y;
                var pos = scene.ScreenPointToRay(mousePos).origin;
                pos.z = 0;
                return pos;
            }
        }

        [MenuItem("CONTEXT/Component/Move on Top")]
        public static void MoveComponentOnTop(MenuCommand command)
        {
            var component = command.context as Component;
            var components = component.GetComponents<Component>();
            while (components[1] != component)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
                components = component.GetComponents<Component>();
            }
        }

        [MenuItem("HandyUtilities/Apply Selected Prefabs")]
        public static void SaveSelectedPrefabs(MenuCommand command)
        {
            foreach (var item in Selection.gameObjects)
            {
                var p = PrefabUtility.GetPrefabParent(item);
                if (p != null)
                {
                    PrefabUtility.ReplacePrefab(item, PrefabUtility.GetPrefabParent(item), ReplacePrefabOptions.ConnectToPrefab);
                }
            }
        }

        public static EventType GetEvent()
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            var res = e.GetTypeForControl(controlID);
            e.Use();
            return res;

        }

        public static void MethodsPopup(string label, UnityEngine.Object target, System.Type classType, string propertyName, BindingFlags flags)
        {
            var so = new SerializedObject(target);
            var methods = classType.GetMethods(flags);

            string[] methodNames = new string[methods.Length + 1];
            methodNames[0] = "None";
            for (int i = 0; i < methods.Length; i++)
            {
                methodNames[i + 1] = methods[i].Name;
            }
            var prop = so.FindProperty(propertyName);
            int index = System.Array.FindIndex(methodNames, (s) => s == prop.stringValue);
            if (index < 0)
                index = 0;
            prop.stringValue = methodNames[EditorGUILayout.Popup(label, index, methodNames)];
            if (GUI.changed)
            {
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        public static T CreateScriptableObjectAsset<T>(string name) where T : ScriptableObject
        {
            var path = "Assets/" + name + ".asset";
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), path);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static void FieldsPopup(string label, UnityEngine.Object target, System.Type classType, string propertyName, BindingFlags flags)
        {
            var so = new SerializedObject(target);
            var fields = classType.GetFields(flags);

            string[] fieldsNames = new string[fields.Length + 1];
            fieldsNames[0] = "None";
            for (int i = 0; i < fields.Length; i++)
            {
                fieldsNames[i + 1] = fields[i].Name;
            }
            var prop = so.FindProperty(propertyName);
            int index = System.Array.FindIndex(fieldsNames, (s) => s == prop.stringValue);
            if (index < 0)
                index = 0;
            prop.stringValue = fieldsNames[EditorGUILayout.Popup(label, index, fieldsNames)];
            if (GUI.changed)
            {
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        public static void FieldsPopup(string label, SerializedProperty target, Rect pos, System.Type classType, string propertyName)
        {
            var fields = classType.GetFields();

            string[] fieldsNames = new string[fields.Length + 1];
            fieldsNames[0] = "None";
            for (int i = 0; i < fields.Length; i++)
            {
                fieldsNames[i + 1] = fields[i].Name;
            }
            var prop = target.FindPropertyRelative(propertyName);
            int index = System.Array.FindIndex(fieldsNames, (s) => s == prop.stringValue);
            if (index < 0)
                index = 0;
            prop.stringValue = fieldsNames[EditorGUI.Popup(pos, label, index, fieldsNames)];
            if (GUI.changed)
            {
                target.serializedObject.ApplyModifiedProperties();
            }
        }

        public static List<string> GetAllProperties(UnityEngine.Object o)
        {
            var serializedObj = new SerializedObject(o);
            var props = new List<string>();
            var p = serializedObj.GetIterator();
            props.Add(p.name);
            while (p.Next(true))
            {
                props.Add(p.name);
            }
            return props;
        }

        public static void MakeTextureReadable(Texture2D map)
        {
            TextureImporter imp = (TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(map));
            if (!imp)
            {
                Debug.LogWarning("Invalid texture " + map);
                return;
            }
            if (!imp.isReadable)
            {
                imp.isReadable = true;
                imp.textureFormat = TextureImporterFormat.ARGB32;
                imp.textureType = TextureImporterType.Advanced;
                imp.SaveAndReimport();
            }
        }

        public static bool toolsHidden
        {
            get
            {
                System.Type type = typeof(Tools);
                FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
                return ((bool) field.GetValue(null));
            }
            set
            {
                System.Type type = typeof(Tools);
                FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
                field.SetValue(null, value);
            }
        }

        public static T[] LoadAllAssetsAtFolder<T>(string folderPath) where T : UnityEngine.Object
        {

            string sAssetFolderPath = folderPath;
            string sDataPath = Application.dataPath;
            string sFolderPath = sDataPath.Substring(0, sDataPath.Length - 6) + sAssetFolderPath;
            if (!System.IO.Directory.Exists(folderPath))
            {
                return new T[0];
            }
            string[] aFilePaths = System.IO.Directory.GetFiles(sFolderPath);
            List<T> filtered = new List<T>(aFilePaths.Length);
            for (var i = 0; i < aFilePaths.Length; i++)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(aFilePaths[i].Substring(sDataPath.Length - 6));
                if (asset)
                {
                    filtered.Add(asset);
                }
            }
            return filtered.ToArray();
        }

        public static void FreezeScene()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        public static void HideTools()
        {
            if (Tools.current != Tool.None)
            {
                lastTool = Tools.current;
                Tools.current = Tool.None;
            }
        }
        public static void RestoreTool()
        {
            if (lastTool != Tool.View && Tools.current != lastTool)
            {
                Tools.current = lastTool;
            }
        }

        public static System.Type[] GetAllDerivedTypes(System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }
            return result.ToArray();
        }

        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = GetAllDerivedTypes(System.AppDomain.CurrentDomain, (typeof(ScriptableObject))).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
            if (containerWinType == null)
                throw new System.MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
                throw new System.MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            var windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows)
            {
                var showmode = (int) showModeField.GetValue(win);
                if (showmode == 4)
                {
                    var pos = (Rect) positionProperty.GetValue(win, null);
                    return pos;
                }
            }
            throw new System.NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
        }

        public static void CenterOnMainWin(EditorWindow aWin)
        {
            var main = GetEditorMainWindowPos();
            var pos = aWin.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            aWin.position = pos;
        }
    }

}

