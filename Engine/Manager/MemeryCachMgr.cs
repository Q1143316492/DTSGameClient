using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;

namespace CWLEngine.Core.Manager
{
    /**
     单例的字典
         */
    public class MemeryCacheMgr : Singleton<MemeryCacheMgr>
    {
        private Dictionary<string, object> dict = new Dictionary<string, object>();

        public object Get(string key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return null;
        }

        public void Set(string key, object val)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, val);
            }
            else
            {
                dict[key] = val;
            }
        }

        public void AppendStr(string key, string val, string split = "|")
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, val);
            }
            else if (dict[key] is string)
            {
                dict[key] += split;
                dict[key] += val;
            }
        }

        public void Remove(string key)
        {
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
            }
        }

    }
}
