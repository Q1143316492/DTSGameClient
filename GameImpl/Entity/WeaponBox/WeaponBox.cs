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

namespace CWLEngine.GameImpl.Entity
{
    public enum WeaponBoxType
    {
        EMPTY,
        BULLET_BOX,     // 子弹盒子
        WEAPON_BOX,     // 武器盒子
    }

    public enum WeaponType
    {
        AK47, 
        AKSU,
        DESERTEAGLE,
        FAL,
        G36,
        MAC10,
        MP5K,
        PKM,
        UZI,
        KNIFE,
    }

    public enum WeaponBoxLifeCycle
    {
        Live,
        Death
    }

    public class WeaponBoxMessage
    {
        public bool isLock = false;
        public bool isShow = false;
        public string message = "";
    }

    public abstract class WeaponBoxBase
    {
        protected WeaponBoxType boxType = WeaponBoxType.EMPTY;
        protected WeaponBoxLifeCycle lifeCycle = WeaponBoxLifeCycle.Death;
        protected WeaponType weaponType;

        public static List<string> weaponModelName = new List<string>()
        {
            "weapon_ak47",
            "weapon_aksu",
            "weapon_fal",
            "weapon_g36",
            "weapon_mac10",
            "weapon_mp5k",
            "weapon_uzi",
            "weapon_deserteagle",
            "weapon_pkm",
        };

        public static Dictionary<WeaponType, string> weaponTypeDict = new Dictionary<WeaponType, string>()
        {
            { WeaponType.AK47 ,"CWLEngine.GameImpl.Controller.Weapon.WeaponAK47"},
            { WeaponType.AKSU ,"CWLEngine.GameImpl.Controller.Weapon.WeaponAksu"},
            { WeaponType.DESERTEAGLE ,"CWLEngine.GameImpl.Controller.Weapon.WeaponDeserteagle"},
            { WeaponType.FAL ,"CWLEngine.GameImpl.Controller.Weapon.WeaponFal"},
            { WeaponType.G36 ,"CWLEngine.GameImpl.Controller.Weapon.WeaponG36"},
            { WeaponType.MAC10 ,"CWLEngine.GameImpl.Controller.Weapon.WeaponMac10"},
            { WeaponType.MP5K ,"CWLEngine.GameImpl.Controller.Weapon.WeaponMp5k"},
            { WeaponType.PKM ,"CWLEngine.GameImpl.Controller.Weapon.WeaponPKM"},
            { WeaponType.UZI ,"CWLEngine.GameImpl.Controller.Weapon.WeaponUzi"},
            { WeaponType.KNIFE ,"CWLEngine.GameImpl.Controller.Weapon.WeaponKnife"},
        };

        public static Dictionary<WeaponType, string> weaponNameDict = new Dictionary<WeaponType, string>()
        {
            { WeaponType.AK47 ,"步枪 AK47"},
            { WeaponType.AKSU ,"步枪 Aksu"},
            { WeaponType.DESERTEAGLE ,"手枪 Deserteagle"},
            { WeaponType.FAL ,"步枪 Fal"},
            { WeaponType.G36 ,"步枪 G36"},
            { WeaponType.MAC10 ,"冲锋枪 Mac10"},
            { WeaponType.MP5K ,"冲锋枪 Mp5k"},
            { WeaponType.PKM ,"机枪 PKM"},
            { WeaponType.UZI ,"冲锋枪 Uzi"},
            { WeaponType.KNIFE ,"匕首"},
        };

        public static List<WeaponType> IntToWeaponType = new List<WeaponType>()
        {
            WeaponType.AK47,
            WeaponType.AKSU,
            WeaponType.DESERTEAGLE,
            WeaponType.FAL,
            WeaponType.G36,
            WeaponType.MAC10,
            WeaponType.MP5K,
            WeaponType.PKM,
            WeaponType.UZI,
            WeaponType.KNIFE,
        };

        public static List<WeaponBagPos> weaponBagPosList = new List<WeaponBagPos>()
        {
            WeaponBagPos.FIRST_WEAPON,
            WeaponBagPos.FIRST_WEAPON,
            WeaponBagPos.SECOND_WEAPON,
            WeaponBagPos.FIRST_WEAPON,
            WeaponBagPos.FIRST_WEAPON,
            WeaponBagPos.FIRST_WEAPON,
            WeaponBagPos.FIRST_WEAPON,
            WeaponBagPos.FIRST_WEAPON,
            WeaponBagPos.FIRST_WEAPON,
            WeaponBagPos.KNIFE_WEAPON,
        };

        protected string modelPath = "";
        protected string warnMsg = "";

        protected WarnPanel panel = null;
        protected GameObject box;

        protected bool autoRefresh;
        protected float autoRefreshTime;

        protected UIType<string> warnText = new UIType<string>(UICacheKeys.BULLET_BOX_WARN_MESSAGE, "");

        public WeaponBoxBase(string warnMsg, GameObject box)
        {
            this.box = box;
            this.modelPath = "warn_panel";
            this.warnMsg = warnMsg;
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            try
            {
                if (collider.name == "PlayerA")
                {
                    if (MemeryCacheMgr.Instance.Get(UICacheKeys.BULLET_BOX_WARN_MESSAGE) == null)
                    {
                        UIMgr.Instance.ShowPanel<WarnPanel>(modelPath, (panel) => {
                            this.panel = panel;
                            MemeryCacheMgr.Instance.Set(UICacheKeys.BULLET_BOX_WARN_MESSAGE, panel);
                            panel.Init();
                            warnText.val = warnMsg;
                            MemeryCacheMgr.Instance.Set(UICacheKeys.WEAPON_BOX, this);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        public void Destory()
        {
            if (MemeryCacheMgr.Instance.Get(UICacheKeys.BULLET_BOX_WARN_MESSAGE) as WarnPanel == panel)
            {
                UIMgr.Instance.HidePanel(modelPath);
                MemeryCacheMgr.Instance.Set(UICacheKeys.BULLET_BOX_WARN_MESSAGE, null);
                MemeryCacheMgr.Instance.Set(UICacheKeys.WEAPON_BOX, null);
            }
            if (autoRefresh == false)
            {
                GameObject.Destroy(box);
            }
            else
            {
                box.SetActive(false);
                MonoMgr.Instance.StartDelayEvent(autoRefreshTime * 1000, () =>
                {
                    box.SetActive(true);
                });
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            try
            {
                if (collider.name == "PlayerA")
                {
                    if (MemeryCacheMgr.Instance.Get(UICacheKeys.BULLET_BOX_WARN_MESSAGE) as WarnPanel == panel)
                    {
                        UIMgr.Instance.HidePanel(modelPath);
                        MemeryCacheMgr.Instance.Set(UICacheKeys.BULLET_BOX_WARN_MESSAGE, null);
                        MemeryCacheMgr.Instance.Set(UICacheKeys.WEAPON_BOX, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        public WeaponBoxType GetBoxType()
        {
            return boxType;
        }

        public WeaponBoxLifeCycle GetState()
        {
            return lifeCycle;
        }

        public Transform GetTransform()
        {
            return box.transform;
        }

        public WeaponType GetWeaponTypeID()
        {
            return weaponType;
        }

        public string GetWeaponType()
        {
            return weaponTypeDict[this.weaponType];
        }

        public WeaponBase CreatorWeapon()
        {
            return WeaponBase.ReflectionCreator(weaponTypeDict[weaponType]);
        }

        public WeaponBagPos GetWeaponBagPos()
        {
            return weaponBagPosList[(int)weaponType];
        }
    }

    public class BulletBox : WeaponBoxBase
    {
        public BulletBox(GameObject obj)
            :base("", obj)
        {
            this.warnMsg = "按 E 键补充弹药";
        }

        public int Get(int count)
        {
            return count;
        }
    }

    public class WeaponBox : WeaponBoxBase
    {
        public WeaponBox(GameObject obj, bool autoRefresh, float autoRefreshTime)
            : base("", obj)
        {
            this.autoRefresh = autoRefresh;
            this.autoRefreshTime = autoRefreshTime;
        }

        public void SetWeaponType(WeaponType weaponType)
        {
            this.weaponType = weaponType;
            warnMsg = "按 E键 获取 " + weaponNameDict[this.weaponType];
        }
    }
}
