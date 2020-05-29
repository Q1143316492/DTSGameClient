using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Entity;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Network;

namespace CWLEngine.GameImpl.Controller
{
    public class HPBoxTrigger : MonoBehaviour
    {
        public float rotSpeed = 1f;

        void OnTriggerEnter(Collider collider)
        {
            if (collider.name == "PlayerA")
            {
                try
                {
                    int userID = (int)MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);
                    GameMgrRouter.AddHpRequestCall(30, userID);
                }
                catch (Exception) { }
                GameObject.Destroy(gameObject);   
            }
        }

        void Update()
        {
            Vector3 rot = gameObject.transform.eulerAngles;
            rot.y += rotSpeed;
            gameObject.transform.eulerAngles = rot;
        }
    }
}
