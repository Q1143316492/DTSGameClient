using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.Core.Manager;


namespace CWLEngine.GameImpl.Entity
{
    public class DelayRemove : MonoBehaviour
    {
        public float delayTime = 1.0f;
        public bool useObjectPool = true;

        void Start()
        {
        }

        public void StartDelayRemove()
        {
            Invoke("RemoveGameobject", delayTime);
        }

        void RemoveGameobject()
        {
            if (useObjectPool)
            {
                ObjectPoolMgr.Instance.RemoveGameObject(gameObject, name);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
