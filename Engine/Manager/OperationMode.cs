using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using UnityEngine;
using UnityEditor;
using System.IO;
using CWLEngine.GameImpl.Conf;
using CWLEngine.GameImpl.Util;

namespace CWLEngine.Core.Manager
{
    public class OperationMode : Singleton<OperationMode>
    {
        bool bLockOpt = false;

        public OperationMode()
        {
            MonoMgr.Instance.AddUpdateEvent(Update);
        }

        public void Unlock()
        {
            bLockOpt = false;
        }

        public void Lock()
        {
            bLockOpt = true;
        }
        
        public bool IsLock()
        {
            return bLockOpt;
        }

        void Update()
        {
            
        }
    }
}
