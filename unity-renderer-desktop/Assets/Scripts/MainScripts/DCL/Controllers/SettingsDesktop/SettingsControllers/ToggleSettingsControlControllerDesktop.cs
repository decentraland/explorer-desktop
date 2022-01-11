using DCL;
using DCL.SettingsCommon;
using DCL.SettingsCommon.SettingsControllers.BaseControllers;

namespace MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers
{
    public class ToggleSettingsControlControllerDesktop : ToggleSettingsControlController
    {
        protected DisplaySettings currentDisplaySettings;
        protected DesktopQualitySettings currentDesktopQualitySettings;
        private ISettingsRepository<DisplaySettings> DisplaySettings => SettingsDesktop.i.displaySettings;
        private ISettingsRepository<DesktopQualitySettings> QualitySettings => SettingsDesktop.i.qualitySettings;

        public override void Initialize()
        {
            currentDisplaySettings = DisplaySettings.Data;
            currentDesktopQualitySettings = QualitySettings.Data;
            DisplaySettings.OnChanged += OnDisplaySettingsChanged;
            QualitySettings.OnChanged += OnQualitySettingsChanged;
            base.Initialize();
        }

        private void OnQualitySettingsChanged(DesktopQualitySettings settings)
        {
            currentDesktopQualitySettings = settings;
        }

        private void OnDisplaySettingsChanged(DisplaySettings settings)
        {
            currentDisplaySettings = settings;
        }

        public override void ApplySettings()
        {
            base.ApplySettings();
            DisplaySettings.Apply(currentDisplaySettings);
            QualitySettings.Apply(currentDesktopQualitySettings);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DisplaySettings.OnChanged -= OnDisplaySettingsChanged;
            QualitySettings.OnChanged -= OnQualitySettingsChanged;
        }
    }
}