using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Network;
using CWLEngine.GameImpl.Util;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CWLEngine.GameImpl.UI
{
    public class TestNetwork : PanelBase
    {
        private bool run = false;
        private Button btnStart = null;
        private Button btnStop = null;
        private Text msg = null;

        private static string TEST = "TEST_NETWORK";

        UIType<string> textContent = new UIType<string>(TEST, "");

        int val = 0;

        void Start()
        {
            MemeryCacheMgr.Instance.Set("ip", NetworkMacro.NETWORK_IP);
            MemeryCacheMgr.Instance.Set("port", NetworkMacro.NETWORK_PORT);
            NetworkMgr.Instance.CheckNetwork();

            btnStart = GetControl<Button>("begin");
            btnStop = GetControl<Button>("stop");
            msg = GetControl<Text>("msg");

            MvvmMgr.Instance.RegisterCallback(TEST, (obj) => {
                msg.text = obj as string;
            });

            //btnStart.onClick.AddListener(StartTest);
            //btnStop.onClick.AddListener(StopTest);
            run = true;
            NetworkMgr.Instance.AddMsgListener(ServiceID.USER_NETWORK_TEST_SERVICE, TestNetworkCallback);
        }

        void OnDestroy()
        {
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.USER_NETWORK_TEST_SERVICE, TestNetworkCallback);
        }

        long []timeCount = new long [205];

        void TestNetworkCallback(Message message)
        {
            UserRouter.NetworkTestResponse res = UserRouter.NetworkTestRequestCallback(message);
            if (res.ret == 0)
            {
                int nowTime = (int) (Time.time * 1000);
                int sumTime = nowTime - res.last_time;
                if (sumTime > 200)
                {
                    timeCount[200] += 1;
                }
                else
                {
                    timeCount[sumTime] += 1;
                }
            }
        }

        IEnumerator StartConn()
        {
            int nowTime = (int) (Time.time * 1000);
            UserRouter.NetworkTestRequestCall("123456789", nowTime);
            yield return null;
        }

        void StartTest()
        {
            run = true;
            //for (int i = 0; i < 3; i++ )
            //{
            //    StartCoroutine(StartConn());
            //}
        }


        private StringBuilder stringBuilder = new StringBuilder();

        void StopTest()
        {
            run = false;
            stringBuilder.Clear();
            for (int i = 0; i <= 200; i++ )
            {
                if (timeCount[i] > 0)
                {
                    stringBuilder.Append(string.Format("{0}:{1}|", i, timeCount[i]));
                }
            }
            Debug.Log(stringBuilder.ToString());
        }

        private Skill skill = new Skill("freq", 20);

        void Update()
        {
            if (Time.time > 10)
            {
                if (run)
                {
                    StopTest();
                }
                return;
            }
            if (run && skill.CheckAndRun())
            {
                StartCoroutine(StartConn());
            }
        }
    }
}
