using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Conf;
using System.Reflection;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Network;

namespace CWLEngine.GameImpl.Controller.Weapon
{
    public class BulletLine : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (!other.name.Equals("PlayerA"))
            {
                Vector3 pos = gameObject.transform.position;

                int roomID = (int)MemeryCacheMgr.Instance.Get(DTSKeys.ROOM_ID);
                GameMgrRouter.AoeFreezeRequestCall(roomID, string.Format("{0:F};{1:F};{2:F}", pos.x, pos.y, pos.z));
                GameObject.Destroy(gameObject);
            }
        }
    }
}
