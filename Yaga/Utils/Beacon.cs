using System;

namespace Yaga.Utils
{
    /// <summary>
    /// Wrapper around event, allows easily unsubscribe using <see cref="Reflector"/>.
    /// Dispose <see cref="Reflector"/> to unsubscribe from beacon.
    /// </summary>
    public class Beacon
    {
        private Action _action;

        public Beacon()
        {
            _action = delegate { };
        }

        public Beacon(Action action)
        {
            _action = action;
        }

        public Reflector Add(Action action)
        {
            _action += action;
            return new Reflector(() => _action -= action);
        }

        public void Execute() => _action?.Invoke();
        public void Remove(Action action) => _action -= action;
        public static Reflector operator +(Beacon a, Action action) => a.Add(action);
    }

    /// <summary>
    /// Wrapper around event, allows easily unsubscribe using <see cref="Reflector"/>.
    /// Dispose <see cref="Reflector"/> to unsubscribe from beacon.
    /// </summary>
    public class Beacon<T1>
    {
        private Action<T1> _action;

        public Beacon()
        {
            _action = delegate { };
        }

        public Beacon(Action<T1> action)
        {
            _action = action;
        }

        public Reflector Add(Action<T1> action)
        {
            _action += action;
            return new Reflector(() => _action -= action);
        }

        public void Execute(T1 arg1) => _action?.Invoke(arg1);
        public void Remove(Action<T1> action) => _action -= action;
        public static Reflector operator +(Beacon<T1> a, Action<T1> action) => a.Add(action);
    }
    
    /// <summary>
    /// Wrapper around event, allows easily unsubscribe using <see cref="Reflector"/>.
    /// Dispose <see cref="Reflector"/> to unsubscribe from beacon.
    /// </summary>
    public class Beacon<T1, T2>
    {
        private Action<T1, T2> _action;

        public Beacon()
        {
            _action = delegate { };
        }

        public Beacon(Action<T1, T2> action)
        {
            _action = action;
        }

        public Reflector Add(Action<T1, T2> action)
        {
            _action += action;
            return new Reflector(() => _action -= action);
        }

        public void Execute(T1 arg1, T2 arg2) => _action?.Invoke(arg1, arg2);
        public void Remove(Action<T1, T2> action) => _action -= action;
        public static Reflector operator +(Beacon<T1, T2> a, Action<T1, T2> action) => a.Add(action);
    }
    
    /// <summary>
    /// Wrapper around event, allows easily unsubscribe using <see cref="Reflector"/>.
    /// Dispose <see cref="Reflector"/> to unsubscribe from beacon.
    /// </summary>
    public class Beacon<T1, T2, T3>
    {
        private Action<T1, T2, T3> _action;

        public Beacon()
        {
            _action = delegate { };
        }

        public Beacon(Action<T1, T2, T3> action)
        {
            _action = action;
        }

        public Reflector Add(Action<T1, T2, T3> action)
        {
            _action += action;
            return new Reflector(() => _action -= action);
        }

        public void Execute(T1 arg1, T2 arg2, T3 arg3) => _action?.Invoke(arg1, arg2, arg3);
        public void Remove(Action<T1, T2, T3> action) => _action -= action;
        public static Reflector operator +(Beacon<T1, T2, T3> a, Action<T1, T2, T3> action) => a.Add(action);
    }
}