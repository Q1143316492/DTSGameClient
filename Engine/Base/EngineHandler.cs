using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Conf;
using CWLEngine.GameImpl.Util;
using CWLEngine.GameImpl.Controller.Weapon;

namespace CWLEngine.Core.Base
{
    /*
        脚本挂到空物体上，作为整个游戏的初始化。每个场景有且只需要挂这一个。有几个作用。
        1，本脚本创建空物体，且该物体过场景不消失。同时也可所谓单例下的 对象池 的父物体(类似收纳盒）。
        2，我需要一个 MonoBehaviour 的子类，获取他就可以让自己写的对象受引擎的 Update FixedUpdate等生命周期函数影响，还能
            开协程。但是过场景该物体消失。在某些情况下可能会造成一些问题。
        
         */
    public class EngineHandler : MonoBehaviour
    {
        void Awake()
        {
            // [0] 创建一个过场景不消失的控制全局的物体，如果上一个场景继承下来了，就找到他，否则创建。
            GameObject engine = null;
            if (MemeryCacheMgr.Instance.Get(EngineMacro.ENGINE_CORE) == null)
            {
                engine = new GameObject(EngineMacro.ENGINE_CORE);
                DontDestroyOnLoad(engine);
                MemeryCacheMgr.Instance.Set(EngineMacro.ENGINE_CORE, true);
            }
            else
            {
                engine = GameObject.Find(EngineMacro.ENGINE_CORE);
            }

            // [1] 
            if (MemeryCacheMgr.Instance.Get(EngineMacro.ENGINE_HOOK) == null)
            {
                GameObject obj = new GameObject(EngineMacro.ENGINE_HOOK);
                obj.AddComponent<EventHook>();
                obj.transform.parent = engine.transform;
                MemeryCacheMgr.Instance.Set(EngineMacro.ENGINE_HOOK, true);
            }
            
            // [2] 挂载音效的空物体
            if (MemeryCacheMgr.Instance.Get(EngineMacro.ENGINE_MUSIC) == null)
            {
                GameObject obj = new GameObject(EngineMacro.ENGINE_MUSIC);
                obj.AddComponent<MusicBase>();
                obj.transform.parent = engine.transform;
                MemeryCacheMgr.Instance.Set(EngineMacro.ENGINE_HOOK, true);
            }

            // [3] 预加载配置
            GameConfigLoad.Instance.ToString();

        }

        void Update()
        {

        }

        void Start()
        {

        }
    }
}

