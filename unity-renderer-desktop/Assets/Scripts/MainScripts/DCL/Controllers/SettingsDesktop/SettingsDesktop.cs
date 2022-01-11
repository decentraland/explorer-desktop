using DCL;
using DCL.Helpers;
using DCL.SettingsCommon;

namespace MainScripts.DCL.Controllers.SettingsDesktop
{
    public class SettingsDesktop : Singleton<SettingsDesktop>
    {
        // TODO: should be Settings.Display.Desktop.. beware of retro-compatibility issues
        private const string DISPLAY_SETTINGS_KEY = "Settings.Quality.Desktop";
        private const string QUALITY_SETTINGS_KEY = "Settings.Quality.Desktop.Variant";

        public readonly ISettingsRepository<DisplaySettings> displaySettings;
        public readonly ISettingsRepository<DesktopQualitySettings> qualitySettings;

        public SettingsDesktop()
        {
            displaySettings = new ProxySettingsRepository<DisplaySettings>(
                new PlayerPrefsDesktopDisplaySettingsRepository(
                    new PlayerPrefsSettingsByKey(DISPLAY_SETTINGS_KEY), GetDefaultDisplaySettings()),
                new SettingsModule<DisplaySettings>(DISPLAY_SETTINGS_KEY, GetDefaultDisplaySettings()));
            qualitySettings = new PlayerPrefsDesktopQualitySettingsRepository(
                new PlayerPrefsSettingsByKey(QUALITY_SETTINGS_KEY),
                GetDefaultQualitySettings());
        }

        private DesktopQualitySettings GetDefaultQualitySettings()
        {
            return new DesktopQualitySettings {depthOfField = true};
        }

        private DisplaySettings GetDefaultDisplaySettings()
        {
            return new DisplaySettings
            {
                windowMode = WindowMode.Borderless,
                resolutionSizeIndex = 0,
                vSync = false
            };
        }

        public void Save()
        {
            displaySettings.Save();
            qualitySettings.Save();
            PlayerPrefsUtils.Save();
        }
    }
}