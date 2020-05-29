using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Network;

namespace CWLEngine.GameImpl.Controller.Weapon
{
    // 控制武器射击，后坐力，射线等
    public class WeaponFire
    {
        private float moveUp = 0;               // 后坐力向上偏移量
        private float moveLeft = 0;             // 后坐力向左偏移量
        private float moveRight = 0;            // 后坐力向右偏移量

        private float horizontalOffset = 0;     // 枪口 水平 实际偏移
        private float verticalOffset = 0;       // 枪口 垂直 实际偏移

        private float horizontalRevert = 0;     // 后坐力 水平 恢复速度
        private float verticalRevert = 0;       // 后坐力 垂直 回复速度
        
        private GameObject camera;                      // 后坐力影响的是主摄像机晃动
        private TPSCameraController cameraController;

        private float hurtIconDeleteTime = 0;   // 击中受伤效果 下一次失效时间 ms
        private float hurtEffectTime = 100;     // 击中受伤效果持续时间 ms

        private float centerStartExpandTime = 0;    // 准信变大 恢复的时间 ms

        private int hpCut;
        private GameObject weapon = null;

        CharacterType belongSoldierType = CharacterType.UNDEFINE;

        public WeaponFire(GameObject camera, 
            CharacterType belongSoldierType,
            float moveUp, 
            float moveLeft, 
            float moveRight,
            float horizontalRevert,
            float verticalRevert,
            int hpCut)
        {
            this.camera = camera;
            this.moveUp = moveUp;
            this.moveLeft = moveLeft;
            this.moveRight = moveRight;
            this.horizontalRevert = horizontalRevert;
            this.verticalRevert = verticalRevert;
            this.hpCut = hpCut;

            this.belongSoldierType = belongSoldierType;

            if (this.camera == null)
            {
                this.camera = GameObject.Find(GameObjectName.MainCameraName);
            }

            cameraController = this.camera.GetComponent<TPSCameraController>();

            MonoMgr.Instance.AddUpdateEvent(Update);
        }

        public void SetWeapon(GameObject weapon)
        {
            this.weapon = weapon;
        }

        public void Destory()
        {
            MonoMgr.Instance.DelUpdateEvent(Update);
        }

        void Update()
        {
            // 后坐力垂直回复
            if (horizontalOffset > 0)
            {
                horizontalOffset -= verticalRevert;
                cameraController.MoveDown(verticalRevert);
                if (horizontalOffset < 0) horizontalOffset = 0;
            }
        }

        /**
         1，开火会有弹痕
         2, 射击弹开的粒子效果
             */
        public void CreateFireEffect(Vector3 point, Vector3 normal)
        {
            ObjectPoolMgr.Instance.LoadGameObject("model/bullet/dankong", (obj) =>
            {
                obj.transform.position = point;
                obj.transform.eulerAngles = normal * 90;
                obj.transform.localScale *= UnityEngine.Random.Range(0.9f, 1.1f);
                obj.GetComponent<DelayRemove>().StartDelayRemove();
            });
        }

        public void CreateBloodEffect(Vector3 point)
        {

        }

        public void PlayerTick()
        {
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                string[] splitName = hit.collider.name.Split('#');

                // TODO 开火烟雾效果
                if (weapon != null)
                {
                    //ResourceMgr.Instance.LoadAsync<GameObject>("particle_system/fire_fog", (obj) =>
                    //{
                    //    obj.transform.position = weapon.transform.position;

                    //    ps.Play();
                    //});
                }
                
                if (splitName.Length == 2 && splitName[0] == "PlayerA")
                {
                    Debug.Log("hit palyer: " + splitName[1]);

                    // 首先会触发一个红叉，表示击中了
                    MvvmMgr.Instance.Set(UICacheKeys.HURT_ICON_SHOW, true);
                    hurtIconDeleteTime = Time.time * 1000 + hurtEffectTime;
                    MonoMgr.Instance.StartDelayEvent(hurtEffectTime, () =>
                    {
                        if (Time.time * 1000 > hurtIconDeleteTime)
                        {
                            MvvmMgr.Instance.Set(UICacheKeys.HURT_ICON_SHOW, false);
                        }
                    });

                    try
                    {
                        int userID = Convert.ToInt32(splitName[1]);
                        int thisUserID = (int) MemeryCacheMgr.Instance.Get(DTSKeys.USER_ID);

                        EventMgr.Instance.EventTrigger(EventName.PLAYER_ATTACKED, new RoleHurtEvent(thisUserID, userID, this.hpCut));
                    }
                    catch (Exception) { }
                }
                else
                {
                    CreateFireEffect(hit.point, hit.normal);
                }
            }

            // 射击时准星会变大
            MvvmMgr.Instance.Set(UICacheKeys.CENTER_STAR_ICON_EXPAND, 1.5F);
            centerStartExpandTime = Time.time * 1000 + hurtEffectTime;
            MonoMgr.Instance.StartDelayEvent(hurtEffectTime, () =>
            {
                if (Time.time * 1000 > centerStartExpandTime)
                {
                    MvvmMgr.Instance.Set(UICacheKeys.CENTER_STAR_ICON_EXPAND, 1.0F);
                }
            });

            horizontalOffset += moveUp;
            cameraController.MoveUp(moveUp);
        }

        public void ComputerTick()
        {
        
        }

        public void Tick(CharacterType characterType)
        {
            if (characterType == CharacterType.PLAYER)
            {
                PlayerTick();
            }
            if (characterType == CharacterType.COMPUTER)
            {
                ComputerTick();
            }
        }
    }
}
