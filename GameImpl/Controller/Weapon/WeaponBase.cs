using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Conf;
using System.Reflection;
using CWLEngine.GameImpl.Entity;

namespace CWLEngine.GameImpl.Controller.Weapon
{
    public abstract class WeaponBase
    {
        protected string modelName = "";              // 每一个武器 都有模型加载路径 
        protected string confFile = "";               // 每一个武器 都有配置文件加载后在内存中的表示 configMgr 
        protected WeaponConf weaponConf = null;       // 配置表加载后 反序列化的类，包含后坐力子弹数量等数据

        protected UIType<string> weaponName = null;

        public UIType<int> BulletCountFirst = null;   // 子弹数量，主弹夹
        public UIType<int> BulletCountSecond = null;  // 子弹数量，备用弹夹 

        protected int BulletCountFirstBack = -1;
        protected int BulletCountSecondBack = -1;

        protected CharacterType belongSoldierType = CharacterType.UNDEFINE;

        protected float nextAllowUseSkillTime = 0;    // 单位毫秒

        public GameObject weaponModel = null;
        protected GameObject camera = null;           // 用来做倍镜效果的摄像机，暂未实现
        protected WeaponFire weaponFire;
        protected int hpCut1 = 30;                    // 枪的默认伤害, 如果配置文件没写的话
        
        public abstract void Enter(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex);
        public abstract void Update(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex);

        public void Init(CharacterType characterType)
        {
            belongSoldierType = characterType;
            BindUIType();
        }

        public Vector3 GetModelPositon()
        {
            return weaponModel.transform.position;
        }

        public int GetHurt()
        {
            return hpCut1 / 2 + 1;
        }

        public void ClearModel()
        {
            if (weaponModel != null)
            {
                GameObject.Destroy(weaponModel);
            }
        }

        public void BackupWeapon()
        {
            BulletCountFirstBack = BulletCountFirst.val;
            BulletCountSecondBack = BulletCountSecond.val;
        }

        public void RecoverBackup()
        {
            if (BulletCountFirstBack > 0)
            {
                BulletCountFirst.val = BulletCountFirstBack;
            }
            if (BulletCountSecondBack > 0)
            {
                BulletCountSecond.val = BulletCountSecondBack;
            }
        }

        protected void BindUIType()
        {
            if (belongSoldierType == CharacterType.PLAYER)
            {
                weaponName = new UIType<string>(UICacheKeys.WEAPON_NAME, weaponConf.name);
                BulletCountFirst = new UIType<int>(UICacheKeys.BULLET_COUNT_FIRST, weaponConf.BulletCountFirst);
                BulletCountSecond = new UIType<int>(UICacheKeys.BULLET_COUNT_SECOND, weaponConf.BulletCountSecond);
            }
            else
            {
                weaponName = new UIType<string>(UICacheKeys.NO_BIND, weaponConf.name);
                BulletCountFirst = new UIType<int>(UICacheKeys.NO_BIND, weaponConf.BulletCountFirst);
                BulletCountSecond = new UIType<int>(UICacheKeys.NO_BIND, weaponConf.BulletCountSecond);
            }
        }

        public void Enable()
        {
            if (weaponModel == null) return;
            weaponModel.SetActive(true);
        }

        public void DisAble()
        {
            if (weaponModel == null) return;
            weaponModel.SetActive(false);
        }

        public virtual bool NeedLeftIKPositon()
        {
            return true;
        }

        public virtual bool NeedRightIKPositon()
        {
            return true;
        }

        public abstract string GetStandAnimationName();
        public abstract string GetCrouchAnimationName();
        public abstract string GetDumpAnimationName();
        public abstract string GetDropDownAnimationName();
        public abstract WeaponType GetWeaponType();

        public static WeaponBase ReflectionCreator(string typeName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.CreateInstance(typeName) as WeaponBase;
        }

        public void Destory()
        {
            if (weaponFire != null)
            {
                weaponFire.Destory();
            }
            if (weaponModel != null)
            {
                GameObject.Destroy(weaponModel);
            }
        }
    }
}
