using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Network;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Controller
{
    public class FightController
    {
        private int roomID;
        private int userID;

        public static int FreezeSkillCD = 2000; // ms
        public static int FreezeLimit = 1000;  // ms

        public FightController(int roomID, int userID)
        {
            this.roomID = roomID;
            this.userID = userID;
            Start();
        }

        void Start()
        {
            EventMgr.Instance.AddEventListener(EventName.ATTACK_YUN, SkillAttackYunFire);
            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_AOE_FREEZE_SERVICE, AoeFreezeCreate);
        }

        public void Destroy()
        {
            EventMgr.Instance.DelEventListener(EventName.ATTACK_YUN, SkillAttackYunFire);
        }

        void AoeFreezeCreate(Message msg)
        {
            GameMgrRouter.AoeFreezeResponse res = GameMgrRouter.AoeFreezeRequestCallback(msg);
            
            if (res.ret != 0)
            {
                return;
            }

            string[] ss = res.pos.Split(';');
            Vector3 pos = new Vector3();

            try
            {
                pos.x = (float)Convert.ToDouble(ss[0]);
                pos.y = (float)Convert.ToDouble(ss[1]);
                pos.z = (float)Convert.ToDouble(ss[2]);
            }
            catch (Exception ex)
            {
                Debug.Log("aoe freeze parse fail." + ex.ToString());
                return;
            }

            ResourceMgr.Instance.LoadAsync<GameObject>("model/env/Game/ball", (obj) =>
            {
                obj.transform.localScale = Vector3.one * 5;
                obj.transform.position = pos;
                obj.AddComponent<FreezeController>();
                MonoMgr.Instance.StartDelayEvent(3000, () =>
                {
                    GameObject.Destroy(obj);
                });
            });
        }

        void SkillAttackYunFire(object obj)
        {
            if (obj is SkillAttackYunBullet bulletParam)
            {
                ResourceMgr.Instance.LoadAsync<GameObject>("model/weapon/bullet", (bullet) =>
                {
                    bullet.transform.position = bulletParam.position;
                    bullet.GetComponent<Rigidbody>().AddForce(bulletParam.direct * bulletParam.force);
                });
            }
        }
    }
}
