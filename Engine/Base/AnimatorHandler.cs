using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.Core.Base
{
    /*
     想封装一下动画模块
         */
    public class AnimatorHandler
    {
        public Animator animator;

        public AnimatorHandler(Animator animator)
        {
            this.animator = animator;
        }

        public void PlayAnimation(string name, int layer)
        {
            if (name == string.Empty)
            {
                Debug.Log("Play Animation string name empty");
                return;
            }
            animator.Play(name, layer);
        }
    }
}
