using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using CWLEngine.Core.Base;
using CWLEngine.GameImpl.Controller.Weapon;

namespace CWLEngine.Core.Manager
{
    /*
     弃用
         */
    public class FactoryMgr : Singleton<FactoryMgr>
    {
        private Dictionary<string, Func<object>> functions = new Dictionary<string, Func<object>>();

        public void Add(string name, Func<object>func)
        {
            if (!functions.ContainsKey(name))
            {
                functions.Add(name, func);
            }
            else
            {
                functions[name] = func;
            }
        }

        public void Remove(string name)
        {
            if (!functions.ContainsKey(name))
            {
                functions.Remove(name);
            }
        }

        public Func<object> GetCreator(string name)
        {
            if (functions.ContainsKey(name))
            {
                return functions[name];
            }
            return null;
        }

        public void Clear()
        {
            functions.Clear();
        }
    }
}
