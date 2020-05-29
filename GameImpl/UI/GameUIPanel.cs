using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;

namespace CWLEngine.GameImpl.UI
{
    public class GameUIPanel : PanelBase
    {
        Text bulletCount = null;
        
        void Start()
        {
            bulletCount = GetControl<Text>("bullet_count");
        }

        void Update()
        {
            //// UI 显示子弹
            //if (bulletCount)
            //{
            //    object obj1 = MemeryCacheMgr.Instance.Get(PlayerKVS.BULLET_COUNT_FIRST);
            //    object obj2 = MemeryCacheMgr.Instance.Get(PlayerKVS.BULLET_COUNT_SECOND);
            //    int bulletCountFirst = 0;
            //    int bulletCountSecond = 0;
            //    if (obj1 is int) bulletCountFirst = (int)obj1;
            //    if (obj2 is int) bulletCountSecond = (int)obj2;
            //    bulletCount.text = bulletCountFirst.ToString() + " / " + bulletCountSecond.ToString();
            //}

        }
    }
}
