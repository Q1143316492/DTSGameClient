using CWLEngine.GameImpl.Entity;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.GameImpl.Base;

namespace CWLEngine.GameImpl.Controller
{
    public class WeaponBoxController
    {
        private CharacterBase soldier = null;

        public WeaponBoxController(CharacterBase soldier)
        {
            this.soldier = soldier;
        }

        public void AddBulletBox()
        {

        }

        void OnTriggerEnter(Collider collider)
        {
            
        }

        void OnTriggerExit(Collider collider)
        {
            
        }
    }
}
