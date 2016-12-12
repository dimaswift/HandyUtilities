using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HandyUtilities
{
    public class EditorCameraAdjuster : EditorWindow
    {
        Vector3 m_camEuler;
        [MenuItem("Handy Utilities/Adjust Scene Camera")]
        public static void Open()
        {
            var w = GetWindow<EditorCameraAdjuster>(true, "Camera Angle", true);
            w.minSize = new Vector2(300, 60);
            w.maxSize = new Vector2(300, 60);
            w.Show();
            HandyEditor.CenterOnMainWin(w);
            w.m_camEuler = SceneView.lastActiveSceneView.rotation.eulerAngles;
        }

        void OnGUI()
        {
            m_camEuler = EditorGUILayout.Vector3Field("", m_camEuler);
            if(GUILayout.Button("Apply"))
            {
                SceneView.lastActiveSceneView.rotation = Quaternion.Euler(m_camEuler);


            }
        }
    }

}
