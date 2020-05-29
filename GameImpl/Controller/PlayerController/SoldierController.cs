using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using CWLEngine.GameImpl.Base;
using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Util;
using CWLEngine.GameImpl.Controller.Weapon;
using CWLEngine.GameImpl.Controller.SoldierState;
using CWLEngine.GameImpl.Network;
using CWLEngine.Core.Network;
using System.Text;

namespace CWLEngine.GameImpl.Controller
{
    public class SoldierController : MonoBehaviour
    {
        public float[] speeds = { 0.04f, 0.06f, 0.03f, 0.04f, 5.0f }; // 分别是 走，跑，蹲, 跳，角色左右视角旋转的速度

        private static readonly float SYNC_TRANSFORM_FREQUENT = NetworkFrequency.SYNC_TRANSFORM_FREQUENT;
        private static readonly float SYNC_ACTION_FREQUENT = NetworkFrequency.SYNC_ACTION_FREQUENT;
        
        public Soldier soldier;
        
        Transform leftHandIKTransform;
        Transform rightHandIKTransform;
        
        PlayerAction action = new PlayerAction();

        void Awake()
        {
            
        }

        void Start()
        {
            // 注册同步 Transform 的函数 上报和查询
            NetworkMgr.Instance.AddMsgListener(ServiceID.SYNCHRONIZATION_REPORT_TRANSFORM_SERVICE, UserSynchronizationRouter.ReportTransformCallback);

            // 注册同步 Transform 的函数 查询
            NetworkMgr.Instance.AddMsgListener(ServiceID.SYNCHRONIZATION_QUERY_USER_TRANSFORM_SERVICE, UpdateTransformCallback);

            // 注册同步 玩家操作 的函数 上报
            NetworkMgr.Instance.AddMsgListener(ServiceID.SYNCHRONIZATION_REPORT_ACTION_SERVICE, ReportActionCallback);
        }

        void OnDestroy()
        {
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.SYNCHRONIZATION_REPORT_TRANSFORM_SERVICE,
                UserSynchronizationRouter.ReportTransformCallback);

            NetworkMgr.Instance.RemoveMsgListener(ServiceID.SYNCHRONIZATION_QUERY_USER_TRANSFORM_SERVICE,
                UpdateTransformCallback);

            NetworkMgr.Instance.RemoveMsgListener(ServiceID.SYNCHRONIZATION_REPORT_ACTION_SERVICE,
                ReportActionCallback);
            
            soldier.Destory();
            CancelInvoke();
        }

        private void UpdateTransformCallback(Message msg)
        {
            UserSynchronizationRouter.QueryUserTransformResponse res = UserSynchronizationRouter.QueryUsersTransformCallback(msg);
            if (res.ret == 0 && res.user_id == soldier.GetUserID())
            {
                soldier.SyncTransform(res.position, res.rotation);
            }
        }

        private void ReportActionCallback(Message msg)
        {
            UserSynchronizationRouter.ReportActionResponse res = UserSynchronizationRouter.ReportActionRequestCallback(msg);
            if (res.ret == 0)
            {

            }
        }

        void OnGUI()
        {
            GUI.Label(new Rect(5, 5, 500, 500), DebugTools.Msg);
        }

        void OnAnimatorIK()
        {
            if (leftHandIKTransform == null)
            {
                leftHandIKTransform = Tools.FindChildrenTransform(gameObject, IKPosition.IKLeftHand);
            }
            if (rightHandIKTransform == null)
            {
                rightHandIKTransform = Tools.FindChildrenTransform(gameObject, IKPosition.IKRightHand);
            }

            if (leftHandIKTransform != null && soldier.NeedLeftIKPositon())
            {
                soldier.animatorHandler.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                soldier.animatorHandler.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTransform.position);
            }

            if (rightHandIKTransform != null && soldier.NeedRightIKPosition())
            {
                soldier.animatorHandler.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                soldier.animatorHandler.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTransform.position);
            }
        }

        private Skill reportTransform = new Skill("ReportTransform", SYNC_TRANSFORM_FREQUENT);
        private Skill queryTransform = new Skill("QueryTransform", SYNC_TRANSFORM_FREQUENT);

        private Skill reportActions = new Skill("ReportActions", SYNC_ACTION_FREQUENT);

        void Update()
        {
            soldier.SetSpeeds(speeds);  // 调手感用，编辑器调试时能让速度直接改变
            action.Init();

            if (OperationMode.Instance.IsLock())
            {
                if (reportTransform.CheckAndRun())
                {
                    UserSynchronizationRouter.ReportTransform(soldier.GetUserID(), soldier.GetTransform());
                }
                return;
            }

            if (soldier.IsFreeze())
            {
                EventMgr.Instance.EventTrigger(EventName.MAIN_PLAYER_ACTION, action);
                if (reportTransform.CheckAndRun())
                {
                    UserSynchronizationRouter.ReportTransform(soldier.GetUserID(), soldier.GetTransform());
                }
                return;
            }

            WeaponChangeAction();

            if (Input.GetMouseButton(0))
            {
                action.Add(EPlayerAction.SHOOT);
            }
            if (Input.GetMouseButton(1))
            {
                action.Add(EPlayerAction.SHOOT_BURST);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                action.Add(EPlayerAction.SHOOT_LINE);
            }

            if (Input.GetKey(KeyCode.R))
            {
                action.Add(EPlayerAction.RELOAD);
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                action.Add(EPlayerAction.BEGIN_RUN);
            }
            else
            {
                action.Add(EPlayerAction.END_RUN);
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                action.Add(EPlayerAction.BEGIN_CROUCH);
            }
            else
            {
                action.Add(EPlayerAction.END_CROUCH);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                action.Add(EPlayerAction.DUMP);
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            action.SetMove(h, v);
            action.SetMouse(mouseX, mouseY);

            // 在武器箱面前，按了E，换武器
            if (MemeryCacheMgr.Instance.Get(UICacheKeys.BULLET_BOX_WARN_MESSAGE) != null && Input.GetKeyDown(KeyCode.E))
            {
                if (MemeryCacheMgr.Instance.Get(UICacheKeys.WEAPON_BOX) is WeaponBoxBase weaponBox)
                {
                    soldier.SetWeapon(weaponBox.CreatorWeapon(), weaponBox.GetWeaponBagPos());
                    GameMgrRouter.NewWeaponRequestCall((int)weaponBox.GetWeaponTypeID(), soldier.GetUserID(), (int)weaponBox.GetWeaponBagPos());
                    weaponBox.Destory();
                }
            }
            
            // 触发直接影响角色行为。不等服务端校验完，体验好一点。
            EventMgr.Instance.EventTrigger(EventName.MAIN_PLAYER_ACTION, action);

            if (reportTransform.CheckAndRun())
            {
                UserSynchronizationRouter.ReportTransform(soldier.GetUserID(), soldier.GetTransform());
            }

            if (reportActions.CheckAndRun())
            {
                ReportActions();
            }
        }

        void FixedUpdate()
        {
            if (OperationMode.Instance.IsLock())
            {
                return;
            }
            soldier.RenderOperation();
        }

        private int lastReportFrame = -1;

        private void ReportActions()
        {
            int nowFrame = FrameSyncMgr.Instance.GetFrame();
            if (lastReportFrame != nowFrame)
            {
                lastReportFrame = nowFrame;
                UserSynchronizationRouter.ReportActionRequestCall(soldier.GetUserID(), action.ToNetworkString(), nowFrame);
            }
        }

        private void WeaponChangeAction()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeWeaponCall(WeaponBagPos.FIRST_WEAPON);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeWeaponCall(WeaponBagPos.SECOND_WEAPON);
            }
            else if (MemeryCacheMgr.Instance.Get(UICacheKeys.BULLET_BOX_WARN_MESSAGE) == null && Input.GetKeyDown(KeyCode.E))
            {
                ChangeWeaponCall(WeaponBagPos.KNIFE_WEAPON);
            }
        }

        public void ChangeWeaponCall(WeaponBagPos pos)
        {
            GameMgrRouter.SolveWeaponsRequestCall(soldier.GetUserID(), (int)pos);
        }
    }
}
