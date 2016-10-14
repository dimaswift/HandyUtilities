using UnityEngine;
using UnityEditor;

namespace HandyUtilities
{
    public class NamingTool : EditorWindow
    {
        System.Action<string> m_callback;
        [SerializeField]
        string m_message, m_name, m_okay;

        public static void Open(System.Action<string> callback, string title = "Enter Name", string message = "", string defaultName = "stuff", string okayButton = "Ok")
        {
            var w = GetWindow<NamingTool>(true, title, true);
            HandyEditor.CenterOnMainWin(w);
            w.m_message = message;
            w.m_callback = callback;
            w.m_okay = okayButton;
            w.m_name = defaultName;
            w.minSize = new Vector2(300, 90);
            w.maxSize = new Vector2(300, 90);
            w.Show(true);
        }

        void OnGUI()
        {
            Undo.RecordObject(this, "Naming Tool");
            EditorGUILayout.LabelField(m_message);
            m_name = EditorGUILayout.TextField(m_name);
            GUILayout.Space(10);
            if (GUILayout.Button(m_okay))
            {
                m_callback(m_name);
                Close();
            }
        }
    }
}
