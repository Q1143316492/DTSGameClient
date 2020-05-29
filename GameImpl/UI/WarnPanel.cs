using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CWLEngine.GameImpl.UI
{
    public class WarnPanel : PanelBase
    {
        private Text warnText = null;


        /**
         由于生命周期的一些问题这里必须在加载多时候手动调用 Init
           
            UIMgr.Instance.ShowPanel<WeaponBoxWarnPanel>("weapon_box_warn", (panel) => {
                ...
                panel.Init();
                warnText.val = "qqq";
                ...
            });

         我需在提前注册 mvvm 的回调，在加载出界面 panel 后生命周期还没有开始调用
             **/
        public void Init()
        {
            warnText = GetControl<Text>("warn_text");

            MvvmMgr.Instance.RegisterCallback(UICacheKeys.BULLET_BOX_WARN_MESSAGE, (msg) =>
            {
                warnText.text = msg as string;
            });
        }

        void OnDisable()
        {
            MvvmMgr.Instance.UnRegisterCallback(UICacheKeys.BULLET_BOX_WARN_MESSAGE);
        }
    }
}
