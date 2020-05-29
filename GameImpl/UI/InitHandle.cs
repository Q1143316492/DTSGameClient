using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.Core.Manager;

namespace CWLEngine.GameImpl.UI
{
    public class InitHandle : MonoBehaviour
    {
        void Start()
        {
            UIMgr.Instance.ShowPanel<MainMenuPanel>("main_menu", (panel)=> {

            });
        }
    }
}
