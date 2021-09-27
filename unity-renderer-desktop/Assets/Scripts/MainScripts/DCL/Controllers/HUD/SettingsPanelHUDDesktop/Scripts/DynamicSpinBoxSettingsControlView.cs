using DCL.SettingsCommon.SettingsControllers.BaseControllers;
using DCL.SettingsPanelHUD.Controls;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    public class DynamicSpinBoxSettingsControlView : SpinBoxSettingsControlView
    {
        public override void Initialize(SettingsControlModel controlConfig, SettingsControlController settingsControlController)
        {
            base.Initialize(controlConfig, settingsControlController);
            SetLabels(GetDynamicLabels());
        }

        private string[] GetDynamicLabels()
        {
            var labelProvider = GetComponent<ISpinBoxLabelProvider>();
            if (labelProvider == null) return new[] { "ERROR" };
            return labelProvider.GetLabels();
        }
    }
}