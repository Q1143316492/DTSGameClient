using System;
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
    /**
     缓存池。
        不要的东西 set active = false 再收起来，放一个空物体下面
         */
    public class PoolObj
    {
        public GameObject fatherObj;
        public List<GameObject> list;

        public PoolObj(GameObject obj, GameObject pool)
        {
            fatherObj = new GameObject(obj.name);
            fatherObj.transform.parent = pool.transform;
            list = new List<GameObject>();
            RecycleGameObject(obj);
        }

        // 回收
        public void RecycleGameObject(GameObject obj)
        {
            list.Add(obj);
            obj.transform.parent = fatherObj.transform;
            obj.SetActive(false);
        }
        
        public GameObject GetGameObject()
        {
            if (list.Count > 0)
            {
                GameObject obj = list[0];
                list.RemoveAt(0);
                obj.SetActive(true);
                obj.transform.parent = null;
                return obj;
            }
            return null;
        }
    }

    public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
    {
        public Dictionary<string, PoolObj> poolDict = new Dictionary<string, PoolObj>();
        private static GameObject pool;

        public ObjectPoolMgr()
        {
            GameObject engine = GameObject.Find(EngineMacro.ENGINE_CORE);
            pool = new GameObject(EngineMacro.ENGINE_POOL);
            pool.transform.parent = engine.transform;
        }

        // 注意 name 是游戏物体在 Assets/Resource/ 目录 下的路径. 最后会 Resource.Load(name)
        public void LoadGameObject(string name, UnityAction<GameObject> callback)
        {
            GameObject obj = null;
            if (poolDict.ContainsKey(name) && poolDict[name].list.Count > 0)
            {
                obj = poolDict[name].GetGameObject();
                callback(obj);
            }
            else
            {
                ResourceMgr.Instance.LoadAsync<GameObject>(name, (o) =>
                {
                    o.name = name;
                    callback(o);
                });
            }
        }

        // 同步加载 Resource
        public GameObject LoadGameObject(string name)
        {
            GameObject obj = null;
            if (poolDict.ContainsKey(name) && poolDict[name].list.Count > 0)
            {
                Debug.Log("load from pool:" + name);
                obj = poolDict[name].GetGameObject();
                return obj;
            }
            else
            {
                Debug.Log("load new obj:" + name);
                obj = ResourceMgr.Instance.Load<GameObject>(name);
                obj.name = name;
            }
            return obj;
        }

        // 回收游戏物体到缓存池， name 是路径，也是目录下面的分类
        public void RemoveGameObject(GameObject obj, string name = null)
        {
            if (name == null)
            {
                name = obj.name;
            }
            if (poolDict.ContainsKey(name))
            {
                poolDict[name].RecycleGameObject(obj);
            }
            else
            {
                poolDict.Add(name, new PoolObj(obj, pool));
            }
        }

        public void Clear()
        {
            poolDict.Clear();
        }
    }
}
