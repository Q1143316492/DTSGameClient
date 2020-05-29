using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Entity
{
    public class RobotAttackEvent
    {
        public RobotAttackEvent(int robotID, GameObject target)
        {
            this.robotID = robotID;
            this.target = target;
        }
        
        public int robotID;
        public GameObject target;
    }
}
