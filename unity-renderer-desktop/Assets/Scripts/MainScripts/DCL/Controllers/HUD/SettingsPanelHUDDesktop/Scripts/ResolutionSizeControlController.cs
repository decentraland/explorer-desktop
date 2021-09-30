using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using MainScripts.DCL.ScriptableObjectsDesktop;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/Resolution Size",
        fileName = "ResolutionSizeControlController")]
    public class ResolutionSizeControlController : SpinBoxSettingsControlControllerDesktop
    {
        public override void Initialize()
        {
            base.Initialize();
            SetupLabels();
            CommonScriptableObjectsDesktop.disableScreenResolution.OnChange += OnDisableOption;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            CommonScriptableObjectsDesktop.disableScreenResolution.OnChange -= OnDisableOption;
        }

        private void OnDisableOption(bool current, bool previous)
        {
            if (current)
            {
                RaiseOnCurrentLabelChange($"{Display.main.systemWidth}x{Display.main.systemHeight} (Forced)");
            }
            else
            {
                var resolutionSizeIndex = currentDisplaySettings.resolutionSizeIndex;
                UpdateSetting(resolutionSizeIndex);
                var currentResolution = Screen.resolutions[Screen.resolutions.Length - 1 - resolutionSizeIndex];
                RaiseOnCurrentLabelChange($"{currentResolution.width}x{currentResolution.height}");
            }
        }

        private void SetupLabels()
        {
            var resolutions = Screen.resolutions;
            var resolutionLabels = new string[resolutions.Length];
            for (var i = 0; i < resolutions.Length; i++)
            {
                Resolution resolution = resolutions[i];
                resolutionLabels[resolutions.Length - 1 - i] = $"{resolution.width}x{resolution.height}";
            }

            RaiseOnOverrideIndicatorLabel(resolutionLabels);
        }

        public override object GetStoredValue()
        {
            return currentDisplaySettings.resolutionSizeIndex;
        }

        public override void UpdateSetting(object newValue)
        {
            var value = (int)newValue;
            currentDisplaySettings.resolutionSizeIndex = value;
            var currentResolution = Screen.resolutions[Screen.resolutions.Length - 1 - value];
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreenMode);
        }
    }
}