using UnityEngine;
using UnityEditor;
using System;

namespace HandyUtilities
{
    public class ConfirmationTool : EditorWindow
    {
        Action<object[]> m_callback;

        [SerializeField]
        string m_okay;
      
        Argument[] m_args;

        [SerializeField]
        object[] m_results;

        public static ConfirmationTool OpenWithArguments(string title, string okayMessage, Action<object[]> callback, params Argument[] args)
        {
            var w = GetWindow<ConfirmationTool>(true, title, true);
            HandyEditor.CenterOnMainWin(w);
            w.m_callback = callback;
            w.m_okay = okayMessage;
            w.SetArguments(args);
            w.Show(true);
            return w;
        }

        public void SetArguments(params Argument[] args)
        {
            this.m_args = args;
            m_results = new object[args.Length];
            minSize = new Vector2(300 + (16 * args.Length), 90);
            maxSize = minSize;
        }

        void OnGUI()
        {
            if (m_callback == null) Close();

            for (int i = 0; i < m_args.Length; i++)
            {
                m_args[i].DoGUILayout();
                m_results[i] = m_args[i].value;
            }

            GUILayout.Space(10);

            if (GUILayout.Button(m_okay))
            {
                m_callback(m_results);
                Close();
            }
        }

        public abstract class Argument
        {
            public string name;

            public object value;

            public Argument(string name) { this.name = name; }

            public abstract void DoGUILayout();
        }

        public class Slider : Argument
        {

            public float left, right;

            public Slider(string name, float defaultValue = 0, float left = 0, float right = 1) : base(name)
            {
                this.left = left;
                this.right = right;
                this.value = defaultValue;
            }

            public override void DoGUILayout()
            {
                this.value = EditorGUILayout.Slider(this.name, (float)this.value, this.left, this.right);
            }
        }

        public class IntSlider : Argument
        {
            public int left, right;

            public IntSlider(string name, int defaultValue = 0, int left = 0, int right = 1) : base(name)
            {
                this.left = left;
                this.right = right;
                this.value = defaultValue;
            }

            public override void DoGUILayout()
            {
                this.value = EditorGUILayout.IntSlider(this.name, (int) this.value, this.left, this.right);
            }
        }

        public class Label : Argument
        {

            public Label(string name, string defaultValue = "") : base(name)
            {
                this.value = defaultValue;
            }

            public override void DoGUILayout()
            {
                this.value = EditorGUILayout.TextField(this.name, (string) this.value);
            }
        }

        public class Toggle : Argument
        {
            public Toggle(string name, bool defaultValue = false) : base(name)
            {
                this.name = name;
                this.value = defaultValue;
            }

            public override void DoGUILayout()
            {
                this.value = EditorGUILayout.Toggle(this.name, (bool) this.value);
            }
        }
    }
}
