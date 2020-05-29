using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;

namespace CWLEngine.Core.Manager
{
    public class ResourceMgr : Singleton<ResourceMgr>
    {
        public T Load<T>(string name) where T : UnityEngine.Object
        {
            T resource = Resources.Load<T>(name);
            if (resource is GameObject)
            {
                return GameObject.Instantiate(resource);
            }
            return resource;
        }

        public void LoadAsync<T>(string name, UnityAction<T> callback) where T : UnityEngine.Object
        {
            MonoMgr.Instance.StartCoroutine(LoadAsyncCoroutine(name, callback));
        }

        private IEnumerator LoadAsyncCoroutine<T>(string name, UnityAction<T> callback) where T : UnityEngine.Object
        {
            ResourceRequest r = Resources.LoadAsync<T>(name);
            yield return r;

            if (r.asset is GameObject)
            {
                callback(UnityEngine.Object.Instantiate(r.asset) as T);
            }
            else
            {
                callback(r.asset as T);
            }
        }

    }
}
