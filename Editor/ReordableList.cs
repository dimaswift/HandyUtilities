using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace HandyUtilities
{
    public abstract class ReordableList<T> : Editor
    {
        protected ReorderableList reorderableList;

        protected abstract string headerName { get; }

        protected abstract IList list { get; }

        protected abstract float elementHeight { get; }

        private void OnEnable()
        {
            reorderableList = new ReorderableList(list, typeof(T), true, true, true, true);
            reorderableList.elementHeight = elementHeight;

            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.drawElementCallback += DrawElement;

            reorderableList.onAddCallback += AddItem;
            reorderableList.onRemoveCallback += RemoveItem;
        }

        private void OnDisable()
        {
            reorderableList.drawHeaderCallback -= DrawHeader;
            reorderableList.drawElementCallback -= DrawElement;

            reorderableList.onAddCallback -= AddItem;
            reorderableList.onRemoveCallback -= RemoveItem;
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, headerName);
        }

        protected abstract void OnDrawItem(Rect rect, int index, bool active, bool focused);

        private void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            Undo.RecordObject(target, "Undo list " + name);
            EditorGUI.BeginChangeCheck();
            OnDrawItem(rect, index, active, focused);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void AddItem(ReorderableList l)
        {
            list.Add(default(T));
            EditorUtility.SetDirty(target);
        }

        private void RemoveItem(ReorderableList l)
        {
            list.RemoveAt(l.index);
            EditorUtility.SetDirty(target);
        }

    }

}

