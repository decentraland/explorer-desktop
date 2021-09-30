using DCL;
using DCL.SettingsCommon;

namespace MainScripts.DCL.Controllers.SettingsDesktop
{
    public class SettingsDesktop : Singleton<SettingsDesktop>
    {
        const string DESKTOP_SETTINGS_KEY = "Settings.Quality.Desktop";

        public readonly SettingsModule<DisplaySettings> displaySettings;

        public SettingsDesktop()
        {
            displaySettings = new SettingsModule<DisplaySettings>(DESKTOP_SETTINGS_KEY, GetDefaultDisplaySettings());
        }

        private DisplaySettings GetDefaultDisplaySettings()
        {
            return new DisplaySettings
            {
                windowMode = WindowMode.FullScreen,
                resolutionSizeIndex = 0,
                vSync = false
            };
        }
    }
}