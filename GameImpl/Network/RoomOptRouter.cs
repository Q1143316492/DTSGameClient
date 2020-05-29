using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Network
{
    public class RoomOptRouter
    {
        public class EnterRoomRequest
        {
            public int user_id;
            public int room_type;
            public int room_id;
        }

        public class EnterRoomResponse
        {
            public int ret;
            public int room_id;
            public string err_msg;
        }

        public static void UserEnterRoom(int user_id, int room_type, int room_id)
        {
            EnterRoomRequest request = new EnterRoomRequest
            {
                user_id = user_id,
                room_type = room_type,
                room_id = room_id,
            };

            Message message = new Message();
            message.PackBuffer(ServiceID.ROOM_ENTER_ROOM_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);

        }

        public static EnterRoomResponse UserEnterRoomCallback(Message message)
        {
            try
            {
                return JsonTools.UnSerializeFromString<EnterRoomResponse>(message.GetMessageBuffer());
            }
            catch (Exception ex)
            {
                Debug.Log("UserEnterRoomCallback err." + ex.ToString());
                return null;
            }
        }

        public class QueryRoomUsersRequest
        {
            public int room_id;
        }

        public class QueryRoomUsersResponse
        {
            public int ret;
            public string user_id_list;
            public string err_msg;
        }

        public static void QueryRoomUsers(int room_id)
        {
            QueryRoomUsersRequest request = new QueryRoomUsersRequest
            {
                room_id = room_id
            };
            Message message = new Message();
            message.PackBuffer(ServiceID.ROOM_QUERY_ROOM_USERS_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static QueryRoomUsersResponse QueryRoomUsersCallback(Message message)
        {
            try
            {
                return JsonTools.UnSerializeFromString<QueryRoomUsersResponse>(message.GetMessageBuffer());
            }
            catch (Exception ex)
            {
                Debug.Log("QueryRoomUsersCallback error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class QueryUserBelongRoomRequest
        {
            public int user_id;
        }

        [Serializable]
        public class QueryUserBelongRoomResponse
        {
            public int ret;
            public int sub_server_id;
            public int room_type;
            public int room_id;
            public string err_msg;
        }

        public static void QueryUserBelongRoom(int user_id)
        {
            QueryUserBelongRoomRequest request = new QueryUserBelongRoomRequest
            {
                user_id = user_id
            };
            Message message = new Message();
            message.PackBuffer(ServiceID.ROOM_QUERY_USER_BELONGED_ROOM_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static QueryUserBelongRoomResponse QueryUserBelongRoomCallback(Message message)
        {
            try
            {
                return JsonTools.UnSerializeFromString<QueryUserBelongRoomResponse>(message.GetMessageBuffer());
            }
            catch (Exception ex)
            {
                Debug.Log("QueryUserBelongRoomCallback error. " + ex.ToString());
                return null;
            }
        }
    }
}
