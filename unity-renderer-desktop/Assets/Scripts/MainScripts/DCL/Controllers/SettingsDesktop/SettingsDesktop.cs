using DCL;
using DCL.SettingsCommon;
using UnityEngine;

namespace MainScripts.DCL.Controllers.SettingsDesktop
{
    public class SettingsDesktop : Singleton<SettingsDesktop>
    {
        const string DESKTOP_SETTINGS_KEY = "Settings.Quality";

        public readonly SettingsModule<DisplaySettings> displaySettings;

        public SettingsDesktop()
        {
            displaySettings = new SettingsModule<DisplaySettings>(DESKTOP_SETTINGS_KEY, GetDefaultDisplaySettings());
        }

        private DisplaySettings GetDefaultDisplaySettings()
        {
            var resolutionsLength = Screen.resolutions.Length;
            return new DisplaySettings
            {
                windowMode = WindowMode.FullScreen,
                resolutionSizeIndex = Mathf.Clamp(resolutionsLength/2, 0, resolutionsLength-1),
                vSync = false
            };
        }
    }
}