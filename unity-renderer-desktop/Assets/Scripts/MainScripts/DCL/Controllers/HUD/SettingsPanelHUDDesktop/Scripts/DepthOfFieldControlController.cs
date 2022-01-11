using DCL;
using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/DepthOfField", fileName = "DepthOfFieldControlController")]
    public class DepthOfFieldControlController : ToggleSettingsControlControllerDesktop
    {
        public override object GetStoredValue()
        {
            return currentDesktopQualitySettings.depthOfField;
        }

        public override void UpdateSetting(object newValue)
        {
            var value = (bool) newValue;
            currentDesktopQualitySettings.depthOfField = value;
            
            if (SceneReferences.i.postProcessVolume.profile.TryGet<DepthOfField>(out var depthOfField))
                depthOfField.active = currentDesktopQualitySettings.depthOfField;
        }
    }
}