using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;

namespace CWLEngine.GameImpl.Controller
{
    // 人物下落，落地触发事件，修改在空中的姿势
    public class DropDownCollider : MonoBehaviour
    {
        Transform root = null;

        void Start()
        {
            if (root == null)
            {
                root = gameObject.transform;
                while (root.parent != null)
                {
                    root = root.parent;
                }
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (root != null)
            {
                MemeryCacheMgr.Instance.Set(root.name.ToString() + "#" + DTSKeys.DROP_DOWN, true);
            }
        }
    }
}

