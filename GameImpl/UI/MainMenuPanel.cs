using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CWLEngine.GameImpl.UI
{
    public class MainMenuPanel : PanelBase
    {
        private Button btnLogin = null;
        private Button btnRegister = null;
        private Button btnExit = null;
        private Button btnOpenSetting = null;

        void Start()
        {
            btnLogin = GetControl<Button>("btn_show_login");
            btnRegister = GetControl<Button>("btn_show_register");
            btnOpenSetting = GetControl<Button>("btn_open_setting");
            btnExit = GetControl<Button>("btn_show_exit");

            btnLogin.onClick.AddListener(() =>
            {
                UIMgr.Instance.HidePanel(UIPanelPath.MAIN_MENU);
                UIMgr.Instance.ShowPanel<LoginPanel>(UIPanelPath.LOGIN);
            });

            btnRegister.onClick.AddListener(() =>
            {
                UIMgr.Instance.HidePanel(UIPanelPath.MAIN_MENU);
                UIMgr.Instance.ShowPanel<RegisterPanel>(UIPanelPath.REGISTER);
            });

            btnExit.onClick.AddListener(() => {
                Debug.Log("exit click");
                Application.Quit();
            });

            btnOpenSetting.onClick.AddListener(() =>
            {
                //UIMgr.Instance.HidePanel(UIPanelPath.MAIN_MENU);
                UIMgr.Instance.ShowPanel<SettingPanel>(UIPanelPath.SETTING);
            });
        }
    }
}
