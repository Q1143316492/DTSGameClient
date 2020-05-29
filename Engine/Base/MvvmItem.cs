using CWLEngine.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CWLEngine.Core.Base
{
    public class UITYPE_NOTBIND<T>
    {
        public T val;
        public UITYPE_NOTBIND(string ID, T val)
        {
            this.val = val;
        }
    }

    public class UIType<T>
    {
        public UIType(string ID, T value)
        {
            this.ID = ID;
            _value = value;
            MvvmMgr.Instance.Set(ID, _value);
        }

        private string ID;

        private T _value;
        public T val
        {
            get
            {
                T oldVal = _value;
                try
                {
                    object _tmpval = MvvmMgr.Instance.Get(ID);
                    _value = (T)_tmpval;
                }
                catch (Exception)
                {
                    _value = oldVal;
                }
                return _value;
            }
            set
            {
                _value = value;
                MvvmMgr.Instance.Set(ID, _value);
            }
        }
    }
}
