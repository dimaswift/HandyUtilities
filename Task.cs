using UnityEngine;
using System.Collections.Generic;

namespace HandyUtilities
{
    public delegate void RoutineHandler(float normalizedTime);

    public static class Invoker
    {
        static TaskInvoker m_invoker;

        static List<Task> m_tasks = new List<Task>(100);
        static List<Routine> m_routines = new List<Routine>(100);

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

        public struct Routine
        {
            public RoutineHandler method;
            public float currentTime;
            public float duration;
            public bool finished;
            public int id;
            public bool ignoreTimeScale;
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

        public static void CancelRoutine(int id)
        {
            for (int i = 0; i < m_routines.Count; i++)
            {
                if (m_routines[i].id == id)
                {
                    m_routines.RemoveAt(i);
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
            var id = GenerateUniqueID();
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

        public static Routine GetRoutine(RoutineHandler r)
        {
            var c = m_routines.Count;
            for (int i = 0; i < c; i++)
            {
                if (m_routines[i].method == r)
                    return m_routines[i];
            }
            return default(Routine);
        }

        public static int StartRoutine(RoutineHandler routine, float duration, bool ignoreTimeScale = false)
        {
            var id = GenerateUniqueID();
            if (duration == 0)
            {
                routine(1);
                return id;
            }
            var current = 0f;
            var r = GetRoutine(routine);
            if (r.id != 0)
            {
                current = duration - r.currentTime;
                m_routines.Remove(r);
            }
            m_routines.Add(new Routine()
            {
                id = id,
                method = routine,
                currentTime = current,
                finished = false,
                duration = duration,
                ignoreTimeScale = ignoreTimeScale
            });
            return id;
        }



        public static int AddLoop(System.Action action, float delay, float rate, bool ignoreTimeScale = false)
        {
            var id = GenerateUniqueID();
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

        static int GenerateUniqueID()
        {
            var id = Random.Range(0, int.MaxValue - 1);
            for (int i = 0; i < m_tasks.Count; i++)
            {
                if (m_tasks[i].id == id)
                    return GenerateUniqueID();
            }
            return id;
        }

        static void ProcessTasks()
        {
            var count = m_tasks.Count;
            bool invoked = false;
            for (int i = 0; i < count; i++)
            {
                var task = m_tasks[i];
                if (!task.paused)
                    task.currentTime += task.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                if (task.currentTime >= task.delayTime)
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
            if (invoked)
            {
                m_tasks.RemoveAll(t => t.invoked);
            }
        }

        static void ProcessRoutines()
        {
            var count = m_routines.Count;
            bool finished = false;
       
            for (int i = 0; i < count; i++)
            {
                var routine = m_routines[i];
                var d = routine.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                var t = routine.currentTime / routine.duration;
               
                if(t >= 1)
                {
                    finished = true;
                    routine.finished = true;
                    routine.method(1);
                }
                else routine.method(t);
                routine.currentTime += d;
                m_routines[i] = routine;
            }
            if (finished)
            {
                m_routines.RemoveAll(t => t.finished);
            }
        }

        public static void Update()
        {
            ProcessTasks();
            ProcessRoutines();
        }
    }

}
