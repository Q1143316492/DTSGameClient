using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;

namespace CWLEngine.GameImpl.Controller.SoldierState
{
    public class SoldierStateJump : SoldierStateBase
    {
        // 跳跃力的大小
        static float force = 350f;

        private SoldierStateBase preState = null;
        private float dumpTime = 0;

        public override void Enter(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            string animation = contex.weapon.GetDumpAnimationName();
            if (animation != string.Empty)
            {
                // 设置角色姿态 为 站立
                animatorHandler.animator.SetInteger(DTSAnimation.POSE_PARAM, DTSAnimation.ANIMATION_STATE_STAND);

                // 播放使用 该武器类型 的 跳跃 动画
                animatorHandler.PlayAnimation(animation, 0);
                gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.up * force);

                preState = contex.preState;

                dumpTime = Time.time * 1000;
                MemeryCacheMgr.Instance.Set(gameObject.name.ToString() + "#" + DTSKeys.DROP_DOWN, false);
            }
        }

        public override SoldierStateBase NextState(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            try
            {
                bool inTheFloor = (bool) MemeryCacheMgr.Instance.Get(gameObject.name.ToString() + "#" + DTSKeys.DROP_DOWN);

                if (inTheFloor && Time.time * 1000 - dumpTime > 500)
                {
                    string animation = contex.weapon.GetDropDownAnimationName();
                    animatorHandler.PlayAnimation(animation, 0);
                    preState.Enter(gameObject, animatorHandler, contex);
                    return preState;
                }
                else
                {
                    return soldierStateJump;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("in the floor boolean parse fail. " + ex.ToString());
                return preState;
            }
        }
        
        public override MoveState GetState()
        {
            return MoveState.DUMP;
        }

    }
}
