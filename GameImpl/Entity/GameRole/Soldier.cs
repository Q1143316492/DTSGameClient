using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using CWLEngine.Core.Base;
using CWLEngine.GameImpl.Controller.SoldierState;
using CWLEngine.GameImpl.Controller.Weapon;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Network;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Util;
using CWLEngine.Core.Manager;

namespace CWLEngine.GameImpl.Entity
{
    public class Soldier : CharacterBase
    {
        public GameObject _this = null;

        public AnimatorHandler animatorHandler;

        public SoldierStateBase soldierState;

        public WeaponBag bag;
        //public WeaponBase weapon;

        public RoleState roleState;

        private StateContex contex = new StateContex();
        private StateContex renderContex = new StateContex();

        private float renderMoveH = 0;
        private float renderMoveV = 0;
        private float viewMoveX = 0;
        private float viewMoveY = 0;
        private bool isAlive = true;
        private bool isFreeze = false;

        // 分别是 走，跑，蹲, 跳，角色左右视角旋转的速度
        public float[] speeds = { 0.04f, 0.06f, 0.03f, 0.04f, 5.0f };
        
        public Soldier(GameObject soldier, 
            AnimatorHandler animatorHandler, 
            SoldierStateBase state,
            CharacterType charactType,
            int userID,
            int roomID)
        {
            this._this = soldier;
            this.animatorHandler = animatorHandler;
            this.charactType = charactType;
            this.userID = userID;
            this.roomID = roomID;

            isAlive = true;

            bag = new WeaponBag();

            SetWeaponAndState(bag.GetNowWeapon(), bag.GetNowWeaponIndex(), state);

            roleState = new RoleState(false);

            EventMgr.Instance.AddEventListener(EventName.PLAYER_CHANGE_WEAPON + "#" + userID.ToString(), ChangeWeaponCmd);
        }

        public void ChangeWeaponCmd(WeaponType weaponType)
        {
            // 武器按 主武器 副武器 近战武器 在背包中位置 1 2 3 weaponBagPos
            WeaponBagPos weaponBagPos = WeaponBoxBase.weaponBagPosList[(int)weaponType];

            // 先把当前武器换到那个位置
            Debug.Log(weaponBagPos);
            bag.ChangeNowUsedWeapon(weaponBagPos);
            FreshWeapon(weaponBagPos);
            Debug.Log(weaponType);
            // 判断下是不是新获得的武器，是的话还要新创建一把
            if (bag.GetNowWeapon().GetWeaponType() != weaponType)
            {
                bag.SwapWeapon(weaponBagPos, WeaponBase.ReflectionCreator(WeaponBoxBase.weaponTypeDict[weaponType]));
                FreshWeapon(weaponBagPos);
            }
        }

        public void ChangeWeaponCmd(WeaponBagPos weaponBagPos)
        {
            bag.ChangeNowUsedWeapon(weaponBagPos);
            FreshWeapon(weaponBagPos);
        }

        void ChangeWeaponCmd(object obj)
        {
            if (obj is WeaponBagPos weaponBagPos)
            {
                ChangeWeaponCmd(weaponBagPos);
            }
        }


        public GameObject GetGameObject()
        {
            return _this;
        }

        public void Freeze()
        {
            isFreeze = true;
        }

        public void UnFreeze()
        {
            isFreeze = false;
        }

        public bool IsFreeze()
        {
            return isFreeze;
        }

        public void SyncAnimation()
        {
            animatorHandler.animator.SetInteger(DTSAnimation.WEAPON_PARAM, DTSAnimation.ANIMATION_STATE_WEAPON_HEAVY);
        }

        public override void SetHp(int hp)
        {
            if (roleState.GetHP() == 0)
            {
                return;
            }
            roleState.SetHP(hp);
        }

        public override float GetHP()
        {
            return roleState.GetHundredPercentHP();
        }

        public override void Hit(int hp)
        {
            roleState.Hit(hp);
        }

        public override void GoToDeath()
        {
            if (!Death())
            {
                return;
            }
            SetHp(0);
            isAlive = false;
            animatorHandler.animator.SetInteger(DTSAnimation.POSE_PARAM, 0);
            animatorHandler.animator.SetInteger(DTSAnimation.WEAPON_PARAM, 0);
            animatorHandler.PlayAnimation(DTSAnimation.DEATH_1, 0);
            animatorHandler.PlayAnimation(DTSAnimation.DEATH_1, 1);
            EventMgr.Instance.EventTrigger(EventName.PLAYER_DEATH, userID);
        }

        public override bool Death()
        {
            return roleState.Death();
        }

        public void SetSpeeds(float []speeds)
        {
            this.speeds = speeds;
        }

        public bool NeedLeftIKPositon()
        {
            return bag.NeedLeftIKPositon();
        }

        public bool NeedRightIKPosition()
        {
            return bag.NeedRightIKPosition();
        }

        public void FreshWeapon(int weaponBagPos)
        {
            if (weaponBagPos == 0)
            {
                FreshWeapon(WeaponBagPos.FIRST_WEAPON);
            }
            else if (weaponBagPos == 1)
            {
                FreshWeapon(WeaponBagPos.SECOND_WEAPON);
            }
            else if (weaponBagPos == 2)
            {
                FreshWeapon(WeaponBagPos.KNIFE_WEAPON);
            }
        }

        public void FreshWeapon(WeaponBagPos weaponBagPos)
        {
            SetWeapon(bag.GetNowWeapon(), weaponBagPos);
        }

        public void SetWeaponAndState(WeaponBase weapon, int pos, SoldierStateBase state)
        {
            SetWeapon(weapon, pos, charactType);
            SetState(state);
        }

        public override Transform GetTransform()
        {
            if (_this != null)
            {
                return _this.transform;
            }
            return null;
        }
        
        public override void SetTransform(Vector3 position, Vector3 rotation)
        {
            if (_this != null)
            {
                _this.transform.position = position;
                _this.transform.eulerAngles = rotation;
            }
        }

        public override void SetPosition(Vector3 position)
        {
            if (_this != null)
            {
                _this.transform.position = position;
            }
        }

        public override void SetRotation(Vector3 rotation)
        {
            if (_this != null)
            {
                _this.transform.eulerAngles = rotation;
            }
        }

        public void SetWeapon(WeaponBase weapon, int weaponBagPos)
        {
            SetWeapon(weapon, weaponBagPos, charactType);
        }

        public void SetWeapon(WeaponBase weapon, WeaponBagPos weaponBagPos)
        {
            SetWeapon(weapon, weaponBagPos, charactType);
        }

        public void SetWeapon(WeaponBase weapon, int weaponBagPos, CharacterType character)
        {
            bag.SwapWeapon(weaponBagPos, weapon);
            bag.ChangeNowUsedWeapon(weaponBagPos);
            contex.Clear();
            contex.weapon = bag.GetNowWeapon();
            contex.weapon.Init(character);
            contex.weapon.Enter(_this, animatorHandler, contex);
        }

        public void SetWeapon(WeaponBase weapon, WeaponBagPos weaponBagPos, CharacterType character)
        {
            SetWeapon(weapon, (int)weaponBagPos, character);
        }

        private void SetState(SoldierStateBase state)
        {
            soldierState = state;
            
            contex.Clear();
            contex.weapon = bag.GetNowWeapon();
            soldierState.Enter(_this, animatorHandler, contex);
        }

        public void MovePosition(Vector3 vecPos)
        {
            if (_this != null)
            {
                _this.GetComponent<Rigidbody>().MovePosition(vecPos);
            }
        }

        public void Update(StateContex contex)
        {
            contex.weapon = bag.GetNowWeapon();
            soldierState = soldierState.NextState(_this, animatorHandler, contex);

            contex.characterType = charactType;
            contex.weapon.Update(_this, animatorHandler, contex);
        }

        public override void UpdateOperation(PlayerAction playerAction)
        {
            renderContex.Clear();

            if (playerAction.Get(EPlayerAction.SHOOT) == 1)
            {
                renderContex.Set(EContexParam.SHOOT);
            }
            if (playerAction.Get(EPlayerAction.SHOOT_BURST) == 1)
            {
                renderContex.Set(EContexParam.SHOOT_BURST);
            }
            if (playerAction.Get(EPlayerAction.SHOOT_LINE) == 1)
            {
                renderContex.Set(EContexParam.SHOOT_LINE);
            }

            if (playerAction.Get(EPlayerAction.RELOAD) == 1)
            {
                renderContex.Set(EContexParam.RELOAD);
            }

            if (playerAction.Get(EPlayerAction.BEGIN_RUN) == 1)
            {
                renderContex.Set(EContexParam.BEGIN_RUN);
            }
            else if (playerAction.Get(EPlayerAction.END_RUN) == 1)
            {
                renderContex.Set(EContexParam.END_RUN);
            }

            if (playerAction.Get(EPlayerAction.BEGIN_CROUCH) == 1)
            {
                renderContex.Set(EContexParam.BEGIN_CROUCH);
            }
            else if (playerAction.Get(EPlayerAction.END_CROUCH) == 1)
            {
                renderContex.Set(EContexParam.END_CROUCH);
            }

            if (playerAction.Get(EPlayerAction.DUMP) == 1)
            {
                renderContex.Set(EContexParam.DUMP);
            }

            renderMoveH = playerAction.GetMoveH();
            renderMoveV = playerAction.GetMoveV();
            viewMoveX = playerAction.GetMouseX();
            viewMoveY = playerAction.GetMouseY();
        }

        public void RenderOperation()
        {
            if (!isAlive)
            {
                return;
            }

            UpdateMove(renderMoveH, renderMoveV);
            UpdateView(viewMoveX, viewMoveY);
            Update(renderContex);

            if (_this.transform.position.y < -20)
            {
                SetHp(0);
                EventMgr.Instance.EventTrigger(EventName.PLAYER_ATTACKED, new RoleHurtEvent(-1, userID, 1000));
            }
        }
        
        private void UpdateMove(float sourceH, float sourceV)
        {
            float h = sourceH;
            float v = sourceV;
            
            if (soldierState == SoldierStateBase.soldierStateRun)
            {
                animatorHandler.animator.SetFloat(DTSAnimation.BLEND_TREE_2D_PARAM_NAME_1, h * 2);
                animatorHandler.animator.SetFloat(DTSAnimation.BLEND_TREE_2D_PARAM_NAME_2, v * 2);
            }
            else if (soldierState != SoldierStateBase.soldierStateJump)
            {
                animatorHandler.animator.SetFloat(DTSAnimation.BLEND_TREE_2D_PARAM_NAME_1, h);
                animatorHandler.animator.SetFloat(DTSAnimation.BLEND_TREE_2D_PARAM_NAME_2, v);
            }

            if (charactType == CharacterType.PLAYER || charactType == CharacterType.COMPUTER)
            {
                h *= 100;
                v *= 100;
            
                // 影响 xz 平面位置
                float speed = speeds[(int)soldierState.GetState()];

                Vector3 forward = _this.transform.forward;
                Vector3 right = _this.transform.right;
                float radius = 0;

                try
                {
                    radius = (float) MemeryCacheMgr.Instance.Get("TPSCameraControllerRadius");
                }
                catch (Exception ex)
                {
                    Debug.Log("soldier update position. parse float fail. " + ex.ToString());
                    radius = 3f;
                }

                forward = Matrix4x4.Rotate(Quaternion.Euler(0, - Mathf.Atan2(1, radius) * 180 / Mathf.PI, 0)).MultiplyPoint(forward);
                right = Matrix4x4.Rotate(Quaternion.Euler(0, -Mathf.Atan2(1, radius) * 180 / Mathf.PI, 0)).MultiplyPoint(right);

                Vector3 vec = forward * v * speed * Time.deltaTime + right * h * speed * Time.deltaTime;
                _this.GetComponent<Rigidbody>().MovePosition(_this.transform.position + vec);
            }
        }

        private void UpdateView(float x, float y)
        {
            if (charactType == CharacterType.PLAYER || charactType == CharacterType.COMPUTER)
            {
                _this.transform.eulerAngles += new Vector3(0, x * speeds[(int)MoveState.ROTATE], 0);
            }
        }

        public void SyncTransform(string position, string rotation)
        {
            try
            {
                string[] posList = position.Split(';');
                string[] rotList = rotation.Split(';');

                Vector3 vecPos = new Vector3(
                    (float) Convert.ToDouble(posList[0]),
                    (float) Convert.ToDouble(posList[1]),
                    (float) Convert.ToDouble(posList[2]));

                Vector3 vecRot = new Vector3(
                    (float) Convert.ToDouble(rotList[0]),
                    (float) Convert.ToDouble(rotList[1]),
                    (float) Convert.ToDouble(rotList[2]));

                SetTransform(vecPos, vecRot);
            }
            catch (Exception ex)
            {
                Debug.Log("QueryUserTransform parse error. " + ex.ToString());
            }
        }

        public override void Destory()
        {
            if (_this != null)
            {
                GameObject.Destroy(_this);
            }
            EventMgr.Instance.DelEventListener(EventName.PLAYER_CHANGE_WEAPON + "#" + userID.ToString(), ChangeWeaponCmd);
            bag.Destory();
        }
    }
}
