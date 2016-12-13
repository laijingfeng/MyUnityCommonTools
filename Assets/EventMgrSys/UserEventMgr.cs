using System.Collections.Generic;
using System;
namespace Jerry
{

    public class UserEventMgr
    {
        private static Dictionary<string, Action<object[]>> _dicEvents = new Dictionary<string, Action<object[]>>();

        public static void AddEvent(string eventName, Action<object[]> listener)
        {
            if (string.IsNullOrEmpty(eventName)
                || listener == null)
            {
                return;
            }

            if (_dicEvents.ContainsKey(eventName))
            {
                _dicEvents[eventName] += listener;
            }
            else
            {
                _dicEvents[eventName] = listener;
            }
        }

        public static void RemoveEvent(string eventName, Action<object[]> listener)
        {
            if (string.IsNullOrEmpty(eventName)
                || listener == null)
            {
                return;
            }

            if (_dicEvents.ContainsKey(eventName))
            {
                _dicEvents[eventName] -= listener;
            }
        }

        public static void DispatchEvent(string eventName, object[] args = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            if (_dicEvents.ContainsKey(eventName))
            {
                if (_dicEvents[eventName] != null)
                {
                    _dicEvents[eventName](args);
                }
            }
        }
    }
}