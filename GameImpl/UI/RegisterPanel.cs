using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Network;
using CWLEngine.GameImpl.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CWLEngine.GameImpl.UI
{
    public class RegisterPanel : PanelBase
    {
        private InputField username = null;
        private InputField password = null;
        private InputField passwordAgain = null;

        private Button btnRegister = null;
        private Button btnExit = null;
        private Button btnExit2 = null;

        UIType<string> warnBoxString = new UIType<string>(UICacheKeys.MESSAGE_STRING, "");

        private void TurnBackToMainMenu()
        {
            UIMgr.Instance.HidePanel(UIPanelPath.REGISTER);
            UIMgr.Instance.ShowPanel<MainMenuPanel>(UIPanelPath.MAIN_MENU);
        }

        void RegisterCallback(Message message)
        {
            UserRouter.RegisterResponse res = UserRouter.RegisterRequestCallback(message);
            if (res.ret == 0 && res.register_success)
            {
                warnBoxString.val = "注册成功";
                UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
            }
            else
            {
                warnBoxString.val = "注册失败";
                UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
            }
        }

        void Start()
        {
            NetworkMgr.Instance.CheckNetwork();
            NetworkMgr.Instance.AddMsgListener(ServiceID.USER_REGISTER_SERVICE, RegisterCallback);

            username = GetControl<InputField>("username");
            password = GetControl<InputField>("password");
            passwordAgain = GetControl<InputField>("password_again");

            MvvmMgr.Instance.AddUIBehaviour("username", username);
            MvvmMgr.Instance.AddUIBehaviour("password", password);
            MvvmMgr.Instance.AddUIBehaviour("password_again", passwordAgain);

            username.onValueChanged.AddListener(UserTools.CheckUserName);
            password.onValueChanged.AddListener(UserTools.CheckUserPasswd);
            passwordAgain.onValueChanged.AddListener(UserTools.CheckUserPasswdAgain);

            btnRegister = GetControl<Button>("btn_register");
            btnExit = GetControl<Button>("btn_register_close");
            btnExit2 = GetControl<Button>("btn_register_back");

            btnRegister.onClick.AddListener(() =>
            {
                if (username.text.Length > 0 && password.text.Length > 0 && passwordAgain.text.Length > 0 && password.text == passwordAgain.text)
                {
                    UserRouter.RegisterRequestCall(username.text, password.text);
                }
                else
                {
                    warnBoxString.val = "用户名密码不符合要求";
                    UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
                }
            });

            btnExit.onClick.AddListener(TurnBackToMainMenu);
            btnExit2.onClick.AddListener(TurnBackToMainMenu);
        }

        void OnDestroy()
        {
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.USER_REGISTER_SERVICE, RegisterCallback);
        }

        private int tabIndex = 0;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tabIndex = (tabIndex + 1) % 3;
                if (tabIndex == 0)
                {
                    username.ActivateInputField();
                }
                else if (tabIndex == 1)
                {
                    password.ActivateInputField();
                }
                else if (tabIndex == 2)
                {
                    passwordAgain.ActivateInputField();
                }
            }
        }
    }
}
