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
            EditorGUILayout.LabelField("Points: " + data.positions.Count);
            if(GUILayout.Button("Build"))
            {
                var root = new GameObject(data.name);
                root.transform.position = data.startPoint;
                for (int i = 0; i < data.positions.Count; i++)
                {
                    var gizmo = GameObject.CreatePrimitive(data.gizmoType);
                    gizmo.transform.localScale = new Vector3(data.gizmoSize, data.gizmoSize, data.gizmoSize);
                    gizmo.transform.SetParent(root.transform);
                    gizmo.transform.position = data.positions[i];
                    gizmo.transform.eulerAngles = data.positions[i];
                    gizmo.name = i.ToString();
                }
            }
        }
    }

}
