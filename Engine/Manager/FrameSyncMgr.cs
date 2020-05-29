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
    public class FrameSyncMgr : Singleton<FrameSyncMgr>
    {
        private int logicFrameCount = 0;
        
        public int GetFrame()
        {
            return logicFrameCount;
        }

        public void SetFrame(int frame)
        {
            logicFrameCount = frame;
        }

        public void ReStart()
        {
            logicFrameCount = 0;
        }

        public void NextFrame()
        {
            logicFrameCount++;
        }
    }
}
