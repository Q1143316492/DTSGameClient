using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using UnityEngine;
using UnityEditor;
using System.IO;
using CWLEngine.GameImpl.Conf;
using CWLEngine.GameImpl.Util;

namespace CWLEngine.Core.Manager
{
    public class ConfMgr : Singleton<ConfMgr>
    {
        public string ConfPath = Application.dataPath + "/config/";
        public string ConfPath2 = Application.dataPath + "/_config/";
        public Dictionary<string, object> confDict = new Dictionary<string, object>();
        
        public ConfMgr()
        {

        }

        public T GetConf<T>(string name)
        {
            if (confDict.ContainsKey(name))
            {
                return (T) confDict[name];
            }
            return default;
        }

        public bool Load<T>(string name, string path)
        {
            try
            {
                path = ConfPath + path;
                string path2 = ConfPath2 + path;
                StringBuilder stringBuilder = new StringBuilder();
                using (StreamReader sr = File.OpenText(path))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        stringBuilder.Append(s);
                    }
                }
                T conf = JsonTools.UnSerializeFromString<T>(stringBuilder.ToString(), path2);
                confDict.Add(name, conf);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("load conf fail." + ex.ToString());
                return false;
            }
        }

    }
}
