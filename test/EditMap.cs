using CWLEngine.GameImpl.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.test
{
    public class EditMap : MonoBehaviour
    {
        void Start()
        {
            GameMapController gameMapController = new GameMapController();
            gameMapController.CreateStatic();
        }
    }
}
