using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Util
{
    public class Tools
    {
        public static Transform FindChildrenTransform(GameObject gameObject, string name)
        {
            if (gameObject == null)
            {
                return null;
            }
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform transform in transforms)
            {
                if (transform.name == name)
                {
                    return transform;
                }
            }

            return null;
        }

        public static T CheckType<T>(object obj, T defaultValue)
        {
            if (obj is T)
            {
                return (T) obj;
            }
            return defaultValue;
        }

    }

    public class JsonTools
    {
        // Json字符串 转 可序列化类
        public static T UnSerializeFromString<T>(string str, string path2 = "")
        {
            try
            {
                T obj = JsonUtility.FromJson<T>(str);

                //if (path2 != string.Empty)
                //{
                //    CreateEncryptionFile(str, path2);
                //}

                return obj;
            }
            catch (ArgumentException ex)
            {
                
                Debug.Log("json UnSerializeFromString fail. err: " + ex.ToString());
                return default;
            }
        }

        public static void CreateEncryptionFile(string str, string path)
        {
            //Debug.Log(path);
        }

        public static string UnEncryptionFile(string str)
        {
            return str;
        }

        // 可序列化类 转 Json字符串
        public static string SerializeToString<T>(T obj)
        {
            try
            {
                return JsonUtility.ToJson(obj);
            }
            catch (ArgumentException ex)
            {
                Debug.Log("json SerializeToString fail. err: " + ex.ToString());
                return default;
            }
        }
    }
}
