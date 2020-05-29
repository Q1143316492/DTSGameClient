using CWLEngine.Core.Manager;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Network;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Controller
{
    public class OtherPlayerController : MonoBehaviour
    {
        private static float SYNC_TRANSFORM_FREQUENT = NetworkFrequency.SYNC_TRANSFORM_FREQUENT;
        private static float SYNC_ACTION_FREQUENT = NetworkFrequency.SYNC_ACTION_FREQUENT;

        public Soldier soldier;

        Transform leftHandIKTransform;
        Transform rightHandIKTransform;

        StateContex contex = new StateContex();

        public OtherPlayerController()
        {
            
        }

        void Start()
        {
            NetworkMgr.Instance.AddMsgListener(ServiceID.SYNCHRONIZATION_QUERY_USER_TRANSFORM_SERVICE, UpdateTransformCallback);
            NetworkMgr.Instance.AddMsgListener(ServiceID.ROOM_QUERY_USER_BELONGED_ROOM_SERVICE, CheckPlayerOutCallback);
        }

        void OnDestroy()
        {
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.SYNCHRONIZATION_QUERY_USER_TRANSFORM_SERVICE, UpdateTransformCallback);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.ROOM_QUERY_USER_BELONGED_ROOM_SERVICE, CheckPlayerOutCallback);

            soldier.Destory();
            CancelInvoke();
        }

        void CheckPlayerOutCallback(Message msg)
        {
            RoomOptRouter.QueryUserBelongRoomResponse res = RoomOptRouter.QueryUserBelongRoomCallback(msg);
            if (res.ret != 0 || res.room_id != soldier.GetRoomID())
            {
                EventMgr.Instance.EventTrigger(EventName.PLAYER_OUT, soldier.GetUserID());
            }
        }
        
        private Skill queryTransform = new Skill("QueryTransform", SYNC_TRANSFORM_FREQUENT);
        private Skill checkPlayerOut = new Skill("CheckPlayerOut", 1000);

        void Update()
        {
            if (queryTransform.CheckAndRun())
            {
                UserSynchronizationRouter.QueryUsersTransform(soldier.GetUserID());
            }

            if (checkPlayerOut.CheckAndRun())
            {
                RoomOptRouter.QueryUserBelongRoom(soldier.GetUserID());
            }
        }

        void FixedUpdate()
        {
            soldier.RenderOperation();
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

            if (leftHandIKTransform != null)
            {
                soldier.animatorHandler.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                soldier.animatorHandler.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTransform.position);
            }

            if (rightHandIKTransform != null)
            {
                soldier.animatorHandler.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                soldier.animatorHandler.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTransform.position);
            }
        }
        
        private void UpdateTransformCallback(Message msg)
        {
            UserSynchronizationRouter.QueryUserTransformResponse res = UserSynchronizationRouter.QueryUsersTransformCallback(msg);
            if (res.ret == 0 && res.user_id == soldier.GetUserID())
            {
                soldier.SyncTransform(res.position, res.rotation);
            }
        }
    }
}
