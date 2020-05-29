using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Network;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Controller
{
    // 每一次开启游戏，会有一个 GameController 控制同步整个场景所有玩家的行为

    public class GameController : MonoBehaviour
    {
        private PlayersController playersController = null;  // 控制玩家
        
        private static readonly float SYNC_ACTION_FREQUENT = NetworkFrequency.SYNC_ACTION_FREQUENT;

        int roomID = -1;                // 当前角色加入游戏房间的ID
        int userID = -1;                // 当前角色ID

        bool ready = false;

        void Awake()
        {
            if (MemeryCacheMgr.Instance.Get(DTSKeys.IS_IN_INIT_ROOM) is null)
            {
                MemeryCacheMgr.Instance.Set(DTSKeys.IS_IN_INIT_ROOM, true);
            }
            
            try
            {
                userID = (int) MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);
            }
            catch (Exception ex)
            {
                Debug.Log("userID is not exist. err: " + ex.ToString());
                userID = 0;
            }
            
            try
            {
                roomID = (int) MemeryCacheMgr.Instance.Get(DTSKeys.ROOM_ID);
            }
            catch (Exception)
            {
                roomID = -1;
            }
        }

        void Start()
        {
            NetworkMgr.Instance.CheckNetwork();

            NetworkMgr.Instance.AddMsgListener(ServiceID.ROOM_ENTER_ROOM_SERVICE, UserEnterRoomCallback);
            NetworkMgr.Instance.AddMsgListener(ServiceID.ROOM_QUERY_ROOM_USERS_SERVICE, CheckUserJoinCallback);
            NetworkMgr.Instance.AddMsgListener(ServiceID.SYNCHRONIZATION_QUERY_ACTION_SERVICE, QueryActionCallback);
            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_REGISTER_ROBOT_SERVICE, GetRobotControllerCallback);

            EventMgr.Instance.AddEventListener(EventName.FIGHT_ROBOT_CONTROLLER, GetRobotController);
            EventMgr.Instance.AddEventListener(EventName.GAME_OVER, GameOver);

            BeginGame();
        }

        void BeginGame()
        {
            if (roomID == -1)
            {
                RoomOptRouter.UserEnterRoom(userID, 1, 0);
            }
            else
            {
                ReadyGame();
            }
        }

        void OnGUI()
        {
            //if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
            //    BeginGame();
        }

        void OnDestroy()
        {
            CancelInvoke();

            gameMapController.Stop();

            NetworkMgr.Instance.RemoveMsgListener(ServiceID.ROOM_ENTER_ROOM_SERVICE, UserEnterRoomCallback);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.ROOM_QUERY_ROOM_USERS_SERVICE, CheckUserJoinCallback);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.SYNCHRONIZATION_QUERY_ACTION_SERVICE, QueryActionCallback);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.GAME_MGR_REGISTER_ROBOT_SERVICE, GetRobotControllerCallback);

            EventMgr.Instance.DelEventListener(EventName.FIGHT_ROBOT_CONTROLLER, GetRobotController);
            EventMgr.Instance.DelEventListener(EventName.GAME_OVER, GameOver);
            playersController.Destroy();
        }

        private void GameOver(object obj)
        {
            if (obj is GameResult result)
            {
                UIType<string> resultTitle = new UIType<string>(UICacheKeys.GAME_RESULT, "");
                UIMgr.Instance.ShowPanel<GameResultPanel>(UIPanelPath.RESULT_PANEL);
                if (result.win)
                {
                    resultTitle.val = "胜利";
                }
                else
                {
                    resultTitle.val = "失败";
                }

            }
        }

        private void GetRobotController(object obj)
        {
            int robotKey = (int)obj;
            GameMgrRouter.RegisterRobotRequestCall(userID, roomID, robotKey);
        }

        private void GetRobotControllerCallback(Message message)
        {
            GameMgrRouter.RegisterRobotResponse res = GameMgrRouter.RegisterRobotRequestCallback(message);
            if (res.ret == 0)
            {
                playersController.AddRobot(res.robot_id);
            }
        }

        GameMapController gameMapController = new GameMapController();

        private void ReadyGame()
        {
            if (roomID != 0)
            {
                gameMapController.CreateStatic();
            }

            UIMgr.Instance.ShowPanel<GameRunPanel>(UIPanelPath.GAME_RUN);
            MemeryCacheMgr.Instance.Set(DTSKeys.ROOM_ID, roomID);
            HeartBeat.Instance.Start();
            FrameSyncMgr.Instance.ReStart();
            playersController = new PlayersController(userID, roomID);

            ready = true;
        }

        private void UserEnterRoomCallback(Message msg)
        {
            RoomOptRouter.EnterRoomResponse res = RoomOptRouter.UserEnterRoomCallback(msg);
            if (res.ret == 0)
            {
                roomID = res.room_id;
                ReadyGame();
                NetworkMgr.Instance.RemoveMsgListener(ServiceID.ROOM_ENTER_ROOM_SERVICE, UserEnterRoomCallback);
            }
        }

        private Skill checkJoin = new Skill("checkJoin", 1000);
        private Skill updateAction = new Skill("updateAction", SYNC_ACTION_FREQUENT);
        
        private int lastQueryFrame = -1;

        void Update()
        {
            if (!ready)
            {
                return ;
            }
            
            if (checkJoin.CheckAndRun())
            {
                // 周期性查询玩家加入，请求成功会回调 CheckUserJoinCallback
                RoomOptRouter.QueryRoomUsers(roomID);
            }
            
            if (updateAction.CheckAndRun())
            {
                // 更新所有用户操作 回调 QueryActionCallback
                UserSynchronizationRouter.QueryActionRequestCall(userID, FrameSyncMgr.Instance.GetFrame());
            }
        }

        private void ParseAction(string []action)
        {
            try
            {
                int userID = Convert.ToInt32(action[0]);

                if (userID == this.userID || playersController.IsUnderController(userID))
                {
                    // 当前用户的行为已经表现过了
                    return;
                }

                int actionSign = Convert.ToInt32(action[1]);
                float moveH = (float)Convert.ToDecimal(action[2]);
                float moveV = (float)Convert.ToDecimal(action[3]);
                float mouseX = (float)Convert.ToDecimal(action[4]);
                float mouseY = (float)Convert.ToDecimal(action[5]);

                PlayerAction playerAction = new PlayerAction
                {
                    actionSign = actionSign
                };

                playerAction.SetMove(moveH, moveV);
                playerAction.SetMouse(mouseX, mouseY);
                playersController.UpdateOperation(userID, playerAction);
            }
            catch (Exception ex)
            {
                Debug.Log("action parse error: " + ex.ToString());
            }
        }

        private void QueryActionCallback(Message msg)
        {
            UserSynchronizationRouter.QueryActionResponse res = UserSynchronizationRouter.QueryActionRequestCallback(msg);
            if (res.ret == 0)
            {
                string [] actions = res.action.Split('#');
                for (int i = 0; i < actions.Length; i++)
                {
                    string[] action = actions[i].Split('|');
                    ParseAction(action);
                }
                FrameSyncMgr.Instance.SetFrame(res.frame);
            }
        }

        private void CheckUserJoinCallback(Message msg)
        {
            RoomOptRouter.QueryRoomUsersResponse res = RoomOptRouter.QueryRoomUsersCallback(msg);
            if (res.ret == 0)
            {
                string[] userIDList = res.user_id_list.Split(';');
                playersController.CompareUserSet(userIDList);
            }
        }
    }
}
