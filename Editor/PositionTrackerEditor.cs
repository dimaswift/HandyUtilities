using UnityEngine;
using System.Collections;
using UnityEditor;
namespace HandyUtilities
{
    [CustomEditor(typeof(PositionTrackerData))]
    public class PositionTrackerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var data = target as PositionTrackerData;
            if (data == null || data.positions == null) return;
            EditorGUILayout.LabelField("Points: " + data.positions.Count);
            if(GUILayout.Button("Build"))
            {
                var root = new GameObject(data.name);
                root.transform.position = data.startPoint;
                for (int i = 0; i < data.positions.Count; i++)
                {
                    var gizmo = Instantiate(data.gizmo);
                    gizmo.transform.SetParent(root.transform);
                    gizmo.transform.position = data.positions[i];
                    gizmo.transform.eulerAngles = data.positions[i];
                    gizmo.name = i.ToString();
                }
            }
        }
    }

}
