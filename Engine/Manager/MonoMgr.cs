using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;

namespace CWLEngine.Core.Manager
{
    /**
        hookobj 挂在一个继承了 MonoBehaviour 的类上
        
        用这个类可以通过 hookobj 为中介 调用 update, StartCoroutine 之类的东西
         */
    public class MonoMgr : Singleton<MonoMgr>
    {
        public EventHook hookobj;

        public MonoMgr()
        {
            hookobj = GameObject.Find(EngineMacro.ENGINE_HOOK).GetComponent<EventHook>();
        }

        public void AddUpdateEvent(UnityAction fun)
        {
            hookobj.AddUpdateEvent(fun);
        }

        public void DelUpdateEvent(UnityAction fun)
        {
            hookobj.DelUpdateEvent(fun);
        }

        public void AddFixedUpdateEvent(UnityAction fun)
        {
            hookobj.AddFixedUpdateEvent(fun);
        }

        public void DelFixedUpdateEvent(UnityAction fun)
        {
            hookobj.DelFixedUpdateEvent(fun);
        }

        public Coroutine StartCoroutine(string methodName)
        {
            return hookobj.StartCoroutine(methodName);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return hookobj.StartCoroutine(routine);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            hookobj.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            hookobj.StopCoroutine(routine);
        }

        private IEnumerator DelayEvent(float delayTimes, UnityAction callback)
        {
            float beginTime = Time.time * 1000;
            while (beginTime + delayTimes > Time.time * 1000)
            {
                yield return null;
            }
            callback();
        }

        private IEnumerator DelayEventMultiTimes(float delayTimes, int times, UnityAction callback)
        {
            float beginTime = Time.time * 1000;
            while (true)
            {
                while (beginTime + delayTimes > Time.time * 1000)
                {
                    yield return null;
                }
                callback();
                times--;
                beginTime = Time.time * 1000;
                if (times == 0)
                {
                    break;
                }
            }
        }

        // 延迟一定时间后执行callback , delayTimes 单位毫秒
        public void StartDelayEvent(float delayTimes, UnityAction callback)
        {
            hookobj.StartCoroutine(DelayEvent(delayTimes, callback));
        }

        public void StartDelayEventMultiTimes(float delayTimes, int times, UnityAction callback)
        {
            hookobj.StartCoroutine(DelayEventMultiTimes(delayTimes, times, callback));
        }
    }
}
