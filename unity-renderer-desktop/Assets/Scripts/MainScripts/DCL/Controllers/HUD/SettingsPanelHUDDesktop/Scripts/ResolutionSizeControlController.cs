using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/Resolution Size",
        fileName = "ResolutionSizeControlController")]
    public class ResolutionSizeControlController : SpinBoxSettingsControlControllerDesktop
    {
        public override object GetStoredValue()
        {
            return currentDisplaySettings.resolutionSizeIndex;
        }

        public override void UpdateSetting(object newValue)
        {
            var value = (int)newValue;
            currentDisplaySettings.resolutionSizeIndex = value;
            var currentResolution = Screen.resolutions[value];
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreenMode);
        }
    }
}