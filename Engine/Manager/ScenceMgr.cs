using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.UI;

namespace CWLEngine.Core.Manager
{
    public class ScenceMgr : Singleton<ScenceMgr>
    {
        public void LoadScence(string name, UnityAction callback = null)
        {
            SceneManager.LoadScene(name);
            callback?.Invoke();
        }

        public bool CheckScence(string name)
        {
            return SceneManager.GetActiveScene().name.Equals(name);
        }

        // 对外接口，设置 loading页面会切换什么场景
        public void LoadSceneAsyncUseLoadingBarBegin(string name)
        {
            MemeryCacheMgr.Instance.Set(EngineMacro.NEXT_SCENE, name);
            ScenceMgr.Instance.LoadScence(ScenePath.LOADING);
        }

        // 注意这里callback调用所在的还是上一个场景，所以后来我默认null了。
        // 这个函数只在 loading 页面使用。
        public void LoadSceneAsyncUseLoadingBarEnd(string name)
        {
            MonoMgr.Instance.StartCoroutine(LoadSceneAsynUseLoadingBarCoroutine(name));
        }

        private IEnumerator LoadSceneAsynUseLoadingBarCoroutine(string name)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
            asyncOperation.allowSceneActivation = false;

            LoadPanel panel = GameObject.Find("loading").GetComponent<LoadPanel>();
            Image prograss = panel.GetControl<Image>("progress");
            Text word = panel.GetControl<Text>("Text");

            while (!asyncOperation.isDone)
            {
                word.text = string.Format("{0:F}%", asyncOperation.progress * 100);
                prograss.fillAmount = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)
                {
                    word.text = "按下“空格键”继续";
                    prograss.fillAmount = 1.0f;
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                }
                yield return null;
            }
        }
    }
}
