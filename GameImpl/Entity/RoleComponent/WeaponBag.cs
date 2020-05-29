using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Base;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Controller;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Controller.Weapon;
using CWLEngine.GameImpl.Network;

namespace CWLEngine.GameImpl.Entity
{
    public enum WeaponBagPos
    {
        FIRST_WEAPON,
        SECOND_WEAPON,
        KNIFE_WEAPON,
    }

    public class WeaponBag
    {
        private WeaponBase[] weapons = new WeaponBase[3];
        private int nowWeaponIndex;

        public WeaponBag()
        {
            nowWeaponIndex = 0;
            SetDefaultWeaponBag();
            
        }



        public void ChangeNowUsedWeapon(WeaponBagPos pos)
        {
            ChangeNowUsedWeapon((int)pos);
        }

        public void ChangeNowUsedWeapon(int pos)
        {
            if (nowWeaponIndex == pos)
            {
                return;
            }

            if (weapons[(int)pos] != null)
            {
                weapons[nowWeaponIndex].ClearModel();
                weapons[nowWeaponIndex].BackupWeapon();
                nowWeaponIndex = (int)pos;
            }
        }

        public void SetDefaultWeaponBag()
        {
            SwapWeapon(WeaponBagPos.FIRST_WEAPON, new WeaponAksu());
            SwapWeapon(WeaponBagPos.SECOND_WEAPON, new WeaponDeserteagle());
            SwapWeapon(WeaponBagPos.KNIFE_WEAPON, new WeaponKnife());
            ChangeNowUsedWeapon(nowWeaponIndex);
        }

        public WeaponBase GetNowWeapon()
        {
            return weapons[nowWeaponIndex];
        }

        public int GetNowWeaponIndex()
        {
            return nowWeaponIndex;
        }

        public void SwapWeapon(int weaponType, WeaponBase weapon)
        {
            if (weapons[(int)weaponType] != null)
            {
                weapons[(int)weaponType].Destory();
            }
            weapons[(int)weaponType] = weapon;
        }

        public void SwapWeapon(WeaponBagPos weaponType, WeaponBase weapon)
        {
            SwapWeapon((int)weaponType, weapon);
        }

        public bool NeedLeftIKPositon()
        {
            return weapons[nowWeaponIndex].NeedLeftIKPositon();
        }

        public bool NeedRightIKPosition()
        {
            return weapons[nowWeaponIndex].NeedRightIKPositon();
        }

        public void Destory()
        {
            for (int i = 0; i < 3; i++ )
            {
                if (weapons[i] != null)
                {
                    weapons[i].Destory();
                }
            }
        }
        
    }
}
