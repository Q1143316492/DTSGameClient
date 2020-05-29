using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;

namespace CWLEngine.Core.Manager
{
    /**
     事件发布订阅模型那种用法。
         */
    public class EventMgr : Singleton<EventMgr>
    {
        private Dictionary<string, UnityAction<object>> events = new Dictionary<string, UnityAction<object>>();

        public void AddEventListener(string name, UnityAction<object> action)
        {
            if (events.ContainsKey(name))
            {
                events[name] += action;
            }
            else
            {
                events.Add(name, action);
            }
        }

        public void DelEventListener(string name, UnityAction<object> action)
        {
            if (events.ContainsKey(name))
            {
                events[name] -= action;
            }
        }

        public void EventTrigger(string name, object info)
        {
            if (events.ContainsKey(name))
            {
                events[name](info);
            }
        }

        public void Clear()
        {
            events.Clear();
        }
    }
}
