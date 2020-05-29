using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;

namespace CWLEngine.Core.Manager
{

    public class UIMgr : Singleton<UIMgr>
    {
        private Dictionary<string, PanelBase> panelDict = new Dictionary<string, PanelBase>();

        GameObject canvas = null;

        public UIMgr()
        {
            Init();
        }

        public void Init()
        {
            canvas = GameObject.Find("Canvas");
        }

        public void ShowPanel<T>(string name, UnityAction<T>callback = null) where T: PanelBase
        {
            // todo 注意不要连续快速加载同一UI
            if (panelDict.ContainsKey(name))
            {
                callback?.Invoke(panelDict[name] as T);
                return;
            }

            ResourceMgr.Instance.LoadAsync<GameObject>("UI/" + name, (obj) =>
            {
                if (canvas == null)
                {
                    // 只有过场景的时候才需要从新找一次, 所以这里Find不会频繁被调用
                    canvas = GameObject.Find("Canvas");
                }

                obj.transform.SetParent(canvas.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.name = name;
                (obj.transform as RectTransform).offsetMax = Vector2.zero;
                (obj.transform as RectTransform).offsetMin = Vector2.zero;


                T panel = obj.GetComponent<T>();
                panelDict.Add(name, panel);
                panel.Show();
                callback?.Invoke(panel);

            });
        }

        public void HidePanel(string name)
        {
            if (panelDict.ContainsKey(name))
            {
                PanelBase panel = panelDict[name];
                panel.Hide();
                GameObject.Destroy(panel.gameObject);
                panelDict.Remove(name);
            }
        }
    }
}
