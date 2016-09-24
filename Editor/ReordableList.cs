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

        protected virtual void OnDrawItem(Rect rect, int index, bool active, bool focused)
        {

        }

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

        protected void DoLayoutList()
        {
            reorderableList.DoLayoutList();
        }

        protected void DoList(Rect rect)
        {
            reorderableList.DoList(rect);
        }

        protected abstract T GetNewItem();

        protected virtual void AddItem(ReorderableList l)
        {
            list.Add(GetNewItem());
            EditorUtility.SetDirty(target);
        }

        protected virtual void RemoveItem(ReorderableList l)
        {
            list.RemoveAt(l.index);
            EditorUtility.SetDirty(target);
        }

    }

}

