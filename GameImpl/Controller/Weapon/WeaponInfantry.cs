﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Conf;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Util;
using UnityEngine;

namespace CWLEngine.GameImpl.Controller.Weapon
{
    public abstract class WeaponInfantry : WeaponBase
    {

        public int BulletCountFirstFull = 30;

        // 单发，三发
        private Skill shoot;
        private Skill shootBrush;
        private Skill shootLine;
        private Skill reload;

        protected string shootSound = "fire";

        public WeaponInfantry(string modelName, string confFile)
        {
            this.modelName = modelName;
            this.confFile = confFile;

            weaponConf = ConfMgr.Instance.GetConf<WeaponConf>(confFile);
            camera = GameObject.Find(GameObjectName.MainCameraName);

            if (weaponConf.hurt1 == 0)
            {
                weaponConf.hurt1 = 30;
            }

            this.hpCut1 = weaponConf.hurt1;

            for (int i = 0; i < weaponConf.skills.Count; i++)
            {
                string name = weaponConf.skills[i].name;
                float cd = weaponConf.skills[i].cd;
                if (weaponConf.skills[i].name == "shoot")
                {
                    shoot = new Skill(name, cd);
                }
                else if (weaponConf.skills[i].name == "shoot_brush")
                {
                    shootBrush = new Skill(name, cd);
                }
                else if (weaponConf.skills[i].name == "reload")
                {
                    reload = new Skill(name, cd);
                }
            }

            // TODO 把这个技能CD 写进配置
            shootLine = new Skill("shootLine", 1000);
            
            BulletCountFirstFull = weaponConf.BulletCountFirstFull;

            if (camera == null)
            {
                camera = GameObject.Find("Main Camera");
            }
        }

        public void SetBulletCount(int first, int second)
        {
            BulletCountFirst.val = first;
            BulletCountSecond.val = second;
        }

        public override void Enter(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            Transform weaponMount = Tools.FindChildrenTransform(gameObject, "Bip001 Spine");

            ObjectPoolMgr.Instance.LoadGameObject("model/" + modelName, (obj) =>
            {
                weaponModel = obj;
                RecoverBackup();
                weaponFire = new WeaponFire(camera,
                    belongSoldierType,
                    weaponConf.recoilUp,
                    weaponConf.recoilLeft,
                    weaponConf.recoilRight,
                    weaponConf.horizontalRevert,
                    weaponConf.verticalRevert,
                    weaponConf.hurt1
                );
                weaponFire.SetWeapon(weaponModel);

                obj.name = modelName;
                obj.transform.parent = weaponMount;

                obj.transform.localScale = weaponConf.localScale;
                obj.transform.localPosition = weaponConf.localPosition;
                obj.transform.localEulerAngles = weaponConf.localRotation;

                Transform leftHandIKTransform = Tools.FindChildrenTransform(gameObject, IKPosition.IKLeftHand);
                Transform rightHandIKTransform = Tools.FindChildrenTransform(gameObject, IKPosition.IKRightHand);

                leftHandIKTransform.localPosition = weaponConf.leftHandIKPositon;
                rightHandIKTransform.localPosition = weaponConf.rightHandIKPositon;
            });

            animatorHandler.animator.SetInteger(DTSAnimation.WEAPON_PARAM, DTSAnimation.ANIMATION_STATE_WEAPON_INFANTRY);
        }

        public override void Update(GameObject gameObject, AnimatorHandler animatorHandler, StateContex contex)
        {
            if (nextAllowUseSkillTime > Time.time * 1000)
            {
                return;
            }

            // 换子弹
            if (contex.Check(EContexParam.RELOAD) && reload.CheckAndRun())
            {
                nextAllowUseSkillTime = Time.time * 1000 + reload.LockTime;
                ReloadBullet(animatorHandler);
                return;
            }

            // 单发射击
            if (contex.Check(EContexParam.SHOOT))
            {
                if (BulletCountFirst.val >= 1 && shoot.CheckAndRun())
                {
                    nextAllowUseSkillTime = Time.time * 1000 + shoot.LockTime;
                    Fire(animatorHandler, contex.characterType);
                    return;
                }
            }

            // 三发射击
            if (contex.Check(EContexParam.SHOOT_BURST))
            {
                if (BulletCountFirst.val >= 3 && shootBrush.CheckAndRun())
                {
                    MonoMgr.Instance.StartCoroutine(MultiFire(3, 0.1f, animatorHandler, contex.characterType));
                    nextAllowUseSkillTime = Time.time * 1000 + shootBrush.LockTime;
                    return;
                }
            }

            if (contex.Check(EContexParam.SHOOT_LINE) && shootLine.CheckAndRun() && camera != null)
            {
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, Mathf.Infinity))
                {
                    EventMgr.Instance.EventTrigger(EventName.ATTACK_YUN, new SkillAttackYunBullet
                    {
                        position = weaponModel.transform.position,
                        direct = hit.point - weaponModel.transform.position,
                        force = 100,
                    });
                }
            }

        }

        public void ReloadBullet(AnimatorHandler animatorHandler)
        {
            if (BulletCountSecond.val == 0)
            {
                MusicMgr.Instance.PlaySound("kake");
                nextAllowUseSkillTime = Time.time * 1000;
                reload.ClearCD();
                return;
            }

            if (BulletCountFirstFull == BulletCountFirst.val)
            {
                return;
            }

            MusicMgr.Instance.PlaySound("reload");
            animatorHandler.PlayAnimation(DTSAnimation.INFANTRY_RELOAD, 1);

            MonoMgr.Instance.StartDelayEvent(reload.LockTime, () => {
                int bulletCount = BulletCountFirstFull - BulletCountFirst.val;
                bulletCount = Math.Min(bulletCount, BulletCountSecond.val);

                BulletCountFirst.val += bulletCount;
                BulletCountSecond.val -= bulletCount;
            });
        }

        public void Fire(AnimatorHandler animatorHandler, CharacterType characterType, bool enterAnimation = true)
        {
            if (BulletCountFirst.val > 0)
            {
                BulletCountFirst.val -= 1;
                MusicMgr.Instance.PlaySound(shootSound);
                weaponFire.Tick(characterType);

                if (enterAnimation)
                {
                    animatorHandler.animator.SetTrigger(DTSAnimation.SHOOT);
                }
            }
            else
            {
                //MusicMgr.Instance.PlaySound("kake");
            }
        }

        // 射击 times 次，每次开火间隔 delay
        private IEnumerator MultiFire(int times, float delay, AnimatorHandler animatorHandler, CharacterType characterType)
        {
            float lastTime = Time.time;

            Fire(animatorHandler, characterType, false);
            animatorHandler.animator.SetTrigger(DTSAnimation.SHOOT_BURST);

            times--;
            if (times > 0)
            {
                while (true)
                {
                    float nowTime = Time.time;

                    if (nowTime - lastTime > delay)
                    {
                        lastTime = nowTime;
                        times--;
                        Fire(animatorHandler, characterType, false);

                        if (times == 0)
                        {
                            break;
                        }
                    }
                    yield return null;
                }
            }
        }

        public override string GetStandAnimationName()
        {
            return DTSAnimation.INFANTRY_STAND;
        }

        public override string GetCrouchAnimationName()
        {
            return DTSAnimation.INFANTRY_CROUCH;
        }

        public override string GetDumpAnimationName()
        {
            return DTSAnimation.INFANTRY_DRUMP_1;
        }
        
        public override string GetDropDownAnimationName()
        {
            return DTSAnimation.INFANTRY_DRUMP_3;
        }
    }
}
