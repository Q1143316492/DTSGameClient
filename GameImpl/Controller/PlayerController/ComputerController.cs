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
    /**
     update 里面的 ControllerMode.SELF ControllerMode.Network
     
        机器人加载是一个空物体。会向服务端强控制权。抢到了是SELF。否则是Network从服务端同步数据。
         */
    public class ComputerController : MonoBehaviour
    {
        private static float ROBOT_LOST_GOAL_TIME = 1000;

        private readonly float MODEL_MIN_DISTANCE = DTSKeys.MODEL_MIN_DISTANCE;
        private readonly float KNIFE_ATTACK_DISTANCE = DTSKeys.KNIFE_ATTACK_DISTANCE;
        private readonly float GUN_ATTACK_DISTANCE = DTSKeys.GUN_ATTACK_DISTANCE;

        private static readonly float SYNC_TRANSFORM_FREQUENT = NetworkFrequency.SYNC_TRANSFORM_FREQUENT;
        private static readonly float SYNC_ACTION_FREQUENT = NetworkFrequency.SYNC_ACTION_FREQUENT;

        private static float SYNC_CONTROLLER_FREQUENT = NetworkFrequency.HEART_BEAT * 1000;  // * 1000 后单位 秒
        private Skill syncController = new Skill("SYNC_CONTROLLER", SYNC_CONTROLLER_FREQUENT);
        
        public Soldier soldier;

        Transform leftHandIKTransform;
        Transform rightHandIKTransform;

        PlayerAction action = new PlayerAction();

        Vector3 bornPoint;

        GameObject targetObject;
        GameObject carrotTarget;
        bool isLockAnemey = false;

        public enum ControllerMode
        {
            UNDEFINE,
            SELF,
            NETWORK,
        }
        public ControllerMode controllerMode = ControllerMode.UNDEFINE;

        void RobotStop()
        {
            action.Init();
        }

        void Start()
        {
            RobotStop();
            NetworkMgr.Instance.AddMsgListener(ServiceID.SYNCHRONIZATION_QUERY_USER_TRANSFORM_SERVICE, UpdateTransformCallback);
            EventMgr.Instance.AddEventListener(EventName.ROBOT_ATTACK_EVENT, CheckRobotAttackEvent);
            EventMgr.Instance.AddEventListener(EventName.SET_CARROT + "#" + soldier.GetUserID().ToString(), SetCarrot);

            carrotTarget = new GameObject("goal#" + soldier.GetUserID().ToString());
            carrotTarget.transform.position = soldier.GetTransform().position;
        }

        void OnDestroy()
        {
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.SYNCHRONIZATION_QUERY_USER_TRANSFORM_SERVICE, UpdateTransformCallback);
            EventMgr.Instance.DelEventListener(EventName.ROBOT_ATTACK_EVENT, CheckRobotAttackEvent);
            EventMgr.Instance.AddEventListener(EventName.SET_CARROT + "#" + soldier.GetUserID().ToString(), SetCarrot);
        }

        private Skill reportTransform = new Skill("ReportTransform", SYNC_TRANSFORM_FREQUENT);
        private Skill queryTransform = new Skill("QueryTransform", SYNC_TRANSFORM_FREQUENT);
        private Skill reportActions = new Skill("ReportActions", SYNC_ACTION_FREQUENT);
        private Skill robotLostGoal = new Skill("RobotLostGoal", ROBOT_LOST_GOAL_TIME);
        private Skill robotFindRoad = new Skill("RobotFindRoal", ROBOT_LOST_GOAL_TIME);
        private Skill attackCD = new Skill("Attack", 1000);

        bool first = true;

        void SetCarrot(object obj)
        {
            if (obj is Vector3 pos)
            {
                carrotTarget.transform.position = pos;
            }
        }

        void RandomWeapon()
        {
            // TODO 这里初始化武器有一点bug 所以延时了下。待修复。。。
            MonoMgr.Instance.StartDelayEvent(1000, () =>
            {
                GameMgrRouter.SolveWeaponsRequestCall(soldier.GetUserID(), UnityEngine.Random.Range(0, 3));
            });

        }

        readonly int[,] Next = new int[4, 2] { { -1, 0 }, { 1, 0 }, { 0, 1 }, { 0, -1 } };

        void Update()
        {
            if (controllerMode == ControllerMode.SELF)
            {
                if (first)
                {
                    first = false;
                    RandomWeapon();
                }
                
                if (soldier.IsFreeze())
                {
                    RobotStop();
                    soldier.UpdateOperation(action);
                    UnLockAnemey();
                    if (reportTransform.CheckAndRun())
                    {
                        UserSynchronizationRouter.ReportTransform(soldier.GetUserID(), soldier.GetTransform());
                    }
                    return;
                }

                if (robotFindRoad.CheckAndRun())
                {
                    //EventMgr.Instance.EventTrigger(EventName.FIND_ROAD, soldier.GetUserID());
                }

                if (robotLostGoal.CheckAndRun())
                {
                    UnLockAnemey();
                }

                EventMgr.Instance.EventTrigger(EventName.NEUTRAL_ROBOT_CHOOSE_GOAL, soldier.GetUserID());

                CalculateMove();
                
                if (!soldier.Death() && attackCD.CheckAndRun())
                {
                    AttackCheck();
                }

                if (reportTransform.CheckAndRun())
                {
                    UserSynchronizationRouter.ReportTransform(soldier.GetUserID(), soldier.GetTransform());
                }

                if (reportActions.CheckAndRun())
                {
                    ReportActions();
                }
                soldier.UpdateOperation(action);
            }
            else if (controllerMode == ControllerMode.NETWORK)
            {
                if (queryTransform.CheckAndRun())
                {
                    UserSynchronizationRouter.QueryUsersTransform(soldier.GetUserID());
                }
            }
        }

        void FixedUpdate()
        {
            soldier.RenderOperation();
        }

        void CheckRobotAttackEvent(object obj)
        {
            if (obj is RobotAttackEvent attackEvent && attackEvent.robotID == soldier.GetUserID())
            {
                LockAnemey(attackEvent.target);
            }
        }

        void UpdateTransformCallback(Message msg)
        {
            UserSynchronizationRouter.QueryUserTransformResponse res = UserSynchronizationRouter.QueryUsersTransformCallback(msg);
            if (res.ret == 0 && res.user_id == soldier.GetUserID())
            {
                soldier.SyncTransform(res.position, res.rotation);
            }
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

        public void LockAnemey(GameObject obj)
        {
            if (obj == null) return ;

            isLockAnemey = true;
            targetObject = obj;
        }

        public void UnLockAnemey()
        {
            isLockAnemey = false;
            targetObject = null;
        }

        private void KnifeAttackSuccess()
        {
            string[] ss = targetObject.gameObject.name.Split('#');
            if (ss.Length == 2 && "PlayerA".Equals(ss[0]))
            {
                try
                {
                    int userID = Convert.ToInt32(ss[1]);
                    
                    EventMgr.Instance.EventTrigger(EventName.PLAYER_ATTACKED, new RoleHurtEvent(soldier.GetUserID(), userID, 
                        soldier.bag.GetNowWeapon().GetHurt()));
                }
                catch (Exception) { }
            }
            else if (ss.Length == 1 && "PlayerA".Equals(ss[0]))
            {
                try
                {
                    int userID = (int) MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);
                    EventMgr.Instance.EventTrigger(EventName.PLAYER_ATTACKED, new RoleHurtEvent(soldier.GetUserID(), userID, 
                        soldier.bag.GetNowWeapon().GetHurt()));
                }
                catch (Exception) { }
            }
        }

        private void GunAttackSuccess()
        {
            string[] splitName = targetObject.gameObject.name.Split('#');

            if (splitName.Length <= 2 && splitName[0] == "PlayerA")
            {
                try
                {
                    int userID = -1;

                    if (splitName.Length == 1)
                    {
                        userID = (int)MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);
                    }
                    else
                    {
                        userID = Convert.ToInt32(splitName[1]);
                    }

                    if (UnityEngine.Random.Range(0F, 1F) < 0.5F)
                    {
                        EventMgr.Instance.EventTrigger(EventName.PLAYER_ATTACKED, new RoleHurtEvent(soldier.GetUserID(), userID,
                            soldier.bag.GetNowWeapon().GetHurt()));
                    }
                }
                catch (Exception) { }
            }
        }

        public void AttackCheck()
        {
            if (soldier.bag.GetNowWeapon() == null || targetObject == null)
            {
                return;
            }
            
            float dis = Vector3.Distance(soldier.GetTransform().position, targetObject.transform.position);

            bool isFire = false;

            if (soldier.bag.GetNowWeapon() is WeaponKnife knife)
            {
                if (dis < KNIFE_ATTACK_DISTANCE)
                {
                    isFire = true;
                    action.Add(EPlayerAction.SHOOT);
                    KnifeAttackSuccess();
                }
                return;
            }

            if (dis < GUN_ATTACK_DISTANCE)
            {
                isFire = true;
                action.Add(EPlayerAction.SHOOT);
                GunAttackSuccess();
                return;
            }

            if (!isFire)
            {
                action.Del(EPlayerAction.SHOOT);
            }
        }

        private Skill actionCD = new Skill("ActionCD", 66);    // 角色不断的在有行为和无行为切换会闪烁，加个检测CD

        private void CalculateMoveForwardAndBackword(Vector3 target, Vector3 now)
        {
            Vector3 forward = soldier.GetTransform().forward;
            Vector3 targetDir = target - now;

            forward.y = 0;
            targetDir.y = 0;

            float c = Vector3.Dot(forward, targetDir);
            float angle = Mathf.Acos(Vector3.Dot(forward.normalized, targetDir.normalized)) * Mathf.Rad2Deg;
            
            if (Vector3.Distance(now, target) < MODEL_MIN_DISTANCE)
            {
                action.Add(EPlayerAction.END_RUN);
                StopMoveForwardAndBackword();
            }
            else
            {
                if (angle > 90)
                {
                    action.Add(EPlayerAction.BEGIN_RUN);
                    MoveBackword();
                }
                else
                {
                    action.Add(EPlayerAction.BEGIN_RUN);
                    MoveForward();
                }
            }
        }

        private void CalculateRotate(Vector3 target, Vector3 now)
        {
            Vector3 forward = soldier.GetTransform().forward;
            Vector3 targetDir = target - now;

            forward.y = 0;
            targetDir.y = 0;

            float c = Vector3.Dot(forward, targetDir);
            float angle = Mathf.Acos(Vector3.Dot(forward.normalized, targetDir.normalized)) * Mathf.Rad2Deg;
            float dir = Vector3.Cross(forward, targetDir).y;

            if (angle < 10)
            {
                RobotStopRotate();
            }
            else
            {
                if (dir > 0)
                {
                    RotateRight();
                }
                else
                {
                    RotateLeft();
                }
            }
        }

        private void CalculateMove()
        {
            Vector3 targetPosition = new Vector3();
            if (!isLockAnemey || targetObject == null)
            {
                targetPosition = carrotTarget.transform.position;
            }
            else
            {
                targetPosition = targetObject.transform.position;
            }

            targetPosition.y = soldier.GetTransform().position.y;
            float dis = Vector3.Distance(soldier.GetTransform().position, targetPosition);

            if (isLockAnemey && dis > 20)
            {
                RobotStop();
                return;
            }

            //if (soldier.bag.GetNowWeapon() is WeaponKnife == false)
            //{
            //    // 拿枪的时候不需要帖太近
            //    if (dis < GUN_ATTACK_DISTANCE * 0.5)
            //    {
            //        return ;
            //    }
            //}

            if (actionCD.CheckAndRun())
            {
                Vector3 target = targetPosition;
                Vector3 now = soldier.GetTransform().position;
                CalculateMoveForwardAndBackword(target, now);
                CalculateRotate(target, now);
            }
        }
        
        public void RobotStopRotate()
        {
            action.SetMouseX(0);
        }

        public void RotateLeft()
        {
            action.SetMouseX(-1);
        }

        public void RotateRight()
        {
            action.SetMouseX(1);
        }

        public void MoveLeft()
        {
            action.SetMoveH(-1);
        }

        public void MoveRight()
        {
            action.SetMoveH(1);
        }

        public void StopMoveLeftAndRight()
        {
            action.SetMoveH(0);
        }

        public void MoveForward()
        {
            action.SetMoveV(1);
        }

        public void MoveBackword()
        {
            action.SetMoveV(-1);
        }

        public void StopMoveForwardAndBackword()
        {
            action.SetMoveV(0);
        }
    }
}
