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
    public class ChangePswdPanel : PanelBase
    {
        private InputField username = null;
        private InputField oldPassword = null;
        private InputField password = null;
        private InputField passwordAgain = null;

        private Button btnChange = null;
        private Button btnExit = null;
        private Button btnExit2 = null;

        UIType<string> warnBoxString = new UIType<string>(UICacheKeys.MESSAGE_STRING, "");

        private void TurnBackToMainMenu()
        {
            UIMgr.Instance.HidePanel(UIPanelPath.CHANGE_PASSWORD);
            UIMgr.Instance.ShowPanel<MainMenuPanel>(UIPanelPath.MAIN_MENU);
        }

        void ChangePasswordCallback(Message message)
        {
            UserRouter.ChangePasswordResponse res = UserRouter.ChangePasswordRequestCallback(message);
            if (res.ret == 0 && res.success)
            {
                warnBoxString.val = "修改成功";
                username.text = "";
                oldPassword.text = "";
                password.text = "";
                passwordAgain.text = "";
                UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
            }
            else
            {
                warnBoxString.val = "修改失败";
                UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
            }
        }

        void Start()
        {
            NetworkMgr.Instance.CheckNetwork();
            NetworkMgr.Instance.AddMsgListener(ServiceID.USER_CHANGE_PASSWORD_SERVICE, ChangePasswordCallback);

            username = GetControl<InputField>("username");
            oldPassword = GetControl<InputField>("old_password");
            password = GetControl<InputField>("password");
            passwordAgain = GetControl<InputField>("password_again");

            MvvmMgr.Instance.AddUIBehaviour("username", username);
            MvvmMgr.Instance.AddUIBehaviour("old_password", oldPassword);
            MvvmMgr.Instance.AddUIBehaviour("password", password);
            MvvmMgr.Instance.AddUIBehaviour("password_again", passwordAgain);

            username.onValueChanged.AddListener(UserTools.CheckUserName);
            oldPassword.onValueChanged.AddListener(UserTools.CheckUserOldPasswd);
            password.onValueChanged.AddListener(UserTools.CheckUserPasswd);
            passwordAgain.onValueChanged.AddListener(UserTools.CheckUserPasswdAgain);

            btnChange = GetControl<Button>("btn_register");
            btnExit = GetControl<Button>("btn_register_close");
            btnExit2 = GetControl<Button>("btn_register_back");

            btnChange.onClick.AddListener(() =>
            {
                if (username.text.Length > 0 && 
                    password.text.Length > 0 && 
                    oldPassword.text.Length > 0 &&
                    passwordAgain.text.Length > 0 && 
                    password.text == passwordAgain.text)
                {
                    UserRouter.ChangePasswordRequestCall(username.text, password.text, oldPassword.text);
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
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.USER_CHANGE_PASSWORD_SERVICE, ChangePasswordCallback);
        }

        private int tabIndex = 0;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tabIndex = (tabIndex + 1) % 4;
                if (tabIndex == 0)
                {
                    username.ActivateInputField();
                }
                else if (tabIndex == 1)
                {
                    oldPassword.ActivateInputField();
                }
                else if (tabIndex == 2)
                {
                    password.ActivateInputField();
                }
                else if (tabIndex == 3)
                {
                    passwordAgain.ActivateInputField();
                }
            }
        }
    }
}
