using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;

namespace CWLEngine.GameImpl.Entity
{
    public class FreezeController : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            int userID = 0;

            if (other.name.Equals("PlayerA"))
            {
                userID = (int)MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);
                EventMgr.Instance.EventTrigger(EventName.ENV_FREEZE_BEGIN, new FreezeMsg {
                    position = gameObject.transform.position,
                    radius = gameObject.transform.localScale.x,
                    userID = userID
                });
            }
            string[] ss = other.name.Split('#');

            try
            {
                userID = Convert.ToInt32(ss[1]);
                EventMgr.Instance.EventTrigger(EventName.ENV_FREEZE_BEGIN, new FreezeMsg
                {
                    position = gameObject.transform.position,
                    radius = gameObject.transform.localScale.x,
                    userID = userID
                });
            }
            catch (Exception) { }
        }
    }
}
