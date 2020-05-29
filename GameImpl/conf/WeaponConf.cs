using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Conf
{
    [Serializable]
    public class SkillConf
    {
        public string name;
        public float cd;

        public SkillConf(string name, float cd)
        {
            this.name = name;
            this.cd = cd;
        }
    }

    [Serializable]
    public class WeaponConf
    {
        public string name;

        public Vector3 localScale;
        public Vector3 localPosition;
        public Vector3 localRotation;

        public Vector3 leftHandIKPositon;
        public Vector3 rightHandIKPositon;

        public int BulletCountFirst;
        public int BulletCountSecond;
        public int BulletCountFirstFull;

        public List<SkillConf> skills = new List<SkillConf>();

        public float recoilUp;
        public float recoilLeft;
        public float recoilRight;

        public float horizontalRevert;  // 水平
        public float verticalRevert;    // 垂直

        public int hurt1;

        public static WeaponConf Creator()
        {
            WeaponConf weaponConf = new WeaponConf()
            {
                name = "ak47"    
            };
            weaponConf.skills.Add(new SkillConf("skill 1", 100));
            weaponConf.skills.Add(new SkillConf("skill 2", 100));
            
            return weaponConf;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}