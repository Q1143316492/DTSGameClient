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
    public class LoginPanel : PanelBase
    {
        private InputField username = null;
        private InputField password = null;

        private Button btnLogin = null;
        private Button btnChangePassword = null;
        private Button btnExit2 = null;

        private Dropdown serverDrop = null;

        UIType<string> warnBoxString = new UIType<string>(UICacheKeys.MESSAGE_STRING, "");

        public void LoginCallback(Message message)
        {
            UserRouter.LoginResponse response = UserRouter.LoginCallback(message);
            if (response.ret == 0 && response.login_success)
            {
                ScenceMgr.Instance.LoadSceneAsyncUseLoadingBarBegin(ScenePath.LEVEL_00);
                MemeryCacheMgr.Instance.Set(DTSKeys.USER_ID, response.user_id);
                Debug.Log("login success");
            }
            else
            {
                warnBoxString.val = "登入失败";
                UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
                
                Debug.Log("login fail");
            }
        }

        private void TurnBackToMainMenu()
        {
            UIMgr.Instance.HidePanel(UIPanelPath.LOGIN);
            UIMgr.Instance.ShowPanel<MainMenuPanel>(UIPanelPath.MAIN_MENU);
        }

        void Start()
        {
            username = GetControl<InputField>("username");
            password = GetControl<InputField>("password");
            
            MvvmMgr.Instance.AddUIBehaviour("username", username);
            MvvmMgr.Instance.AddUIBehaviour("password", password);

            username.onValueChanged.AddListener(UserTools.CheckUserName);
            password.onValueChanged.AddListener(UserTools.CheckUserPasswd);
            
            btnLogin = GetControl<Button>("btn_login_enter");
            btnChangePassword = GetControl<Button>("btn_change");
            btnExit2 = GetControl<Button>("btn_login_close");

            btnLogin.onClick.AddListener(() => {
                string name = username.text;
                string passwd = password.text;
                if (name.Length > 0 && passwd.Length > 0)
                {
                    UserRouter.Login(name, passwd);
                }
                else
                {
                    warnBoxString.val = "账户密码不能为空";
                    UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
                }
            });

            btnChangePassword.onClick.AddListener(()=> {
                UIMgr.Instance.HidePanel(UIPanelPath.LOGIN);
                UIMgr.Instance.ShowPanel<ChangePswdPanel>(UIPanelPath.CHANGE_PASSWORD);
            });
            btnExit2.onClick.AddListener(TurnBackToMainMenu);
            
            NetworkMgr.Instance.AddMsgListener(ServiceID.USER_LOGIN_SERVICE, LoginCallback);
            
            serverDrop = GetControl<Dropdown>("server_drop");

            List<string> serverList = new List<string>()
            {
                "测试服一 (本地服务)",
                "测试服二 (云服务器)"
            };
            serverDrop.ClearOptions();
            serverDrop.AddOptions(serverList);
            serverDrop.onValueChanged.AddListener((index) =>
            {
                if (index == 0)
                {
                    MemeryCacheMgr.Instance.Set("ip", NetworkMacro.LOCAL_IP);
                    MemeryCacheMgr.Instance.Set("port", NetworkMacro.LOCAL_PORT);
                }
                else if(index == 1)
                {
                    MemeryCacheMgr.Instance.Set("ip", NetworkMacro.NETWORK_IP);
                    MemeryCacheMgr.Instance.Set("port", NetworkMacro.NETWORK_PORT);
                }
            });
        }

        private int tabIndex = 0;
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tabIndex = (tabIndex + 1) % 2;
                if (tabIndex == 0)
                {
                    username.ActivateInputField();
                }
                else if (tabIndex == 1)
                {
                    password.ActivateInputField();
                }
            }
        }
    }
}
