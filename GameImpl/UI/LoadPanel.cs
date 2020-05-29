using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;

namespace CWLEngine.GameImpl.UI
{
    public class LoadPanel : PanelBase
    {
        void Start()
        {
            ObjectPoolMgr.Instance.Clear();
            MvvmMgr.Instance.Clear();

            string nextScenc = MemeryCacheMgr.Instance.Get(EngineMacro.NEXT_SCENE) as string;

            if (nextScenc != string.Empty)
            {
                MemeryCacheMgr.Instance.Remove(EngineMacro.NEXT_SCENE);
                ScenceMgr.Instance.LoadSceneAsyncUseLoadingBarEnd(nextScenc);
            }

            // todo 加载失败处理
        }
    }
}
