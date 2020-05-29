using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using CWLEngine.Core.Base;
using UnityEngine.Events;

namespace CWLEngine.Core.Manager
{
    /**
     * 
   
     UIType<int> u = new UIType<int>(key, default_val)
     UIType 在 MvvmItem.cs
        
     u.val 对 UIType 的修改，影响下面单例 MvvmMgr 的字典里面 key 的值
     然后UI上注册绑定key的回调。

     对 UIType 的修改会直接影响到 UI 上。
         */
    public class MvvmMgr : Singleton<MvvmMgr>
    {
        private Dictionary<string, UIBehaviour> UIDict = new Dictionary<string, UIBehaviour>();
        
        public void AddUIBehaviour(string key, UIBehaviour value)
        {
            if (!UIDict.ContainsKey(key))
            {
                UIDict[key] = value;
            }
        }

        public void DelUIBehaviour(string key)
        {
            if (UIDict.ContainsKey(key))
            {
                UIDict.Remove(key);
            }
        }

        public UIBehaviour GetUIBehaviour(string name)
        {
            if (UIDict.ContainsKey(name))
            {
                return UIDict[name];
            }
            return null;
        }

        public void SetText(string textItemName, string textValue)
        {
            if (UIDict.ContainsKey(textItemName))
            {
                if (UIDict[textItemName] is Text)
                {
                    (UIDict[textItemName] as Text).text = textValue;
                    return;
                }
                if (UIDict[textItemName] is InputField)
                {
                    (UIDict[textItemName] as InputField).text = textValue;
                    return;
                }
            }
        }

        public void Clear()
        {
            UIDict.Clear();
        }

        // 另一个思路 ========================================================
        
        private Dictionary<string, object> valueDict = new Dictionary<string, object>();
        private Dictionary<string, UnityAction<object>> callbackDict = new Dictionary<string, UnityAction<object>>();

        public object Get(string key)
        {
            if (valueDict.ContainsKey(key))
            {
                return valueDict[key];
            }
            return null;
        }

        public T Get<T>(string key)
        {
            if (valueDict.ContainsKey(key))
            {
                return (T) valueDict[key];
            }
            return default(T);
        }

        public void RegisterCallback(string name, UnityAction<object>callback)
        {
            if (callbackDict.ContainsKey(name))
            {
                callbackDict[name] += callback;
            }
            else
            {
                callbackDict.Add(name, callback);
            }
        }

        public void UnRegisterCallback(string name)
        {
            if (callbackDict.ContainsKey(name))
            {
                callbackDict.Remove(name);
            }
        }

        public void Set(string key, object val)
        {
            if (!valueDict.ContainsKey(key))
            {
                valueDict.Add(key, val);
            }
            else
            {
                valueDict[key] = val;
            }

            if (callbackDict.ContainsKey(key))
            {
                UnityAction<object> callback = callbackDict[key];
                callback?.Invoke(val);
            }
        }

        public void Fresh(string key)
        {
            Set(key, Get(key));
        }
    }
}
