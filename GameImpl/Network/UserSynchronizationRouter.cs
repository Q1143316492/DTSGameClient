using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Network
{
    public class UserSynchronizationRouter
    {
        /**
         上报玩家坐标
             */
        [Serializable]
        public class ReportTransformRequest
        {
            public int user_id;
            public string position;
            public string rotation;
            public int time;
        }

        [Serializable]
        public class ReportTransformResponse
        {
            public int ret;
            public string err_msg;
            public int time;
        }

        public static void ReportTransform(int user_id, Transform transform)
        {
            int msTime = (int)(Time.time * 1000.0f);

            ReportTransformRequest request = new ReportTransformRequest
            {
                user_id = user_id,
                position = string.Format("{0:F};{1:F};{2:F}",
                    transform.position.x, transform.position.y, transform.position.z),
                rotation = string.Format("{0:F};{1:F};{2:F}",
                    transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z),
                time = msTime,
            };
            Message message = new Message();
            message.PackBuffer(ServiceID.SYNCHRONIZATION_REPORT_TRANSFORM_SERVICE, JsonTools.SerializeToString(request));

            NetworkMgr.Instance.Send(message);
        }

        public static void ReportTransformCallback(Message message)
        {
            try
            {
                ReportTransformResponse response = null;
                response = JsonTools.UnSerializeFromString<ReportTransformResponse>(message.GetMessageBuffer());

                int msTime = (int)(Time.time * 1000.0f);
                //Debug.Log("last time:" + (msTime - response.time) + "ms");
            }
            catch (Exception ex)
            {
                Debug.Log("ReportTransformCallback parse error. " + ex.ToString());
            }
        }

        /**
         查询玩家坐标
             */
        [Serializable]
        public class QueryUserTransformRequest
        {
            public int user_id;
            public int time;
        }

        [Serializable]
        public class QueryUserTransformResponse
        {
            public int ret;
            public int user_id;
            public string position;
            public string rotation;
            public string err_msg;
            public int time;
        }

        public static void QueryUsersTransform(int user_id)
        {
            int msTime = (int) (Time.time * 1000.0f);
            QueryUserTransformRequest request = new QueryUserTransformRequest
            {
                user_id = user_id,
                time = msTime
            };
            Message message = new Message();
            message.PackBuffer(ServiceID.SYNCHRONIZATION_QUERY_USER_TRANSFORM_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }
       
        public static QueryUserTransformResponse QueryUsersTransformCallback(Message message)
        {
            try
            {
                QueryUserTransformResponse response = null;
                response = JsonTools.UnSerializeFromString<QueryUserTransformResponse>(message.GetMessageBuffer());
                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("QueryUserTransformResponse parse error. " + ex.ToString());
                return null;
            }
        }

        /**
         心跳包
             */
        [Serializable]
        public class HeartBeatRequest
        {
            public int user_id;
            public int mode;
            public float time;

        }

        [Serializable]
        public class HeartBeatResponse
        {
            public string err_msg;
            public int ret;

        }

        public static void HeartBeatRequestCall(int user_id, int mode, float time)
        {
            HeartBeatRequest request = new HeartBeatRequest
            {
                user_id = user_id,
                mode = mode,
                time = time,

            };
            Message message = new Message();
            message.PackBuffer(ServiceID.SYNCHRONIZATION_HEART_BEAT_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static void HeartBeatRequestCallback(Message message)
        {
            try
            {
                HeartBeatResponse response = null;
                response = JsonTools.UnSerializeFromString<HeartBeatResponse>(message.GetMessageBuffer());
                
                if (response.ret == -1)
                {
                    UIType<string> warnText = new UIType<string>(UICacheKeys.MESSAGE_STRING, "网络连接中断");
                    UIMgr.Instance.ShowPanel<WarnBox>(UIPanelPath.WARN_MESSAGE_BOX);
                    HeartBeat.Instance.Stop();
                    NetworkMgr.Instance.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Log("HeartBeatRequestCallback parse error. " + ex.ToString());
            }
        }

        /**
         同步玩家行为 上报
         */
        [Serializable]
        public class ReportActionRequest
        {
            public int user_id;
            public string action;
            public int frame;
        }

        [Serializable]
        public class ReportActionResponse
        {
            public string err_msg;
            public int ret;
            public int frame;
        }

        public static void ReportActionRequestCall(int user_id, string action, int frame)
        {
            ReportActionRequest request = new ReportActionRequest
            {
                user_id = user_id,
                action = action,
                frame = frame
            };

            Message message = new Message();
            message.PackBuffer(ServiceID.SYNCHRONIZATION_REPORT_ACTION_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static ReportActionResponse ReportActionRequestCallback(Message message)
        {
            try
            {
                ReportActionResponse response = null;
                response = JsonTools.UnSerializeFromString<ReportActionResponse>(message.GetMessageBuffer());
                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("ReportActionRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }
        
        /**
         同步玩家行为 查询
         */

        [Serializable]
        public class QueryActionRequest
        {
            public int frame;
            public int user_id;
        }

        [Serializable]
        public class QueryActionResponse
        {
            public string action;
            public string err_msg;
            public int ret;
            public int frame;
        }

        public static void QueryActionRequestCall(int user_id, int frame)
        {
            QueryActionRequest request = new QueryActionRequest
            {
                user_id = user_id,
                frame = frame
            };

            Message message = new Message();
            message.PackBuffer(ServiceID.SYNCHRONIZATION_QUERY_ACTION_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static QueryActionResponse QueryActionRequestCallback(Message message)
        {
            try
            {
                QueryActionResponse response = null;

                response = JsonTools.UnSerializeFromString<QueryActionResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("QueryActionRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }
        
    }
}
