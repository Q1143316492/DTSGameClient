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
    /**
        attackUserID 攻击了 hittedUserID ，打掉了 hurtHP 这么多血
        attackUserID 会在服务端计算击杀人数，所以坠落，毒圈伤害用 小于等于0 的值吧。 小于0的ID是机器人的
         * */
    public class RoleHurtEvent
    {
        public int attackUserID;
        public int hittedUserID;
        public int hurtHP;

        public RoleHurtEvent(int attackUserID, int hittedUserID, int hurtHP)
        {
            this.attackUserID = attackUserID;
            this.hittedUserID = hittedUserID;
            this.hurtHP = hurtHP;
        }
    }

    public class RoleState
    {
        public bool Unbeatable;
        private int HP = 100;
        private readonly int HP_MAX = 100;

        public RoleState(bool Unbeatable, int HP = 100)
        {
            this.Unbeatable = Unbeatable;   // 是否运行被伤害，标记
            this.HP = HP;
        }

        public void SetHP(int hp)
        {
            if (!Unbeatable)
            {
                hp = Math.Max(0, hp);
                HP = hp;
            }
        }

        public int GetHP()
        {
            return HP;
        }

        public float GetHundredPercentHP()
        {
            return (float)HP / HP_MAX;
        }

        public void Hit(int hp)
        {
            if (!Unbeatable)
            {
                hp = Math.Max(0, hp);
                HP -= Mathf.Min(HP, hp);
            }
        }

        public bool Death()
        {
            return !Unbeatable && HP == 0;
        }

    }
}
