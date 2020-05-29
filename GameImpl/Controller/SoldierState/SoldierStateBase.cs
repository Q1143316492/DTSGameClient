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
    public enum MoveState
    {
        WALK,
        RUN,
        CROUCH,
        DUMP,
        ROTATE,
    }

    public abstract class SoldierStateBase
    {
        public static SoldierStateWalk soldierStateWalk = new SoldierStateWalk();
        public static SoldierStateRun soldierStateRun = new SoldierStateRun();
        public static SoldierStateCrouch soldierStateCrouch = new SoldierStateCrouch();
        public static SoldierStateJump soldierStateJump = new SoldierStateJump();
        public static GameObject inputHook = null;

        protected static Skill dump = new Skill("dump", 500);

        // 进入一个姿势的时候要做写什么
        public abstract void Enter(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex);

        // 计算下一个状态，可能用户输入驱动，也可能是AI计算的结果
        public abstract SoldierStateBase NextState(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex);

        public abstract MoveState GetState();

    }
}
