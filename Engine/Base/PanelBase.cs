using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using CWLEngine.Core.Manager;

namespace CWLEngine.Core.Base
{
    /**
       挂在每个面板上 把组件归类。让 GetControl 可以直接拿到那个东西的引用
         */
    public class PanelBase : MonoBehaviour
    {
        private Dictionary<string, List<UIBehaviour>> controlDict = new Dictionary<string, List<UIBehaviour>>();

        void Awake()
        {
            FindChildrenControl<Button>();
            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<Slider>();
            FindChildrenControl<ScrollRect>();
            FindChildrenControl<InputField>();
            FindChildrenControl<Dropdown>();
        }

        void OnDestroy()
        {

        }

        public T GetControl<T>(string name) where T : UIBehaviour
        {
            if (controlDict.ContainsKey(name))
            {
                for (int i = 0; i < controlDict[name].Count; i++)
                {
                    if (controlDict[name][i] is T)
                    {
                        return controlDict[name][i] as T;
                    }
                }
            }
            return null;
        }

        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            T[] controls = GetComponentsInChildren<T>();
            for (int i = 0; i < controls.Length; i++)
            {
                string name = controls[i].gameObject.name;
                if (controlDict.ContainsKey(name))
                {
                    controlDict[name].Add(controls[i]);
                }
                else
                {
                    controlDict.Add(name, new List<UIBehaviour>() { controls[i] });
                }
            }
        }

        public virtual void Show()
        {

        }

        public virtual void Hide()
        {

        }
    }
}
