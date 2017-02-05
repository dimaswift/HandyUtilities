using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CodeGenerator;
namespace HandyUtilities
{
    public class ComponentPicker : EditorWindow
    {
        Vector2 scrollPos;
        System.Action<List<ComponentCell>, GameObject> onSubmit;
        float cellSize = 30;
        GUIStyle typeLabel;
        GameObject root;
        float scrollHeight;

        public static void Open(System.Action<List<ComponentCell>, GameObject> onSubmit, GameObject root, Class elementsClass = null)
        {
            var w = GetWindow<ComponentPicker>(false, "Select Elements you want", true);
            w.root = root;
            w.Show(true);
            w.minSize = new Vector2(600, 600);
            w.maxSize = new Vector2(1600, 1600);
            HandyEditor.CenterOnMainWin(w);
            w.onSubmit = onSubmit;
            var container = ComponentPickerContainer.Instance;
            container.root = new ComponentContainer(root, 1);
            if (elementsClass != null)
                SelectExisting(container.root, elementsClass);
            EditorUtility.SetDirty(ComponentPickerContainer.Instance);
            Undo.undoRedoPerformed += w.OnUndo;
        }

        static void SelectExisting(ComponentContainer cont, Class elementsClass)
        {
            foreach (var m in elementsClass.members)
            {
                foreach (var c in cont.components)
                {
                    if (c.fieldName == m.name && c.type == m.type)
                        c.selected = true;
                }
            }
            foreach (var c in cont.children)
            {
                SelectExisting(c, elementsClass);
            }
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndo;
        }

        void OnEnable()
        {
            typeLabel = new GUIStyle() { alignment = TextAnchor.MiddleRight, fontStyle = FontStyle.Bold };
        }



        void OnGUI()
        {
            var settings = ComponentPickerContainer.Instance;
            Undo.RecordObject(settings, "Components Container");

            var root = ComponentPickerContainer.Instance.root;
            float scrollViewSize = position.height - 150;
            var rect = new Rect(5, 5, position.width - 10, scrollViewSize);
        
            var scrollView = new Rect(rect.x, 20, position.width - 25, scrollHeight);
            scrollPos = GUI.BeginScrollView(new Rect(5, 20, rect.width, rect.height), scrollPos, scrollView, false, true);

            rect.y += 20;
            var elementRect = rect;
            elementRect.width = scrollView.width;
            elementRect.x += 15;
            scrollHeight = 0;
            
            DrawComponentContainer(root, ref elementRect);
            
            GUI.EndScrollView();


            if (GUI.Button(new Rect(rect.x, position.height - 125, rect.width, 20), "Add Type To Names"))
            {
                AddTypeToNames(root);
                EditorUtility.SetDirty(settings);
            }
            if (GUI.Button(new Rect(rect.x, position.height - 100, rect.width, 20), "Remove Duplicates"))
            {
                RemoveDuplicates(root);
                EditorUtility.SetDirty(settings);  
            }
            if (GUI.Button(new Rect(rect.x, position.height - 75, rect.width * .5f, 20), "Open All"))
            {
                OpenAll(root);
                EditorUtility.SetDirty(settings);
            }
            if (GUI.Button(new Rect(rect.x + rect.width * .5f, position.height - 75, rect.width * .5f, 20), "Collapse All"))
            {
                CollapseAll(root);
                EditorUtility.SetDirty(settings);
            }
            if (GUI.Button(new Rect(rect.x, position.height - 50, rect.width, 20), "Deselct All"))
            {
                DeselectAll(root);
                EditorUtility.SetDirty(ComponentPickerContainer.Instance);
            }
            if (GUI.Button(new Rect(rect.x, position.height - 25, rect.width, 20), "Submit"))
            {
                var list = GetComponents(root, new List<ComponentCell>());
                onSubmit(list, root.gameObject);
                Close();
            }
            EditorUtility.SetDirty(ComponentPickerContainer.Instance);
        }

        void DeselectAll(ComponentContainer cont)
        {
            foreach (var c in cont.components)
            {
                c.selected = false;
            }
            foreach (var c in cont.children)
            {
                DeselectAll(c);
            }
        }

        void AddTypeToNames(ComponentContainer cont)
        {
            foreach (var c in cont.components)
            {
                c.fieldName += c.type;
            }
            foreach (var c in cont.children)
            {
                AddTypeToNames(c);
            }
        }

        void OpenAll(ComponentContainer container)
        {
            container.show = true;
            foreach (var c in container.children)
            {
                OpenAll(c);
            }
        }

        void CollapseAll(ComponentContainer container)
        {
            container.show = false;
            foreach (var c in container.children)
            {
                CollapseAll(c);
            }
        }

        int GetIndexCount(string str)
        {
            int c = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int r = 0;
                if (int.TryParse(str[i].ToString(), out r))
                    c++;
                else break;
            }
            return c;
        }

        void RemoveDuplicates(ComponentContainer container)
        {
            foreach (var c in container.children)
            {
                RemoveDuplicates(c);
            }
            foreach (var cell in container.components)
            {
                foreach (var cell2 in container.components)
                {
                    if (cell2.selected && cell.selected && cell.fieldName == cell2.fieldName &&
                        cell.component != cell2.component)
                    {
                        if (cell.type == "Image" && cell2.type == "Button")
                            cell.selected = false;
                        else if (cell2.type == "Image" && cell.type == "Button")
                            cell2.selected = false;
                        else if (cell.type == "RectTransform" && cell2.type == "Image")
                            cell2.selected = false;
                        else if (cell.type == "RectTransform" && cell2.type == "Text")
                            cell.selected = false;
                        else if (cell.type == "RectTransform" && cell2.type == "Button")
                            cell.selected = false;
                        else cell.selected = false;
                    }
                }
            }
        }

        void OnUndo()
        {
            Repaint();
        }

        List<ComponentCell> GetComponents(ComponentContainer cont, List<ComponentCell> list)
        {
            cont.components.RemoveAll(c => c.selected == false);
            foreach (var c in cont.components)
            {
                list.Add(c);
            }
            foreach (var c in cont.children)
            {
                list = GetComponents(c, list);
            }
            return list;
        }



        void DrawComponentContainer(ComponentContainer container, ref Rect rect)
        {
            scrollHeight += 20;
            rect.x = container.depth * 15;
            rect.y += 17;
            var color = GUI.color;
            container.show = EditorGUI.Foldout(new Rect(rect.x + container.depth, rect.y, rect.width, 16), container.show, container.gameObject.name, true);
            if(container.show)
            {
                rect.y += 20;
                var x = rect.x + 15;
                foreach (var c in container.components)
                {
                    scrollHeight += cellSize + 5;
                    GUI.color = c.selected ? Color.green.SetAlpha(.3f) : Color.red.SetAlpha(.3f);
                    GUI.enabled = c.selected;
                    GUI.Box(new Rect(x, rect.y, rect.width, cellSize - 3), "");
                    GUI.color = color;

                    c.fieldName = GUI.TextField(new Rect(x + 25, rect.y + 5, 200, 22), c.fieldName, EditorStyles.largeLabel);
                    var o = EditorGUIUtility.ObjectContent(c.component, c.component.GetType());
                    GUI.DrawTexture(new Rect(x + 5, rect.y + 3, 20, 20), o.image);
                    GUI.Label(new Rect(rect.width - 35, rect.y + 5, 5, 16), string.Format("{0} ({1})", c.component.name, c.type), typeLabel);
                    GUI.enabled = true;
                    c.selected = GUI.Toggle(new Rect(rect.width - 20, rect.y + 5, 20, 20), c.selected, "");
                    rect.y += cellSize;
                }
                rect.y -= 10;
                foreach (var c in container.children)
                {
                    DrawComponentContainer(c, ref rect);
                }
            }
        }
    }

    [System.Serializable]
    public class ComponentContainer
    {
        public int depth;
        public bool show = true;
        public List<ComponentCell> components;
        public List<ComponentContainer> children;
        public GameObject gameObject { get { return EditorUtility.InstanceIDToObject(gameObjectId) as GameObject; } }
        int gameObjectId;
        public ComponentContainer(GameObject gameObject, int depth)
        {
            this.depth = depth;
            gameObjectId = gameObject.GetInstanceID();
            components = new List<ComponentCell>();
            var childrenTransfroms = gameObject.transform.GetChildren();
            children = new List<ComponentContainer>();
            foreach (var c in childrenTransfroms)
            {
                children.Add(new ComponentContainer(c.gameObject, depth + 1));
            }
            var allComponents = gameObject.GetComponents<Component>();
            foreach (var c in allComponents)
            {
                if(c is CanvasRenderer == false)
                    components.Add(new ComponentCell(c, gameObject));
            }
        }
    }

    [System.Serializable]
    public class ComponentCell
    {
        public Component component { get { return EditorUtility.InstanceIDToObject(componentID) as Component; } }
        public int componentID;
        public bool selected;
        public string objectName;
        public string fieldName;
        public string type;
        public Color color;

        public ComponentCell(Component c, GameObject screen)
        {
            componentID = c.GetInstanceID();
            var t = c.GetType().ToString().Split('.');
            type = t[t.Length - 1];
            objectName = component.name;
            fieldName = System.Text.RegularExpressions.Regex.Replace(component.name, @"[\s*\(\)-]", "");
            fieldName = char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
        }
    }

}
