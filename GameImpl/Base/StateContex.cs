using CWLEngine.GameImpl.Controller.SoldierState;
using CWLEngine.GameImpl.Controller.Weapon;
using CWLEngine.GameImpl.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWLEngine.GameImpl.Base
{
    public class StateContex
    {
        // 这部分是老的版本
        // 每次 dict 都 new 怕会造成很大GC压力，就慢慢把这部分丢掉
        //private Dictionary<string, object> dict = new Dictionary<string, object>();

        //public object Get(string key)
        //{
        //    if (dict.ContainsKey(key))
        //    {
        //        return dict[key];
        //    }
        //    return null;
        //}

        //public void Set(string key, object val)
        //{
        //    dict[key] = val;
        //}

        //public void Clear()
        //{
        //    dict.Clear();
        //}

        // 下面是新版本.................

        public SoldierStateBase preState = null;
        public WeaponBase weapon;
        public CharacterType characterType;

        private int[] contexes = new int[20];

        public void Clear()
        {
            weapon = null;
            preState = null;
            for (int i = 0; i < contexes.Length; i++ )
            {
                contexes[i] = -1;
            }
        }

        public void Set(EContexParam id)
        {
            contexes[ (int) id] = 1;
        }

        public bool Check(EContexParam id)
        {
            return contexes[ (int) id] == 1;
        }

        public string Get(EContexParam id)
        {
            return ContexParam.Params[ (int) id];
        }
    }
}
