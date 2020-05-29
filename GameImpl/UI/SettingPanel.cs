using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CWLEngine.GameImpl.UI
{
    public class SettingPanel : PanelBase
    {
        private Button btnSave = null;
        private Button btnExit = null;
        private Button btnExit2 = null;

        private Dropdown resolutionDropdown = null;

        private int resolutionIndex = 0;
       
        private class ResolutionOptions
        {
            public string optName;
            public int height;
            public int width;
            public bool isFullScreen = false;

            public ResolutionOptions(string name, int width, int height, bool isFullScreen)
            {
                this.optName = name;
                this.width = width;
                this.height = height;
                this.isFullScreen = isFullScreen;
            }
        }

        private Dictionary<int, ResolutionOptions> resolutionMapping = new Dictionary<int, ResolutionOptions>();

        private void TurnBackToMainMenu()
        {
            UIMgr.Instance.HidePanel(UIPanelPath.SETTING);
        }

        void Start()
        {
            btnSave = GetControl<Button>("btn_setting_yes");
            
            resolutionMapping.Add(0, new ResolutionOptions("全屏模式 1920 * 1080", 1920, 1080, true));
            resolutionMapping.Add(1, new ResolutionOptions("全屏模式 1366 *  768", 1366,  768, true));
            resolutionMapping.Add(2, new ResolutionOptions("全屏模式 1024 *  768", 1024,  768, true));
            resolutionMapping.Add(3, new ResolutionOptions("窗口模式 1920 * 1080", 1920, 1080, false));
            resolutionMapping.Add(4, new ResolutionOptions("窗口模式 1366 *  768", 1366,  768, false));
            resolutionMapping.Add(5, new ResolutionOptions("窗口模式 1024 *  768", 1024,  768, false));

            //resolutionMapping.Add(3, new ResolutionOptions("全屏模式  800 *  600",  800,  600, true));
            //resolutionMapping.Add(7, new ResolutionOptions("窗口模式  800 *  600",  800,  600, false));

            btnSave.onClick.AddListener(() => {

                if (resolutionMapping.ContainsKey(resolutionIndex))
                {
                    ResolutionOptions options = resolutionMapping[resolutionIndex];
                    Screen.SetResolution(options.width, options.height, options.isFullScreen);
                }
            });

            btnExit = GetControl<Button>("btn_setting_close");
            btnExit2 = GetControl<Button>("btn_setting_no");

            btnExit.onClick.AddListener(TurnBackToMainMenu);
            btnExit2.onClick.AddListener(TurnBackToMainMenu);

            resolutionDropdown = GetControl<Dropdown>("resolution_drop");

            List<string> resolutionDropOptions = new List<string>();

            for (int i = 0; i < resolutionMapping.Count; i++ )
            {
                resolutionDropOptions.Add(resolutionMapping[i].optName);
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionDropOptions);

            resolutionDropdown.onValueChanged.AddListener((index) =>
            {
                resolutionIndex = index;
            });
            
        }
    }
}
