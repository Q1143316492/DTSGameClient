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
    public class GameMgrRouter
    {

        [Serializable]
        public class PlayAloneRequest
        {
            public int user_id;
        }

        [Serializable]
        public class PlayAloneResponse
        {
            public string err_msg;
            public int ret;
            public int room_id;

        }

        public static void PlayAloneRequestCall(int user_id)
        {
            PlayAloneRequest request = new PlayAloneRequest
            {
                user_id = user_id,
            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_PLAY_ALONE_SERVICE, JsonTools.SerializeToString(request));

            NetworkMgr.Instance.Send(message);
        }

        public static PlayAloneResponse PlayAloneRequestCallback(Message message)
        {
            try
            {
                PlayAloneResponse response = null;

                response = JsonTools.UnSerializeFromString<PlayAloneResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("PlayAloneRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class PlayWithOthersRequest
        {
            public int user_id;
            public int mode;
            public float matching_time;

        }

        [Serializable]
        public class PlayWithOthersResponse
        {
            public string err_msg;
            public int ret;
            public int mode;

        }

        public static void PlayWithOthersRequestCall(int user_id, int mode, float matching_time)
        {
            PlayWithOthersRequest request = new PlayWithOthersRequest
            {
                user_id = user_id,
                mode = mode,
                matching_time = matching_time,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_PLAY_WITH_OTHERS_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static PlayWithOthersResponse PlayWithOthersRequestCallback(Message message)
        {
            try
            {
                PlayWithOthersResponse response = null;

                response = JsonTools.UnSerializeFromString<PlayWithOthersResponse>(message.GetMessageBuffer());
                
                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("PlayWithOthersRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class QueryMatchingResultRequest
        {
            public int player_count;
            public int user_id;

        }

        [Serializable]
        public class QueryMatchingResultResponse
        {
            public int room_id;
            public string err_msg;
            public int ret;

        }

        public static void QueryMatchingResultRequestCall(int player_count, int user_id)
        {
            QueryMatchingResultRequest request = new QueryMatchingResultRequest
            {
                player_count = player_count,
                user_id = user_id,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_QUERY_MATCHING_RESULT_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static QueryMatchingResultResponse QueryMatchingResultRequestCallback(Message message)
        {
            try
            {
                QueryMatchingResultResponse response = null;

                response = JsonTools.UnSerializeFromString<QueryMatchingResultResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("QueryMatchingResultRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        /**
         战斗系统
         */

        [Serializable]
        public class FightSystemRequest
        {
            public string opt;
            public int room_id;
            public string param;
        }

        [Serializable]
        public class FightSystemResponse
        {
            public string msg;
            public string err_msg;
            public string opt;
            public int ret;

        }

        public static void FightSystemRequestCall(string opt, int room_id, string param)
        {
            FightSystemRequest request = new FightSystemRequest
            {
                opt = opt,
                room_id = room_id,
                param = param,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_FIGHT_SYSTEM_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static FightSystemResponse FightSystemRequestCallback(Message message)
        {
            try
            {
                FightSystemResponse response = null;

                response = JsonTools.UnSerializeFromString<FightSystemResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("FightSystemRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class RegisterRobotRequest
        {
            public int user_id;
            public int room_id;
            public int robot_key;
        }

        [Serializable]
        public class RegisterRobotResponse
        {
            public int robot_id;
            public string err_msg;
            public int ret;
            public int robot_key;
            public int born;

        }

        public static void RegisterRobotRequestCall(int user_id, int room_id, int robot_key)
        {
            RegisterRobotRequest request = new RegisterRobotRequest
            {
                user_id = user_id,
                room_id = room_id,
                robot_key = robot_key,
            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_REGISTER_ROBOT_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static RegisterRobotResponse RegisterRobotRequestCallback(Message message)
        {
            try
            {
                RegisterRobotResponse response = null;

                response = JsonTools.UnSerializeFromString<RegisterRobotResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("RegisterRobotRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }


        [Serializable]
        public class QueryBornPointRequest
        {
            public int user_id;
            public int room_id;

        }

        [Serializable]
        public class QueryBornPointResponse
        {
            public int user_id;
            public int born;
            public string err_msg;
            public int ret;
        }

        public static void QueryBornPointRequestCall(int user_id, int room_id)
        {
            QueryBornPointRequest request = new QueryBornPointRequest
            {
                user_id = user_id,
                room_id = room_id,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_QUERY_BORN_POINT_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static QueryBornPointResponse QueryBornPointRequestCallback(Message message)
        {
            try
            {
                QueryBornPointResponse response = null;

                response = JsonTools.UnSerializeFromString<QueryBornPointResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("QueryBornPointRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class SolveWeaponsRequest
        {
            public int wid;
            public int user_id;
        }

        [Serializable]
        public class SolveWeaponsResponse
        {
            public int wid;
            public string err_msg;
            public int ret;
            public int user_id;
        }

        public static void SolveWeaponsRequestCall(int user_id, int wid)
        {
            SolveWeaponsRequest request = new SolveWeaponsRequest
            {
                wid = wid,
                user_id = user_id,
            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_SOLVE_WEAPONS_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static SolveWeaponsResponse SolveWeaponsRequestCallback(Message message)
        {
            try
            {
                SolveWeaponsResponse response = null;

                response = JsonTools.UnSerializeFromString<SolveWeaponsResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("SolveWeaponsRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class AoeFreezeRequest
        {
            public int room_id;
            public string pos;

        }

        [Serializable]
        public class AoeFreezeResponse
        {
            public string pos;
            public string err_msg;
            public int ret;

        }

        public static void AoeFreezeRequestCall(int room_id, string pos)
        {
            AoeFreezeRequest request = new AoeFreezeRequest
            {
                room_id = room_id,
                pos = pos,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_AOE_FREEZE_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static AoeFreezeResponse AoeFreezeRequestCallback(Message message)
        {
            try
            {
                AoeFreezeResponse response = null;
                response = JsonTools.UnSerializeFromString<AoeFreezeResponse>(message.GetMessageBuffer());
                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("AoeFreezeRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class NewWeaponRequest
        {
            public int w_type;
            public int user_id;
            public int w_pos;

        }

        [Serializable]
        public class NewWeaponResponse
        {
            public int user_id;
            public int w_type;
            public string err_msg;
            public int ret;
            public int w_pos;

        }

        public static void NewWeaponRequestCall(int w_type, int user_id, int w_pos)
        {
            NewWeaponRequest request = new NewWeaponRequest
            {
                w_type = w_type,
                user_id = user_id,
                w_pos = w_pos,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_NEW_WEAPON_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static NewWeaponResponse NewWeaponRequestCallback(Message message)
        {
            try
            {
                NewWeaponResponse response = null;

                response = JsonTools.UnSerializeFromString<NewWeaponResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("NewWeaponRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }

        [Serializable]
        public class AddHpRequest
        {
            public int hp;
            public int user_id;

        }

        [Serializable]
        public class AddHpResponse
        {
            public string err_msg;
            public int ret;

        }

        public static void AddHpRequestCall(int hp, int user_id)
        {
            AddHpRequest request = new AddHpRequest
            {
                hp = hp,
                user_id = user_id,

            };

            Message message = new Message();
            message.PackBuffer(ServiceID.GAME_MGR_ADD_HP_SERVICE, JsonTools.SerializeToString(request));
            NetworkMgr.Instance.Send(message);
        }

        public static AddHpResponse AddHpRequestCallback(Message message)
        {
            try
            {
                AddHpResponse response = null;

                response = JsonTools.UnSerializeFromString<AddHpResponse>(message.GetMessageBuffer());

                return response;
            }
            catch (Exception ex)
            {
                Debug.Log("AddHpRequestCallback parse error. " + ex.ToString());
                return null;
            }
        }
    }

}
