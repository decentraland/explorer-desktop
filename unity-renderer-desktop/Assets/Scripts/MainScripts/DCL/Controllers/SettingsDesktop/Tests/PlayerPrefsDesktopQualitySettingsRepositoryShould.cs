using NSubstitute;
using NUnit.Framework;

namespace DCL.SettingsCommon
{
    public class PlayerPrefsDesktopQualitySettingsRepositoryShould
    {
        [Test]
        public void GetDataWhenIsAnythingStored()
        {
            var storedSettings = new DesktopQualitySettings
            {
                depthOfField = false
            };
            var settingsByKey = GivenStoredSettings(storedSettings);
            var repository = new PlayerPrefsDesktopQualitySettingsRepository(settingsByKey,
                GetDefaultDisplaySettings());

            var settings = WhenGetSettings(repository);
            
            Assert.AreEqual(storedSettings, settings);
        }

        [Test]
        public void GetDefaultSettingsWhenNothingIsStored()
        {
            var settingsByKey = GivenNoStoredSettings();
            var defaultSettings = GetDefaultDisplaySettings();
            var repository = new PlayerPrefsDesktopQualitySettingsRepository(settingsByKey,
                defaultSettings);
            
            var settings = WhenGetSettings(repository);
            
            Assert.AreEqual(defaultSettings, settings);
        }

        [Test]
        public void ApplySettings()
        {
            var newSettings = new DesktopQualitySettings
            {
                depthOfField = true
            };
            var settingsByKey = GivenNoStoredSettings();
            var repository = new PlayerPrefsDesktopQualitySettingsRepository(settingsByKey,
                GetDefaultDisplaySettings());
            var onChangedCalled = false;
            repository.OnChanged += x => onChangedCalled = true;
            
            repository.Apply(newSettings);
            var settings = repository.Data;
            
            Assert.AreEqual(newSettings, settings);
            Assert.IsTrue(onChangedCalled);
        }

        [Test]
        public void SaveSettings()
        {
            var newSettings = new DesktopQualitySettings
            {
                depthOfField = true
            };
            var settingsByKey = GivenNoStoredSettings();
            var repository = new PlayerPrefsDesktopQualitySettingsRepository(settingsByKey,
                GetDefaultDisplaySettings());
            
            WhenSettingsAreSaved(repository, newSettings);

            ThenSettingsAreSaved(settingsByKey, newSettings);
        }

        private DesktopQualitySettings WhenGetSettings(PlayerPrefsDesktopQualitySettingsRepository repository) =>
            repository.Data;

        private void WhenSettingsAreSaved(PlayerPrefsDesktopQualitySettingsRepository repository,
            DesktopQualitySettings newSettings)
        {
            repository.Apply(newSettings);
            repository.Save();
        }

        private void ThenSettingsAreSaved(IPlayerPrefsSettingsByKey settingsByKey, DesktopQualitySettings settings)
        {
            settingsByKey.Received(1).SetBool(PlayerPrefsDesktopQualitySettingsRepository.DEPTH_OF_FIELD, settings.depthOfField);
        }

        private IPlayerPrefsSettingsByKey GivenStoredSettings(DesktopQualitySettings settings)
        {
            var settingsByKey = Substitute.For<IPlayerPrefsSettingsByKey>();
            settingsByKey.GetBool(PlayerPrefsDesktopQualitySettingsRepository.DEPTH_OF_FIELD, Arg.Any<bool>())
                .Returns(settings.depthOfField);
            return settingsByKey;
        }
        
        private IPlayerPrefsSettingsByKey GivenNoStoredSettings()
        {
            var settingsByKey = Substitute.For<IPlayerPrefsSettingsByKey>();
            settingsByKey.GetBool(Arg.Any<string>(), Arg.Any<bool>())
                .Returns(call => call[1]);
            return settingsByKey;
        }
        
        private DesktopQualitySettings GetDefaultDisplaySettings()
        {
            return new DesktopQualitySettings
            {
                depthOfField = false
            };
        }
    }
}