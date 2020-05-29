using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Util;
using System.Collections;
using CWLEngine.GameImpl.Conf;
using CWLEngine.GameImpl.Entity;

namespace CWLEngine.GameImpl.Controller.Weapon
{
    // 步枪
    public class WeaponAK47 : WeaponInfantry
    {
        public WeaponAK47()
            :base("weapon/weapon_ak47", "ak47")
        {
            this.shootSound = "ak47";
        }
        
        public override WeaponType GetWeaponType()
        {
            return WeaponType.AK47;
        }
    }

    public class WeaponAksu : WeaponInfantry
    {
        public WeaponAksu()
            : base("weapon/weapon_aksu", "aksu")
        {

        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.AKSU;
        }
    }

    public class WeaponFal : WeaponInfantry
    {
        public WeaponFal()
            : base("weapon/weapon_fal", "fal")
        {

        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.FAL;
        }
    }

    public class WeaponG36 : WeaponInfantry
    {
        public WeaponG36()
            : base("weapon/weapon_g36", "g36")
        {

        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.G36;
        }
    }
    
    public class WeaponMac10 : WeaponInfantry
    {
        public WeaponMac10()
            : base("weapon/weapon_mac10", "mac10")
        {
        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.MAC10;
        }
    }

    public class WeaponMp5k : WeaponInfantry
    {
        public WeaponMp5k()
            : base("weapon/weapon_mp5k", "mp5k")
        {

        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.MP5K;
        }
    }

    public class WeaponUzi : WeaponInfantry
    {
        public WeaponUzi()
            : base("weapon/weapon_uzi", "uzi")
        {
        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.UZI;
        }
    }

    // 手枪

    public class WeaponDeserteagle : WeaponHandgun
    {
        public WeaponDeserteagle()
            : base("weapon/weapon_deserteagle", "deserteagle")
        {
        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.DESERTEAGLE;
        }
    }

    // 重武器
    public class WeaponPKM : WeaponHeavy
    {
        public WeaponPKM()
            : base("weapon/weapon_pkm", "pkm")
        {

        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.PKM;
        }
    }

}
