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
                    Screen.fullScreen = true;
                    //Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
#if UNITY_STANDALONE_OSX
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
#else
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
#endif
                    break;
                case WindowMode.FullScreen:
                    Screen.fullScreen = true;
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CommonScriptableObjectsDesktop.disableVSync.Set(currentDisplaySettings.windowMode != WindowMode.FullScreen);
            CommonScriptableObjectsDesktop.disableScreenResolution.Set(currentDisplaySettings.windowMode ==
                                                                       WindowMode.Borderless);
        }
    }
}