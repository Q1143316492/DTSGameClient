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
    public class GameResultPanel : PanelBase
    {
        Text resultTitle = null;
        Text killCount = null;

        Button btnTurnBackToInitRoom = null;

        private readonly string BEGIN_ROOM = ScenePath.LEVEL_00;

        void Start()
        {
            
            resultTitle = GetControl<Text>("title_result");
            MvvmMgr.Instance.RegisterCallback(UICacheKeys.GAME_RESULT, (result) =>
            {
                try
                {
                    resultTitle.text = result.ToString();
                }
                catch (Exception) { }
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.GAME_RESULT);

            killCount = GetControl<Text>("kill_count");
            MvvmMgr.Instance.RegisterCallback(UICacheKeys.TOTAL_KILL, (count) =>
            {
                try
                {
                    killCount.text = ((int)count).ToString();
                    int userID = (int)MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);
                    UserRouter.UserLevelRequestCall(1, userID, (int)count + 1);
                }
                catch (Exception) { }
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.TOTAL_KILL);


            btnTurnBackToInitRoom = GetControl<Button>("btn_out");
            btnTurnBackToInitRoom.onClick.AddListener(() =>
            {
                MemeryCacheMgr.Instance.Set(DTSKeys.ROOM_ID, -1);
                MemeryCacheMgr.Instance.Set(DTSKeys.IS_IN_INIT_ROOM, true);
                UIMgr.Instance.HidePanel(UIPanelPath.GAME_OPT);
                UIMgr.Instance.HidePanel(UIPanelPath.GAME_RUN);
                ScenceMgr.Instance.LoadSceneAsyncUseLoadingBarBegin(BEGIN_ROOM);
            });
        }
        
        void Update()
        {

        }
    }
}
