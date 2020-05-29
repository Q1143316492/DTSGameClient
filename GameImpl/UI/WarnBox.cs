using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using CWLEngine.Core.Base;
using CWLEngine.GameImpl.Base;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Util;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Network;


namespace CWLEngine.GameImpl.UI
{
    // 这个只是一个提示框
    public class WarnBox : PanelBase
    {
        Text warnText = null;
        Button btnOK = null;

        void Start()
        {
            warnText = GetControl<Text>("warn_text");

            MvvmMgr.Instance.RegisterCallback(UICacheKeys.MESSAGE_STRING, (obj) =>
            {
                if (obj is string msg && warnText != null)
                {
                    warnText.text = msg;
                }
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.MESSAGE_STRING);

            btnOK = GetControl<Button>("btnOK");
            btnOK.onClick.AddListener(() =>
            {
                UIMgr.Instance.HidePanel(UIPanelPath.WARN_MESSAGE_BOX);
            });
        }
    }
}
