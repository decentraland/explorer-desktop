using DCL.SettingsPanelHUD;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    public class SettingsPanelHUDControllerDesktop : SettingsPanelHUDController
    {
        protected override SettingsPanelHUDView CreateView()
        {
            return SettingsPanelHUDViewDesktop.Create();
        }
    }
}