using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Conf;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Controller.Weapon
{
    public class WeaponKnife : WeaponBase
    {
        private Skill leftAttack = null;
        private Skill rightAttack = null;

        private int userID = -1;

        public WeaponKnife()
        {
            modelName = "weapon/weapon_knife";
            confFile = "knife";

            weaponConf = ConfMgr.Instance.GetConf<WeaponConf>(confFile);
            camera = GameObject.Find(GameObjectName.MainCameraName);

            if (weaponConf.hurt1 == 0)
            {
                weaponConf.hurt1 = 30;
            }

            for (int i = 0; i < weaponConf.skills.Count; i++)
            {
                string name = weaponConf.skills[i].name;
                float cd = weaponConf.skills[i].cd;
                if (weaponConf.skills[i].name == "left")
                {
                    leftAttack = new Skill(name, cd);
                }
                else if (weaponConf.skills[i].name == "right")
                {
                    rightAttack = new Skill(name, cd);
                }
            }
            
            BindUIType();
        }

        public override bool NeedLeftIKPositon()
        {
            return false;
        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.KNIFE;
        }

        public override void Enter(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            Transform weaponMount = Tools.FindChildrenTransform(gameObject, "Bip001 Spine");

            ObjectPoolMgr.Instance.LoadGameObject("model/" + modelName, (obj) =>
            {
                weaponModel = obj;

                obj.name = modelName;
                obj.transform.parent = weaponMount;

                obj.transform.localScale = weaponConf.localScale;
                obj.transform.localPosition = weaponConf.localPosition;
                obj.transform.localEulerAngles = weaponConf.localRotation;

                Transform leftHandIKTransform = Tools.FindChildrenTransform(gameObject, IKPosition.IKLeftHand);
                Transform rightHandIKTransform = Tools.FindChildrenTransform(gameObject, IKPosition.IKRightHand);

                if (leftHandIKTransform) leftHandIKTransform.localPosition = weaponConf.leftHandIKPositon;
                if (rightHandIKTransform) rightHandIKTransform.localPosition = weaponConf.rightHandIKPositon;
            });
            animatorHandler.animator.SetInteger(DTSAnimation.WEAPON_PARAM, DTSAnimation.ANIMATION_STATE_WEAPON_KNIFE);
        }

        public override void Update(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            if (nextAllowUseSkillTime > Time.time * 1000)
            {
                return;
            }
            
            if (contex.Check(EContexParam.SHOOT) && leftAttack.CheckAndRun())
            {
                LeftAttack(animatorHandler);

                if (contex.characterType == Entity.CharacterType.PLAYER)
                {
                    EventMgr.Instance.EventTrigger(EventName.PLAYER_KNIFE_ATTACK, 30);
                }
                return;    
            }
            
            if (contex.Check(EContexParam.SHOOT_BURST) && rightAttack.CheckAndRun())
            {
                RightAttack(animatorHandler);

                if (contex.characterType == Entity.CharacterType.PLAYER)
                {
                    EventMgr.Instance.EventTrigger(EventName.PLAYER_KNIFE_ATTACK, 60);
                }
                return;
            }
        }

        public void LeftAttack(AnimatorHandler animatorHandler)
        {
            animatorHandler.animator.SetTrigger(DTSAnimation.SHOOT);
            nextAllowUseSkillTime = Time.time * 1000 + leftAttack.LockTime;
        }

        public void RightAttack(AnimatorHandler animatorHandler)
        {
            animatorHandler.animator.SetTrigger(DTSAnimation.SHOOT_BURST);
            nextAllowUseSkillTime = Time.time * 1000 + rightAttack.LockTime;
        }
        
        public override string GetStandAnimationName()
        {
            return DTSAnimation.KNIFE_STAND;
        }

        public override string GetCrouchAnimationName()
        {
            return DTSAnimation.KNIFE_CROUCH;
        }

        public override string GetDumpAnimationName()
        {
            return DTSAnimation.KNIFE_DRUMP_1;
        }

        public override string GetDropDownAnimationName()
        {
            return DTSAnimation.KNIFE_DRUMP_1;
        }
    }
}
