using System.Linq;
using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/Current Display",
        fileName = "CurrentDisplayControlController")]
    public class CurrentDisplayControlController : SpinBoxSettingsControlControllerDesktop
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
        }

        private void SetupAvailableResolutions()
        {
            availableFilteredResolutions = GetAvailableFilteredResolutions();
        }

        public static Resolution[] GetAvailableFilteredResolutions()
        {
            return Screen.resolutions.Where(r => r.width >= 1024 && r.refreshRate > 0).ToArray();
        }

        private void SetupLabels()
        {
            var length = Display.displays.Length;
            var displayLabels = new string[length];
            for (var i = 0; i < length; i++)
            {
                displayLabels[length - 1 - i] = $"Display {i+1}";
            }

            RaiseOnOverrideIndicatorLabel(displayLabels);
        }

        public override object GetStoredValue()
        {
            return currentDisplaySettings.displayIndex;
        }

        public override void UpdateSetting(object newValue)
        {
            if (currentDisplaySettings.displayIndex >= Display.displays.Length)
            {
                currentDisplaySettings.displayIndex = 0;
            }
            else
            {
                var value = (int)newValue;
                currentDisplaySettings.displayIndex = value;
            }
            var currentResolution = availableFilteredResolutions[availableFilteredResolutions.Length - 1 - currentDisplaySettings.resolutionSizeIndex];
            Display.displays[currentDisplaySettings.displayIndex].Activate(currentResolution.width, currentResolution.width, currentResolution.refreshRate);
        }
    }
}