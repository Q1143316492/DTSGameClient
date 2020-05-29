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
    public class SoldierStateCrouch : SoldierStateBase
    {
        public override void Enter(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            string animation = contex.weapon.GetCrouchAnimationName();
            if (animation != string.Empty)
            {
                // 设置角色姿态 为 蹲下
                animatorHandler.animator.SetInteger(DTSAnimation.POSE_PARAM, DTSAnimation.ANIMATION_STATE_CROUCH);
                // 播放使用 该武器类型 的 站立动画
                animatorHandler.PlayAnimation(animation, 0);
                animatorHandler.PlayAnimation(animation, 1);
            }
        }

        public override SoldierStateBase NextState(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            //float h = (float)contex.Get(DTSAnimation.BLEND_TREE_2D_PARAM_NAME_1);
            //float v = (float)contex.Get(DTSAnimation.BLEND_TREE_2D_PARAM_NAME_2);

            //animatorHandler.animator.SetFloat(DTSAnimation.BLEND_TREE_2D_PARAM_NAME_1, h);
            //animatorHandler.animator.SetFloat(DTSAnimation.BLEND_TREE_2D_PARAM_NAME_2, v);
            

            if (contex.Check(EContexParam.END_CROUCH))
            {
                soldierStateWalk.Enter(gameObject, animatorHandler, contex);
                return soldierStateWalk;
            }

            if (contex.Check(EContexParam.DUMP) && dump.CheckAndRun())
            {
                contex.preState = soldierStateCrouch;
                soldierStateJump.Enter(gameObject, animatorHandler, contex);
                return soldierStateJump;
            }

            return soldierStateCrouch;
        }

        public override MoveState GetState()
        {
            return MoveState.CROUCH;
        }
        
    }
}
