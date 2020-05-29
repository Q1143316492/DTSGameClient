using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using CWLEngine.Core.Base;
using CWLEngine.GameImpl.Controller.SoldierState;
using CWLEngine.GameImpl.Controller.Weapon;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Network;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Util;
using CWLEngine.Core.Manager;

namespace CWLEngine.GameImpl.Entity
{
    public class RobotCreator : MonoBehaviour
    {
        void Start()
        {

        }

        public void CreateRobot(int robotKey)
        {
            MemeryCacheMgr.Instance.Set(DTSKeys.ROBOT_BORN + "#" + robotKey.ToString(), gameObject);
            EventMgr.Instance.EventTrigger(EventName.FIGHT_ROBOT_CONTROLLER, robotKey);
        }
    }
}
