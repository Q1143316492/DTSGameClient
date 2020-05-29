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
    public class BulletBoxTrigger : MonoBehaviour
    {        
        private WeaponBoxBase box = null;

        void Start()
        {
            box = new BulletBox(this.transform.parent.gameObject);
        }

        void OnTriggerEnter(Collider collider)
        {
            box.OnTriggerEnter(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            box.OnTriggerExit(collider);
        }
    }
}
