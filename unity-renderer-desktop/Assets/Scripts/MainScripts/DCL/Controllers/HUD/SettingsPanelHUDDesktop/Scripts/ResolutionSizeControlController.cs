using System.Linq;
using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using MainScripts.DCL.ScriptableObjectsDesktop;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/Resolution Size",
        fileName = "ResolutionSizeControlController")]
    public class ResolutionSizeControlController : SpinBoxSettingsControlControllerDesktop
    {
        private Resolution[] availableFilteredResolutions;

        public override void Initialize()
        {
            SetupAvailableResolutions();
            base.Initialize();
            SetupLabels();
        }

        // Filter the smallest resolutions as no one will ever use them
        private void SetupAvailableResolutions()
        {
            availableFilteredResolutions = Screen.resolutions.Where(r => r.width >= 1024 && r.refreshRate > 0).ToArray();
        }

        private void SetupLabels()
        {
            var length = availableFilteredResolutions.Length;
            var resolutionLabels = new string[length];
            for (var i = 0; i < length; i++)
            {
                Resolution resolution = availableFilteredResolutions[i];
                
                // by design we want the list to be inverted so the biggest resolutions stay on top
                // our resolutionSizeIndex is based on this decision
                resolutionLabels[length - 1 - i] = GetLabel(resolution);
            }

            RaiseOnOverrideIndicatorLabel(resolutionLabels);
        }

        private static string GetLabel(Resolution resolution)
        {
            return $"{resolution.width}x{resolution.height} {resolution.refreshRate}Hz";
        }

        public override object GetStoredValue()
        {
            return currentDisplaySettings.resolutionSizeIndex;
        }

        public override void UpdateSetting(object newValue)
        {
            var value = (int)newValue;
            currentDisplaySettings.resolutionSizeIndex = value;
            var currentResolution = availableFilteredResolutions[availableFilteredResolutions.Length - 1 - value];
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreenMode);
        }
    }
}