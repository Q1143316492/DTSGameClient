using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Base
{
    public class FrameSync : MonoBehaviour
    {
        private int frame = 0;
        private float interval = 0.1f;

        private static int cnt = 0;

        void FrameUpdate()
        {
            cnt++;
            Debug.Log(Time.time);
        }

        void Update()
        {
            while (Time.time > frame * interval)
            {
                FrameUpdate();
                frame++;
            }
        }
    }
}
