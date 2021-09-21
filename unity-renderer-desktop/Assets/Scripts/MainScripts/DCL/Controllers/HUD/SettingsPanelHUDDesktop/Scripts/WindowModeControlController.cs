using System;
using MainScripts.DCL.Controllers.SettingsDesktop;
using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/Window Mode",
        fileName = "WindowModeControlController")]
    public class WindowModeControlController : SpinBoxSettingsControlControllerDesktop
    {
        public override object GetStoredValue() { return (int)currentDisplaySettings.windowMode; }

        public override void UpdateSetting(object newValue)
        {
            currentDisplaySettings.windowMode = (WindowMode)(int)newValue;

            switch (currentDisplaySettings.windowMode)
            {
                case WindowMode.Windowed:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                case WindowMode.FullScreen:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Screen.fullScreen = true;
        }
        
    }
}