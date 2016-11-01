using UnityEngine;
using System.Collections.Generic;

namespace HandyUtilities
{
    public static class Invoker
    {
        static TaskInvoker m_invoker;

        static List<Task> m_tasks = new List<Task>(100);

        public struct Task
        {
            public System.Action action;
            public float delayTime;
            public float currentTime;
            public bool invoked;
            public int id;
            public bool ignoreTimeScale;
            public bool loop;
            public float rate;
            public bool paused;
            public bool loopStarted;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if (m_invoker != null) return;
            m_invoker = new GameObject("Invoker").AddComponent<TaskInvoker>();
            Object.DontDestroyOnLoad(m_invoker.gameObject);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitLate()
        {
            var all = Object.FindObjectsOfType<TaskInvoker>();
            foreach (var i in all)
            {
                if (i != m_invoker)
                    Object.Destroy(i.gameObject);
            }
        }

        public static Task Get(int id)
        {
            for (int i = 0; i < m_tasks.Count; i++)
            {
                if (m_tasks[i].id == id)
                    return m_tasks[i];
            }
            return new Task() { action = () => Debug.LogWarning(string.Format("Task with id {0} not found!", id)) }; 
        }

        public static void Pause(System.Action action)
        {
            for (int i = 0; i < m_tasks.Count; i++)
            {
                var t = m_tasks[i];
                if (t.action.Equals(action))
                {
                    t.paused = true;
                    m_tasks[i] = t;
                    break;
                }
            }
        }

        public static void Pause(int id)
        {
            for (int i = 0; i < m_tasks.Count; i++)
            {
                var t = m_tasks[i];
                if (t.id == id)
                {
                    t.paused = true;
                    m_tasks[i] = t;
                    break;
                }
            }
        }

        public static void Resume(System.Action action)
        {
            for (int i = 0; i < m_tasks.Count; i++)
            {
                var t = m_tasks[i];
                if (t.action.Equals(action))
                {
                    t.paused = false;
                    m_tasks[i] = t;
                    break;
                }
            }
        }

        public static void Resume(int id)
        {
            for (int i = 0; i < m_tasks.Count; i++)
            {
                var t = m_tasks[i];
                if (t.id == id)
                {
                    t.paused = false;
                    m_tasks[i] = t;
                    break;
                }
            }
        }

        public static void Cancel(int id)
        {
            for(int i = 0; i < m_tasks.Count; i++)
            {
                if (m_tasks[i].id == id)
                {
                    m_tasks.RemoveAt(i);
                    break;
                }
            }
        }

        public static void Cancel(System.Action action)
        {
            for (int i = 0; i < m_tasks.Count; i++)
            {
                if (m_tasks[i].action.Equals(action))
                {
                    m_tasks.RemoveAt(i);
                    break;
                }
            }
        }

        public static bool Contains(System.Action action)
        {
            for (int i = 0; i < m_tasks.Count; i++)
            {
                if (m_tasks[i].action.Equals(action))
                    return true;
            }
            return false;
        }

        public static bool Contains(int id)
        {
            for (int i = 0; i < m_tasks.Count; i++)
            {
                if (m_tasks[i].id == id)
                    return true;
            }
            return false;
        }

        public static int Add(System.Action action, float delay, bool ignoreTimeScale = false)
        {
            var id = GetnerateUniqueID();
            m_tasks.Add(new Task()
            {
                id = id,
                action = action,
                delayTime = delay,
                currentTime = 0,
                invoked = false,
                ignoreTimeScale = ignoreTimeScale,
                loop = false
            });
            return id;
        }

        public static int AddLoop(System.Action action, float delay, float rate, bool ignoreTimeScale = false)
        {
            var id = GetnerateUniqueID();
            m_tasks.Add(new Task()
            {
                id = id,
                action = action,
                delayTime = delay,
                currentTime = 0,
                invoked = false,
                ignoreTimeScale = ignoreTimeScale,
                rate = rate,
                loop = true,
                loopStarted = false
            });
            return id;
        }

        static int GetnerateUniqueID()
        {
            var id = Random.Range(0, int.MaxValue - 1);
            for (int i = 0; i < m_tasks.Count; i++)
            {
                if (m_tasks[i].id == id)
                    return GetnerateUniqueID();
            }
            return id;
        }

        public static void Update()
        {
            var count = m_tasks.Count;
            bool invoked = false;
            for (int i = 0; i < count; i++)
            {
                var task = m_tasks[i];
                if(!task.paused)
                    task.currentTime += task.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                if(task.currentTime >= task.delayTime)
                {
                    if (task.loop)
                    {
                        task.currentTime = 0;
                        if (!task.loopStarted)
                        {
                            task.loopStarted = true;
                            task.delayTime = task.rate;
                        }
                    }
                    else
                    {
                        invoked = true;
                        task.invoked = true;
                    }
                    task.action();
                   
                }
                m_tasks[i] = task;
            }
            if(invoked)
            {
                m_tasks.RemoveAll(t => t.invoked);
            }
        }
    }

}
