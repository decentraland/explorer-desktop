using System;
using MainScripts.DCL.Controllers.SettingsDesktop;
using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using MainScripts.DCL.ScriptableObjectsDesktop;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/Window Mode",
        fileName = "WindowModeControlController")]
    public class WindowModeControlController : SpinBoxSettingsControlControllerDesktop
    {
        public override object GetStoredValue()
        {
            return (int)currentDisplaySettings.windowMode;
        }

        public override void UpdateSetting(object newValue)
        {
            currentDisplaySettings.windowMode = (WindowMode)(int)newValue;
            switch (currentDisplaySettings.windowMode)
            {
                case WindowMode.Windowed:
                    Screen.fullScreen = false;
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                case WindowMode.Borderless:
                    var maxRes = Screen.resolutions[Screen.resolutions.Length - 1];
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    Screen.SetResolution(maxRes.width, maxRes.height, FullScreenMode.FullScreenWindow, maxRes.refreshRate);
                    currentDisplaySettings.resolutionSizeIndex = 0;
                    break;
                case WindowMode.FullScreen:
                    Screen.fullScreen = true;
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CommonScriptableObjectsDesktop.disableVSync.Set(currentDisplaySettings.windowMode == WindowMode.Windowed);
            CommonScriptableObjectsDesktop.disableScreenResolution.Set(currentDisplaySettings.windowMode == WindowMode.Borderless);
        }
    }
}