using System;

namespace HandyUtilities.Events
{
    public sealed class GameEvent
    {
        event Action _event;
        public void AddListener(Action action)
        {
            _event += action;
        }
        public void RemoveListener(Action action)
        {
            _event -= action;
        }
        public void RemoveAllListeners(Action action)
        {
            _event = null;
        }
        public void RaiseEvent()
        {
            if (_event != null)
                _event();
        }
    }

    public sealed class GameEvent<T>
    {
        event Action<T> _event;
        public void AddListener(Action<T> action)
        {
            _event += action;
        }
        public void RemoveListener(Action<T> action)
        {
            _event -= action;
        }
        public void RemoveAllListeners(Action<T> action)
        {
            _event = null;
        }
        public void RaiseEvent(T arg)
        {
            if (_event != null)
                _event(arg);
        }
    }

    public sealed class GameEvent<T1, T2>
    {
        event Action<T1, T2> _event;
        public void AddListener(Action<T1, T2> action)
        {
            _event += action;
        }
        public void RemoveListener(Action<T1, T2> action)
        {
            _event -= action;
        }
        public void RemoveAllListeners(Action<T1, T2> action)
        {
            _event = null;
        }
        public void RaiseEvent(T1 arg, T2 arg2)
        {
            if (_event != null)
                _event(arg, arg2);
        }
    }

    public sealed class GameEvent<T1, T2, T3>
    {
        event Action<T1, T2, T3> _event;
        public void AddListener(Action<T1, T2, T3> action)
        {
            _event += action;
        }
        public void RemoveListener(Action<T1, T2, T3> action)
        {
            _event -= action;
        }
        public void RemoveAllListeners(Action<T1, T2, T3> action)
        {
            _event = null;
        }
        public void RaiseEvent(T1 arg, T2 arg2, T3 arg3)
        {
            if (_event != null)
                _event(arg, arg2, arg3);
        }
    }
}
