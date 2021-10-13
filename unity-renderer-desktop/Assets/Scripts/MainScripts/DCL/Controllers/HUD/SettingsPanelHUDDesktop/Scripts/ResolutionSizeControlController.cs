using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
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
            
            Display.onDisplaysUpdated += OnUpdate;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Display.onDisplaysUpdated -= OnUpdate;
        }

        private void OnUpdate()
        {
            SetupLabels();
            UpdateSetting(currentDisplaySettings.resolutionSizeIndex);
        }

        private void SetupAvailableResolutions()
        {
            availableFilteredResolutions = CurrentDisplayControlController.GetAvailableFilteredResolutions();
        }

        private void SetupLabels()
        {
            var length = availableFilteredResolutions.Length;
            var resolutionLabels = new string[length];
            for (var i = 0; i < length; i++)
            {
                Resolution resolution = availableFilteredResolutions[i];
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