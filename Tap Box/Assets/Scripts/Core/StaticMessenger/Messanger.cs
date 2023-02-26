using System;
using System.Linq;

namespace Core.StaticMessenger
{
    // No parameters
    public static class Messenger
    {
        public static void AddListener(string eventType, Action handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void AddListener<TReturn>(string eventType, Func<TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void RemoveListener(string eventType, Action handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void RemoveListener<TReturn>(string eventType, Func<TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void Broadcast(string eventType)
        {
            var invocationList = MessengerInternal.GetInvocationList<Action>(eventType);

            if (null == invocationList)
            {
                return;
            }

            foreach (var callback in invocationList)
            {
                callback.Invoke();
            }
        }

        public static void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall)
        {
            var invocationList = MessengerInternal.GetInvocationList<Func<TReturn>>(eventType);

            foreach (var result in invocationList.Select(del => del.Invoke()))
            {
                returnCall.Invoke(result);
            }
        }
    }

// One parameter
    public static class Messenger<T>
    {
        public static void AddListener(string eventType, Action<T> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void AddListener<TReturn>(string eventType, Func<T, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void RemoveListener(string eventType, Action<T> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void RemoveListener<TReturn>(string eventType, Func<T, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }


        public static void Broadcast(string eventType, T arg1)
        {
            var invocationList = MessengerInternal.GetInvocationList<Action<T>>(eventType);

            if (null == invocationList)
            {
                return;
            }

            foreach (var callback in invocationList)
            {
                callback.Invoke(arg1);
            }
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall)
        {
            var invocationList = MessengerInternal.GetInvocationList<Func<T, TReturn>>(eventType);

            foreach (var result in invocationList.Select(del => del.Invoke(arg1)))
            {
                returnCall.Invoke(result);
            }
        }
    }


// Two parameters
    public static class Messenger<T, TU>
    {
        public static void AddListener(string eventType, Action<T, TU> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void AddListener<TReturn>(string eventType, Func<T, TU, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void RemoveListener(string eventType, Action<T, TU> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void RemoveListener<TReturn>(string eventType, Func<T, TU, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void Broadcast(string eventType, T arg1, TU arg2)
        {
            var invocationList = MessengerInternal.GetInvocationList<Action<T, TU>>(eventType);

            if (null == invocationList)
            {
                return;
            }

            foreach (var callback in invocationList)
            {
                callback.Invoke(arg1, arg2);
            }
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, TU arg2, Action<TReturn> returnCall)
        {
            var invocationList = MessengerInternal.GetInvocationList<Func<T, TU, TReturn>>(eventType);

            if (null == invocationList)
            {
                return;
            }

            foreach (var result in invocationList.Select(del => del.Invoke(arg1, arg2)))
            {
                returnCall.Invoke(result);
            }
        }
    }

// Three parameters
    public static class Messenger<T, TU, TV>
    {
        public static void AddListener(string eventType, Action<T, TU, TV> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void AddListener<TReturn>(string eventType, Func<T, TU, TV, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        public static void RemoveListener(string eventType, Action<T, TU, TV> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void RemoveListener<TReturn>(string eventType, Func<T, TU, TV, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        public static void Broadcast(string eventType, T arg1, TU arg2, TV arg3)
        {
            var invocationList = MessengerInternal.GetInvocationList<Action<T, TU, TV>>(eventType);

            if (null == invocationList)
            {
                return;
            }

            foreach (var callback in invocationList)
            {
                callback.Invoke(arg1, arg2, arg3);
            }
        }

        public static void Broadcast<TReturn>(string eventType, T arg1, TU arg2, TV arg3, Action<TReturn> returnCall)
        {
            var invocationList = MessengerInternal.GetInvocationList<Func<T, TU, TV, TReturn>>(eventType);

            if (null == invocationList)
            {
                return;
            }

            foreach (var result in invocationList.Select(del => del.Invoke(arg1, arg2, arg3)))
            {
                returnCall.Invoke(result);
            }
        }
    }
}