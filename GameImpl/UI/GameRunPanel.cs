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
    public class GameRunPanel : PanelBase
    {
        private Slider hpSlider = null;

        private class WeaponMessageBox
        {
            public Text weaponName = null;
            public Text bulletCount1 = null;
            public Text bulletCount2 = null;
        }
        private WeaponMessageBox weaponMessage = new WeaponMessageBox();

        private class GoalPanel
        {
            public Text totalKillCount = null;
            public Text totalPlayerCount = null;
        }
        private GoalPanel goalPanel = new GoalPanel();

        private Image centerStarIcon = null;
        private Vector2 centerStarIconInitSize = Vector2.zero;

        private Text txtEve = null;

        private Image attackIcon = null;

        public void SetHp(float hp)
        {
            if (hpSlider != null)
            {
                hpSlider.value = hp;
            }
        }

        void StartCenterStart()
        {
            centerStarIcon = GetControl<Image>("center_star");
            centerStarIconInitSize = centerStarIcon.GetComponent<RectTransform>().sizeDelta;
            MvvmMgr.Instance.RegisterCallback(UICacheKeys.CENTER_STAR_ICON_EXPAND, (ratio) =>
            {
                try
                {
                    centerStarIcon.GetComponent<RectTransform>().sizeDelta = centerStarIconInitSize * (float)ratio;
                }
                catch (Exception) {}
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.CENTER_STAR_ICON_EXPAND);

            attackIcon = GetControl<Image>("center_star_hurt");
            MvvmMgr.Instance.RegisterCallback(UICacheKeys.HURT_ICON_SHOW, (isShow) =>
            {
                try
                {
                    bool show = (bool)isShow;
                    if (show)
                    {
                        Color color = attackIcon.color;
                        color.a = 1;
                        attackIcon.color = color;
                    }
                    else
                    {
                        Color color = attackIcon.color;
                        color.a = 0;
                        attackIcon.color = color;
                    }
                }
                catch (Exception)
                {
                    //Debug.Log("center_star_hurt callback fail. " + ex.ToString());
                }
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.HURT_ICON_SHOW);
        }

        void StartWeaponMessageBox()
        {
            // 武器名称
            weaponMessage.weaponName = GetControl<Text>("weapon_name_txt");
            MvvmMgr.Instance.RegisterCallback(UICacheKeys.WEAPON_NAME, (name) =>
            {
                try
                {
                    weaponMessage.weaponName.text = name.ToString();
                }
                catch (Exception) { }
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.WEAPON_NAME);

            // 子弹数
            weaponMessage.bulletCount1 = GetControl<Text>("bullet_txt_1");
            weaponMessage.bulletCount2 = GetControl<Text>("bullet_txt_2");

            MvvmMgr.Instance.RegisterCallback(UICacheKeys.BULLET_COUNT_FIRST, (bullet) =>
            {
                try
                {
                    weaponMessage.bulletCount1.text = ((int)bullet).ToString();
                }
                catch (Exception) {}
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.BULLET_COUNT_FIRST);

            MvvmMgr.Instance.RegisterCallback(UICacheKeys.BULLET_COUNT_SECOND, (bullet) =>
            {
                try
                {
                    weaponMessage.bulletCount2.text = ((int)bullet).ToString();
                }
                catch (Exception) {}
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.BULLET_COUNT_SECOND);
        }

        void StartGoalPanel()
        {
            goalPanel.totalKillCount = GetControl<Text>("total_kill_txt");
            MvvmMgr.Instance.RegisterCallback(UICacheKeys.TOTAL_KILL, (count) =>
            {
                try
                {
                    goalPanel.totalKillCount.text = ((int)count).ToString();
                }
                catch (Exception) { }
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.TOTAL_KILL);

            goalPanel.totalPlayerCount = GetControl<Text>("total_player_txt");
            MvvmMgr.Instance.RegisterCallback(UICacheKeys.TOTAL_PLAYER, (count) =>
            {
                try
                {
                    goalPanel.totalPlayerCount.text = ((int)count).ToString();
                }
                catch (Exception) { }
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.TOTAL_PLAYER);
        }

        void StartPlayerHP()
        {
            hpSlider = GetControl<Slider>("player_hp");
            
            MvvmMgr.Instance.RegisterCallback(UICacheKeys.PLAYER_HP, (obj) =>
            {
                try
                {
                    float value = (float) obj;
                    hpSlider.value = value;
                }
                catch (Exception) { }
            });
            MvvmMgr.Instance.Fresh(UICacheKeys.PLAYER_HP);
        }

        UIType<int> eve = new UIType<int>(UICacheKeys.EVE, 0);

        void QueryEveCallback(Message msg)
        {
            UserRouter.UserLevelResponse res = UserRouter.UserLevelRequestCallback(msg);
            if (res.ret != 0)
            {
                return;
            }

            if (res.opt == 2)
            {
                eve.val = res.val;
            }
        }

        void Start()
        {
            //NetworkMgr.Instance.AddMsgListener(ServiceID.USER_USER_LEVEL_SERVICE, QueryEveCallback);

            //int userID = (int)MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);
            //UserRouter.UserLevelRequestCall(2, userID, 0);

            StartPlayerHP();
            StartCenterStart();
            StartWeaponMessageBox();
            StartGoalPanel();

            //txtEve = GetControl<Text>("eve_txt");

            //MvvmMgr.Instance.RegisterCallback(UICacheKeys.EVE, (obj) =>
            //{
            //    Debug.Log(obj);
            //    txtEve.text = "123";
            //});
        }

        void Destroy()
        {
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.USER_USER_LEVEL_SERVICE, QueryEveCallback);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIMgr.Instance.HidePanel(UIPanelPath.GAME_RUN);
                UIMgr.Instance.ShowPanel<GameOptPanel>(UIPanelPath.GAME_OPT);
            }
        }
    }
}
