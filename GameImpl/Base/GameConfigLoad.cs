using CWLEngine.Core.Manager;
using CWLEngine.Core.Base;
using CWLEngine.GameImpl.Conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWLEngine.GameImpl.Base
{
    public class GameConfigLoad : Singleton<GameConfigLoad>
    {
        public GameConfigLoad()
        {
            Load();
        }

        public void Load()
        {
            // 步枪
            ConfMgr.Instance.Load<WeaponConf>("ak47", "weapon/ak47.json");
            ConfMgr.Instance.Load<WeaponConf>("aksu", "weapon/aksu.json");
            ConfMgr.Instance.Load<WeaponConf>("fal", "weapon/fal.json");
            ConfMgr.Instance.Load<WeaponConf>("g36", "weapon/g36.json");
            ConfMgr.Instance.Load<WeaponConf>("mac10", "weapon/mac10.json");
            ConfMgr.Instance.Load<WeaponConf>("mp5k", "weapon/mp5k.json");
            ConfMgr.Instance.Load<WeaponConf>("uzi", "weapon/uzi.json");

            // 刀
            ConfMgr.Instance.Load<WeaponConf>("knife", "weapon/knife.json");

            // 手枪
            ConfMgr.Instance.Load<WeaponConf>("deserteagle", "weapon/deserteagle.json");

            // 机枪
            ConfMgr.Instance.Load<WeaponConf>("pkm", "weapon/pkm.json");
        }
    }
}
