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
    public class SoldierStateRun : SoldierStateBase
    {
        public override void Enter(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            string animation = contex.weapon.GetStandAnimationName();
            if (animation != string.Empty)
            {
                animatorHandler.animator.SetInteger(DTSAnimation.POSE_PARAM, DTSAnimation.ANIMATION_STATE_STAND);
                animatorHandler.PlayAnimation(animation, 0);
                animatorHandler.PlayAnimation(animation, 1);
            }
        }

        public override SoldierStateBase NextState(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            if (contex.Check(EContexParam.END_RUN))
            {
                soldierStateWalk.Enter(gameObject, animatorHandler, contex);
                return soldierStateWalk;
            }

            if (contex.Check(EContexParam.DUMP) && dump.CheckAndRun())
            {
                contex.preState = soldierStateRun;
                soldierStateJump.Enter(gameObject, animatorHandler, contex);
                return soldierStateJump;
            }
            return soldierStateRun;
        }

        public override MoveState GetState()
        {
            return MoveState.RUN;
        }
        
    }
}
