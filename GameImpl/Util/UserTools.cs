using CWLEngine.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CWLEngine.GameImpl.Util
{
    public class UserTools
    {
        private static readonly int MAX_USERNAME_SIZE = 15;
        private static readonly int MAX_PASSWORD_SIZE = 15;
        
        public static bool Valid(char ch)
        {
            if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z')
                return true;
            if (ch >= '0' && ch <= '9')
                return true;
            return false;
        }

        public static void CheckUserName(string field)
        {
            if (field.Length > 0)
            {
                char last = field[field.Length - 1];
                if (field.Length > MAX_USERNAME_SIZE || !Valid(last))
                {
                    InputField username = MvvmMgr.Instance.GetUIBehaviour("username") as InputField;
                    username.text = field.Substring(0, field.Length - 1);
                }
            }
        }

        public static void CheckUserPasswd(string field)
        {
            if (field.Length > 0)
            {
                char last = field[field.Length - 1];
                if (field.Length > MAX_PASSWORD_SIZE || !Valid(last))
                {
                    InputField password = MvvmMgr.Instance.GetUIBehaviour("password") as InputField;
                    password.text = field.Substring(0, field.Length - 1);
                }
            }
        }

        public static void CheckUserOldPasswd(string field)
        {
            if (field.Length > 0)
            {
                char last = field[field.Length - 1];
                if (field.Length > MAX_PASSWORD_SIZE || !Valid(last))
                {
                    InputField password = MvvmMgr.Instance.GetUIBehaviour("old_password") as InputField;
                    password.text = field.Substring(0, field.Length - 1);
                }
            }
        }

        public static void CheckUserPasswdAgain(string field)
        {
            if (field.Length > 0)
            {
                char last = field[field.Length - 1];
                if (field.Length > MAX_PASSWORD_SIZE || !Valid(last))
                {
                    InputField password = MvvmMgr.Instance.GetUIBehaviour("password_again") as InputField;
                    password.text = field.Substring(0, field.Length - 1);
                }
            }
        }
    }
}
