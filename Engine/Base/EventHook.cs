using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CWLEngine.Core.Base
{
    public class EventHook : MonoBehaviour
    {
        private event UnityAction updateEvent;
        private event UnityAction fixedUpdateEvent;

        void Update()
        {
            updateEvent?.Invoke();
        }

        void FixedUpdate()
        {
            fixedUpdateEvent?.Invoke();
        }

        public void AddUpdateEvent(UnityAction fun)
        {
            updateEvent += fun;
        }

        public void DelUpdateEvent(UnityAction fun)
        {
            updateEvent -= fun;
        }

        public void AddFixedUpdateEvent(UnityAction fun)
        {
            fixedUpdateEvent += fun;
        }

        public void DelFixedUpdateEvent(UnityAction fun)
        {
            fixedUpdateEvent -= fun;
        }
    }
}
