using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace HandyUtilities
{
    public class ComponentPicker : EditorWindow
    {
        Vector2 scrollPos;
        System.Action<List<ComponentCell>, GameObject> onSubmit;
        float cellSize = 40;
        GUIStyle typeLabel;
        GameObject screen;

        public static void Open(System.Action<List<ComponentCell>, GameObject> onSubmit, List<Component> components, GameObject screen)
        {
            var w = GetWindow<ComponentPicker>(true, "Select Elements you want", true);
            w.screen = screen;
            w.Show(true);
            w.minSize = new Vector2(600, 600);
            w.maxSize = new Vector2(600, 600);
            HandyEditor.CenterOnMainWin(w);
            w.onSubmit = onSubmit;
            var container = ComponentPickerContainer.Instance;
            container.cells.Clear();
            foreach (var c in components)
            {
                container.cells.Add(new ComponentCell(c, screen));
            }
            EditorUtility.SetDirty(ComponentPickerContainer.Instance);
            Undo.undoRedoPerformed += w.OnUndo;
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
            Undo.RecordObject(ComponentPickerContainer.Instance, "Components Container");
            var cells = ComponentPickerContainer.Instance.cells;
            float scrollViewSize = position.height - 120;
            var rect = new Rect(5, 5, position.width - 10, scrollViewSize);
            GUI.Box(rect, "Found " + cells.Count + " elements:", EditorStyles.boldLabel);
            var scrollView = new Rect(rect.x, 20, position.width - 25, (cellSize * cells.Count) + 45);
            scrollPos = GUI.BeginScrollView(new Rect(5, 20, rect.width, rect.height), scrollPos, scrollView, false, true);

            rect.y += 20;
            var elementRect = rect;
            elementRect.width = scrollView.width;
            for (int i = 0; i < cells.Count; i++)
            {
                var cell = cells[i];
                DrawComponent(cell, ref elementRect);
            }
            GUI.EndScrollView();


            if (GUI.Button(new Rect(rect.x, position.height - 100, rect.width, 20), "Add Type To Names"))
            {
                AddTypeToNames();
            }
            if (GUI.Button(new Rect(rect.x, position.height - 75, rect.width, 20), "Remove Duplicates"))
            {
                RemoveDuplicates();
            }
            if (GUI.Button(new Rect(rect.x, position.height - 50, rect.width, 20), "Remove All"))
            {
                Undo.RecordObject(ComponentPickerContainer.Instance, "Components Container");
                cells.ForEach(c => c.selected = false);
                EditorUtility.SetDirty(ComponentPickerContainer.Instance);
            }
            if (GUI.Button(new Rect(rect.x, position.height - 25, rect.width, 20), "Submit"))
            {
                Submit();
            }
            EditorUtility.SetDirty(ComponentPickerContainer.Instance);
        }

        void AddTypeToNames()
        {
            Undo.RecordObject(ComponentPickerContainer.Instance, "Components Container");
            var cells = ComponentPickerContainer.Instance.cells;
            foreach (var cell in cells)
            {
                cell.fieldName += cell.type;
            }
            EditorUtility.SetDirty(ComponentPickerContainer.Instance);
        }

        void RemoveDuplicates()
        {
            Undo.RecordObject(ComponentPickerContainer.Instance, "Components Container");
            var cells = ComponentPickerContainer.Instance.cells;
            foreach (var cell in cells)
            {
                foreach (var cell2 in cells)
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
                    }
                }
            }
            EditorUtility.SetDirty(ComponentPickerContainer.Instance);
        }

        void OnUndo()
        {
            Repaint();
        }

        void Submit()
        {
            ComponentPickerContainer.Instance.cells.RemoveAll(c => c.selected == false);
            onSubmit(ComponentPickerContainer.Instance.cells, screen);
            Close();
        }

        bool DrawComponent(ComponentCell c, ref Rect rect)
        {
            rect.y += 15;
            var color = GUI.color;
            float depth = c.depth * 15;
            GUI.color = c.selected ? Color.green.SetAlpha(.3f) : Color.red.SetAlpha(.3f);
            GUI.enabled = c.selected;
            GUI.Box(new Rect(rect.x + depth, rect.y, rect.width, cellSize - 3), "");
            GUI.color = color;

            c.fieldName = GUI.TextField(new Rect(rect.x + depth + 5, rect.y + 8, 200, 22), c.fieldName, EditorStyles.largeLabel);
 
            GUI.Label(new Rect(rect.width - 35, rect.y + 8, 5, 16), string.Format("{0} ({1})", c.component.name, c.type), typeLabel);
            GUI.enabled = true;
            c.selected = GUI.Toggle(new Rect(rect.width - 20, rect.y + 10, 20, 20), c.selected, "");
            rect.y += cellSize - 15;
            return c.selected;
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
        public int depth;

        public ComponentCell(Component c, GameObject screen)
        {

            componentID = c.GetInstanceID();
            var t = c.GetType().ToString().Split('.');
            type = t[t.Length - 1];
            objectName = component.name;
            selected = true;
            fieldName = System.Text.RegularExpressions.Regex.Replace(component.name, @"[\s*\(\)-]", "");
            fieldName = char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
            var parent = c.transform.parent;
            while (parent != null && parent != screen.transform)
            {
                depth++;
                parent = parent.parent;
            }
        }
    }

}
