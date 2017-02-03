﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CodeGenerator;

namespace HandyUtilities
{
    public class ElementsBinder : Editor
    {
        static MonoScript script;
        static string scriptPath;

        [MenuItem("CONTEXT/Component/Bind Elements")]
        static void BindElements(MenuCommand c)
        {
            var mono = c.context as MonoBehaviour;
            if (mono == null)
                return;
            script = MonoScript.FromMonoBehaviour(mono);
            scriptPath = mono.GetScriptPath();
            ComponentPicker.Open(OnElementsPicked, new List<Component>(mono.GetComponentsInChildren<Component>()), mono.gameObject);
        }

        static Field CreateField(ComponentCell cell)
        {
            var field = new Field();
            field.type = GetTypeName(cell.component.GetType());
            field.name = "m_" + cell.fieldName;
            field.AddAttributes("SerializeField");
            return field;
        }

        static Property CreateProperty(ComponentCell cell)
        {
            var prop = new Property();
            prop.protectionLevel = "public";
            prop.SetReadonly(true);
            prop.type = GetTypeName(cell.component.GetType());
            prop.name = cell.fieldName;
            prop.SetFieldName("m_" + cell.fieldName);
            return prop;
        }

        public static string GetTypeName(System.Type t)
        {
            var n = t.ToString().Split('.');
            return n[n.Length - 1];
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptsCompiled()
        {
            var container = ComponentPickerContainer.Instance;
            if (container.pendingScriptCompile)
            {
                var target = EditorUtility.InstanceIDToObject(container.targetID) as GameObject;
                HandyEditor.SendMessage(target, "BindElements");
              //  target.SendMessage("BindElements", SendMessageOptions.DontRequireReceiver);
                container.pendingScriptCompile = false;
                EditorUtility.SetDirty(container);
            }
        }

        static void OnElementsPicked(List<ComponentCell> components, GameObject target)
        {
            Class elementsClass = new Class("Elements", "public", "");
            elementsClass.AddAttribute("System.Serializable");
            var bindMethod = new Method("void", "Bind", "", "public", "", new Method.Parameter(script.name, "mono"));

            foreach (var c in components)
            {
                var f = CreateField(c);
                elementsClass.AddMember(CreateField(c));
                bindMethod.AddLine(string.Format(@"{0} = mono.transform.FindChild(""{1}"").GetComponent<{2}>();", f.name, c.component.transform.GetPath(target.transform), f.type));
            }
            bindMethod.AddLine("#if UNITY_EDITOR");
            bindMethod.AddLine("UnityEditor.EditorUtility.SetDirty(mono);");
            bindMethod.AddLine("#endif");
            elementsClass.AddMember(bindMethod);
            var bindElementsMethod = new Method("void", "BindElements", "", "public", "");
            bindElementsMethod.AddLine("elements.Bind(this);");
    
            foreach (var c in components)
            {
                var p = CreateProperty(c);
                elementsClass.AddMember(p);
            }

            var lines = new List<string>(script.text.Split('\n'));

            int startIndex = -1;
            int indent = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Contains("class " + script.name + " "))
                {
                    startIndex = line.Contains("{") ? i + 1 : i + 2;
                    foreach (var c in line)
                    {
                        if (c == ' ')
                            indent++;
                    }
                }
            }

            indent /= 4;
            var indentString = "  ";
            for (int i = 0; i < indent; i++)
            {
                indentString += "   ";
            }

            lines.Insert(startIndex, "        #endregion Elements");
            lines.Insert(startIndex, "");
            lines.Insert(startIndex, elementsClass.ToString(indent));
            lines.Insert(startIndex, bindElementsMethod.ToString(indent));
            lines.Insert(startIndex, "");
            lines.Insert(startIndex, new Property("Elements", "elements", "public", "m_elements", "").SetReadonly(true).ToString(indent));
            lines.Insert(startIndex, "");
            lines.Insert(startIndex, indentString + "Elements m_elements;");
            lines.Insert(startIndex, indentString + "[SerializeField]");
            lines.Insert(startIndex, "");
            lines.Insert(startIndex, "        #region Elements");
            lines.Insert(startIndex, "");

            ComponentPickerContainer.Instance.pendingScriptCompile = true;
            ComponentPickerContainer.Instance.targetID = target.GetInstanceID();
            System.IO.File.WriteAllText(Helper.ConvertToAbsolutePath(scriptPath), string.Join("\n", lines.ToArray()));
            EditorUtility.SetDirty(ComponentPickerContainer.Instance);
            AssetDatabase.Refresh();
        }
    }

}
