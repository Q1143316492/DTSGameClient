using CWLEngine.Core.Manager;
using CWLEngine.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Entity;

namespace CWLEngine.GameImpl.Controller
{
    public class WeaponBoxTrigger : MonoBehaviour
    {
        public WeaponType weaponType;
        public bool autoRefresh = false;
        public float autoRefreshTime = 5.0f; // s

        private WeaponBox weaponBox = null;
        
        void Start()
        {
            weaponBox = new WeaponBox(transform.parent.gameObject, autoRefresh, autoRefreshTime);
        }

        void OnTriggerEnter(Collider collider)
        {
            weaponBox.SetWeaponType(weaponType);
            weaponBox.OnTriggerEnter(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            weaponBox.OnTriggerExit(collider);
        }
    }
}
