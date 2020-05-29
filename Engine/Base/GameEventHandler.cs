using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace CWLEngine.Core.Base
{
    // 和 EventHook 唯一区别是 Update FixedUpdate 访问级别。后来没什么用了
    public class GameEventHandler
    {
        private event UnityAction updateEvent;
        private event UnityAction fixedUpdateEvent;

        public void Update()
        {
            updateEvent?.Invoke();
        }

        public void FixedUpdate()
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
