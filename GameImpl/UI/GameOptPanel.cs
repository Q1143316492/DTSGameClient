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
    public class GameOptPanel : PanelBase
    {
        Button btnBeginComputerGame = null;         // 人机对战
        Button btnBeginMatching = null;             // 开始匹配
        Button btnCancelMatching = null;            // 取消匹配


        Button btnTurnBackToInitRoom = null;        // 回到大厅
        Button btnSetting = null;                   // 设置
        Button btnUnlogin = null;                   // 注销登入
        Button btnExitGame = null;                  // 退出游戏

        private readonly string GAME_SCENE = ScenePath.LEVEL_EMPTY;
        private readonly string BEGIN_ROOM = ScenePath.LEVEL_00;

        private bool InInitRoom;
        private int userID = 0;
        private float matching_time = 15;           // 秒

        public override void Show()
        {
            OperationMode.Instance.Lock();
        }

        public override void Hide()
        {
            OperationMode.Instance.Unlock();
        }

        private void TurnBackToGame()
        {
            UIMgr.Instance.HidePanel(UIPanelPath.GAME_OPT);
            UIMgr.Instance.ShowPanel<GameRunPanel>(UIPanelPath.GAME_RUN);
        }

        void InitParam()
        {
            object isInInitRoom = MemeryCacheMgr.Instance.Get(DTSKeys.IS_IN_INIT_ROOM);
            if (isInInitRoom == null)
            {
                InInitRoom = true;
            }
            else
            {
                InInitRoom = (bool) isInInitRoom;
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
        }
        
        void Start()
        {
            InitParam();
            RegisterPlayWithRobotEvent();

            RegisterMatchingEvent();

            RegisterTurnBackToInitRoom();

            if (InInitRoom)
            {
                btnBeginComputerGame.gameObject.SetActive(true);
                btnBeginMatching.gameObject.SetActive(true);
                btnCancelMatching.gameObject.SetActive(false);
                btnTurnBackToInitRoom.gameObject.SetActive(false);
            }
            else
            {
                btnBeginComputerGame.gameObject.SetActive(false);
                btnBeginMatching.gameObject.SetActive(false);
                btnCancelMatching.gameObject.SetActive(false);
                btnTurnBackToInitRoom.gameObject.SetActive(true);
            }

            btnSetting = GetControl<Button>("btn_setting");
            btnSetting.onClick.AddListener(() =>
            {
                UIMgr.Instance.ShowPanel<SettingPanel>(UIPanelPath.SETTING);
            });

            btnUnlogin = GetControl<Button>("btn_un_login");
            btnUnlogin.onClick.AddListener(() => { Application.Quit(); });

            btnExitGame = GetControl<Button>("btn_exit_game");
            btnExitGame.onClick.AddListener(() => { Application.Quit(); });

        }

        void OnDestroy()
        {
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.GAME_MGR_PLAY_ALONE_SERVICE, GameMgrPlayerAloneCallback);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.GAME_MGR_PLAY_WITH_OTHERS_SERVICE, GameMgrPlayerWithOthers);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.GAME_MGR_QUERY_MATCHING_RESULT_SERVICE, GameMgrQueryMathingResult);
        }
        
        private void BeginGameAndChangeScene(int room_id)
        {
            MemeryCacheMgr.Instance.Set(DTSKeys.IS_IN_INIT_ROOM, false);
            UIMgr.Instance.HidePanel(UIPanelPath.GAME_OPT);
            UIMgr.Instance.HidePanel(UIPanelPath.GAME_RUN);
            MemeryCacheMgr.Instance.Set(DTSKeys.ROOM_ID, room_id);
            ScenceMgr.Instance.LoadSceneAsyncUseLoadingBarBegin(GAME_SCENE);
        }

        void RegisterTurnBackToInitRoom()
        {
            btnTurnBackToInitRoom = GetControl<Button>("btn_to_init_room");
            
            btnTurnBackToInitRoom.onClick.AddListener(() =>
            {
                if (!InInitRoom)
                {
                    MemeryCacheMgr.Instance.Set(DTSKeys.ROOM_ID, -1);
                    MemeryCacheMgr.Instance.Set(DTSKeys.IS_IN_INIT_ROOM, true);
                    UIMgr.Instance.HidePanel(UIPanelPath.GAME_OPT);
                    ScenceMgr.Instance.LoadSceneAsyncUseLoadingBarBegin(BEGIN_ROOM);
                }
            });

        }

        private void GameMgrPlayerAloneCallback(Message msg)
        {
            GameMgrRouter.PlayAloneResponse response = NetworkMgr.ParseCallback<GameMgrRouter.PlayAloneResponse>(msg);
            if (response.ret == 0)
            {
                BeginGameAndChangeScene(response.room_id);
            }
        }

        private void RegisterPlayWithRobotEvent()
        {
            btnBeginComputerGame = GetControl<Button>("btn_play_computer");
            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_PLAY_ALONE_SERVICE, GameMgrPlayerAloneCallback);

            btnBeginComputerGame.onClick.AddListener(() =>
            {
                GameMgrRouter.PlayAloneRequestCall(userID);
            });
        }

        private void GameMgrPlayerWithOthers(Message msg)
        {
            GameMgrRouter.PlayWithOthersResponse response = NetworkMgr.ParseCallback<GameMgrRouter.PlayWithOthersResponse>(msg);
            if (response.ret == 0)
            {
                if (response.mode == 1)  // 开始匹配成功
                {
                    if (MemeryCacheMgr.Instance.Get(DTSKeys.MATCHING) is true)
                    {
                        return;
                    }
                    MemeryCacheMgr.Instance.Set(DTSKeys.MATCHING, true);

                    btnBeginComputerGame.gameObject.SetActive(false);
                    btnBeginMatching.gameObject.SetActive(false);
                    btnCancelMatching.gameObject.SetActive(true);

                    MonoMgr.Instance.StartDelayEventMultiTimes(1000, (int)matching_time, () =>
                    {
                        // 发起 matching_time 次查询，每次间隔 1000 ms
                        if (MemeryCacheMgr.Instance.Get(DTSKeys.MATCHING) is false)
                        {
                            return;
                        }
                        GameMgrRouter.QueryMatchingResultRequestCall(2, userID);
                    });
                }
                else if (response.mode == 2)    // 取消匹配成功
                {

                }
            }
        }

        private void GameMgrQueryMathingResult(Message msg)
        {
            if (MemeryCacheMgr.Instance.Get(DTSKeys.MATCHING) is false)
            {
                return;
            }

            GameMgrRouter.QueryMatchingResultResponse response = GameMgrRouter.QueryMatchingResultRequestCallback(msg);
            if (response.ret == 0 && response.room_id > 0)
            {
                MemeryCacheMgr.Instance.Set(DTSKeys.MATCHING, false);
                BeginGameAndChangeScene(response.room_id);
            }
            else
            {
                Debug.Log("query matching result. not ready. msg = " + response.err_msg);
            }
        }

        private void RegisterMatchingEvent()
        {
            btnBeginMatching = GetControl<Button>("btn_play_matching");

            btnBeginMatching.onClick.AddListener(() =>
            {
                object isMatching = MemeryCacheMgr.Instance.Get(DTSKeys.MATCHING);
                if (isMatching is false || isMatching is null)
                {
                    GameMgrRouter.PlayWithOthersRequestCall(userID, 1, matching_time);
                }
            });

            btnCancelMatching = GetControl<Button>("btn_cancel_matching");

            btnCancelMatching.onClick.AddListener(() =>
            {
                if (MemeryCacheMgr.Instance.Get(DTSKeys.MATCHING) is true)
                {
                    btnBeginComputerGame.gameObject.SetActive(true);
                    btnBeginMatching.gameObject.SetActive(true);
                    btnCancelMatching.gameObject.SetActive(false);
                    MemeryCacheMgr.Instance.Set(DTSKeys.MATCHING, false);
                    GameMgrRouter.PlayWithOthersRequestCall(userID, 2, matching_time);
                }
            });

            // 注册开始匹配，服务器同意结果 的回调函数
            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_PLAY_WITH_OTHERS_SERVICE, GameMgrPlayerWithOthers);

            // 查询匹配结果回调, 发起匹配后没一秒查询一次
            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_QUERY_MATCHING_RESULT_SERVICE, GameMgrQueryMathingResult);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIMgr.Instance.HidePanel(UIPanelPath.GAME_OPT);
                UIMgr.Instance.ShowPanel<GameRunPanel>(UIPanelPath.GAME_RUN);
                //TurnBackToGame();
            }
            
        }
    }
}
