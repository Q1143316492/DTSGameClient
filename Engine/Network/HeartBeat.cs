using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Network;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CWLEngine.Core.Network
{
    public class HeartBeat : Singleton<HeartBeat>
    {
        private Skill heartBeat = new Skill("heart_beat", NetworkFrequency.HEART_BEAT * 1000);  // 这里单位是 ms
        private bool start = false;

        public HeartBeat()
        {
            NetworkMgr.Instance.AddMsgListener(ServiceID.SYNCHRONIZATION_HEART_BEAT_SERVICE, UserSynchronizationRouter.HeartBeatRequestCallback);
            MonoMgr.Instance.AddUpdateEvent(Update);
        }

        public void Start()
        {
            start = true;
        }

        public void Stop()
        {
            start = false;
        }

        void Update()
        {
            if (start && heartBeat.CheckAndRun())
            {
                int user_id = 0;
                try
                {
                    user_id = (int) MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);
                }
                catch (Exception ex)
                {
                    Debug.Log("userID is not exist. err: " + ex.ToString());
                    user_id = 0;
                    return;
                }
                UserSynchronizationRouter.HeartBeatRequestCall(user_id, 1, NetworkFrequency.HEART_BEAT);
            }
        }
    }
}
