using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CWLEngine.GameImpl.Base
{
    public class Skill
    {
        public string Name { get; }

        // 冷却时间
        private float cd;
        public float CD
        {
            get { return cd; }
            set { CD = Math.Max(0, value); }
        }

        // 锁定时间，表示施法后，在锁定时间内不能是否其他技能
        private float lockTime;
        public float LockTime
        {
            get { return lockTime; }
            set { LockTime = Math.Max(0, value); }
        }

        // 上一次施法时间
        private float lastUsed = 0;

        public Skill(string name, float cd)
        {
            Name = name;
            this.cd = cd;
            this.lockTime = cd;
        }

        public Skill(string name, float cd, float lockTime)
        {
            Name = name;
            this.cd = cd;
            this.lockTime = lockTime;
        }

        private float NowTick()
        {
            return Time.time * 1000; // ms
        }

        /*
         判断到上一次施法是否经过 cd ms。
         use 默认 true。表示checkCD调用时刷新施法。
        */
        public bool CheckAndRun()
        {
            float now = NowTick();
            if (lastUsed + cd <= now)
            {
                lastUsed = now;
                return true;
            }
            return false;
        }

        public void ClearCD()
        {
            lastUsed = 0;
        }
    }
}
