using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.MessengerStatic
{
    internal static class MessengerInternal
    {
        private static readonly Dictionary<string, Delegate> EventTable = new();

        public static void AddListener(string eventType, Delegate callback)
        {
            if (!EventTable.ContainsKey(eventType))
                EventTable.Add(eventType, null);
            
            EventTable[eventType] = Delegate.Combine(EventTable[eventType], callback);
        }

        public static void RemoveListener(string eventType, Delegate handler)
        {
            EventTable[eventType] = Delegate.Remove(EventTable[eventType], handler);
            
            if (EventTable[eventType] == null)
                EventTable.Remove(eventType);
        }

        public static T[] GetInvocationList<T>(string eventType)
        {
            if (!EventTable.TryGetValue(eventType, out var d))
                return null;

            return d?.GetInvocationList().Cast<T>().ToArray();
        }
    }
}