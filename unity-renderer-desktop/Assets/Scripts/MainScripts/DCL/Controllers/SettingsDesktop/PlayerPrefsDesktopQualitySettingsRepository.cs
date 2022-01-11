using System;
using DCL.SettingsCommon;
using UnityEngine;

namespace DCL
{
    public class PlayerPrefsDesktopQualitySettingsRepository : ISettingsRepository<DesktopQualitySettings>
    {
        public const string DEPTH_OF_FIELD = "depthOfField";

        private readonly IPlayerPrefsSettingsByKey settingsByKey;
        private readonly DesktopQualitySettings defaultSettings;
        private DesktopQualitySettings currentSettings;
        
        public event Action<DesktopQualitySettings> OnChanged;

        public PlayerPrefsDesktopQualitySettingsRepository(
            IPlayerPrefsSettingsByKey settingsByKey,
            DesktopQualitySettings defaultSettings)
        {
            this.settingsByKey = settingsByKey;
            this.defaultSettings = defaultSettings;
            currentSettings = Load();
        }

        public DesktopQualitySettings Data => currentSettings;

        public void Apply(DesktopQualitySettings settings)
        {
            if (currentSettings.Equals(settings)) return;
            currentSettings = settings;
            OnChanged?.Invoke(currentSettings);
        }

        public void Reset()
        {
            Apply(defaultSettings);
        }

        public void Save()
        {
            settingsByKey.SetBool(DEPTH_OF_FIELD, currentSettings.depthOfField);
        }

        public bool HasAnyData() => !Data.Equals(defaultSettings);

        private DesktopQualitySettings Load()
        {
            var settings = defaultSettings;
            
            try
            {
                settings.depthOfField = settingsByKey.GetBool(DEPTH_OF_FIELD, defaultSettings.depthOfField);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return settings;
        }
    }
}