using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Util;
using CWLEngine.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Network
{
    public class UserRouter
    {
        [Serializable]
        public class LoginRequest
        {
            public string username;
            public string password;
            public int time;

            public LoginRequest() { }

            public LoginRequest(string username, string password)
            {
                this.username = username;
                this.password = password;
            }
        }

        [Serializable]
        public class LoginResponse
        {
            public int ret;
            public bool login_success;
            public int user_id;
            public int time;
        }

        public static void Login(string name, string passwd)
        {
            int msTime = (int)(Time.time * 1000);
            LoginRequest request = new LoginRequest
            {
                username = name,
                password = passwd,
                time = msTime
            };

            Message message = new Message();
            message.PackBuffer(ServiceID.USER_LOGIN_SERVICE, JsonTools.SerializeToString(request));

            string ip = string.Empty;
            int port = -1;
            try
            {
                ip = MemeryCacheMgr.Instance.Get("ip") as string;
                port = (int) MemeryCacheMgr.Instance.Get("port");
            }
            catch(Exception ex)
            {
                Debug.Log("ip or port not set. " + ex.ToString());
                ip = NetworkMacro.LOCAL_IP;
                port = NetworkMacro.LOCAL_PORT;
            }
            
            NetworkMgr.Instance.Connect(ip, port, () =>
            {
                NetworkMgr.Instance.Send(message);
            });
        }

        public static LoginResponse LoginCallback(Message message)
        {
            try
            {
                LoginResponse result = JsonTools.UnSerializeFromString<LoginResponse>(message.GetMessageBuffer());

                int msTime = (int)(Time.time * 1000);

                return result;
            }
            catch(Exception ex)
            {
                Debug.Log("LoginCallback result error. " + ex.ToString());
                return null;
            }
        }


        [Serializable]
        public class RegisterRequest
        {
            public string username;
            public string password;

        }

        [Serializable]
        public class RegisterResponse
        {
            public string err_msg;
            public int ret;
            public bool register_success;

        }

        public static void RegisterRequestCall(string username, string password)
        {
            RegisterRequest request = new RegisterRequest
            {
                username = username,
                password = password,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.USER_REGISTER_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static RegisterResponse RegisterRequestCallback(Message message)
        {
            try
            {
                RegisterResponse response = null;

                response = JsonTools.UnSerializeFromString<RegisterResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("RegisterRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class ChangePasswordRequest
        {
            public string username;
            public string password;
            public string old_password;

        }

        [Serializable]
        public class ChangePasswordResponse
        {
            public string err_msg;
            public bool success;
            public int ret;

        }

        public static void ChangePasswordRequestCall(string username, string password, string old_password)
        {
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                username = username,
                password = password,
                old_password = old_password,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.USER_CHANGE_PASSWORD_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static ChangePasswordResponse ChangePasswordRequestCallback(Message message)
        {
            try
            {
                ChangePasswordResponse response = null;

                response = JsonTools.UnSerializeFromString<ChangePasswordResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("ChangePasswordRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class NetworkTestRequest
        {
            public string msg;
            public int last_time;

        }

        [Serializable]
        public class NetworkTestResponse
        {
            public string extend;
            public string err_msg;
            public int last_time;
            public int ret;

        }

        public static void NetworkTestRequestCall(string msg, int last_time)
        {
            NetworkTestRequest request = new NetworkTestRequest
            {
                msg = msg,
                last_time = last_time,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.USER_NETWORK_TEST_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static NetworkTestResponse NetworkTestRequestCallback(Message message)
        {
            try
            {
                NetworkTestResponse response = null;

                response = JsonTools.UnSerializeFromString<NetworkTestResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("NetworkTestRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class UserLevelRequest
        {
            public int opt;
            public int user_id;
            public int val;

        }

        [Serializable]
        public class UserLevelResponse
        {
            public string err_msg;
            public int val;
            public int opt;
            public int ret;

        }

        public static void UserLevelRequestCall(int opt, int user_id, int val)
        {
            UserLevelRequest request = new UserLevelRequest
            {
                opt = opt,
                user_id = user_id,
                val = val,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.USER_USER_LEVEL_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static UserLevelResponse UserLevelRequestCallback(Message message)
        {
            try
            {
                UserLevelResponse response = null;

                response = JsonTools.UnSerializeFromString<UserLevelResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("UserLevelRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }
    }
}
