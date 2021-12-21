using MainScripts.DCL.Controllers.HUD.Profile;
using MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts;

namespace MainScripts.DCL.Controllers.HUD
{
    public class HUDDesktopFactory : HUDFactory
    {
        public override IHUD CreateHUD(HUDElementID hudElementId)
        {
            IHUD hudElement = null;

            switch (hudElementId)
            {
                case HUDElementID.NONE:
                    break;
                case HUDElementID.TASKBAR:
                    hudElement = new TaskbarHUDControllerDesktop();
                    break;
                case HUDElementID.SETTINGS_PANEL:
                    hudElement = new SettingsPanelHUDControllerDesktop();
                    break;
                case HUDElementID.PROFILE_HUD:
                    hudElement = new ProfileHUDControllerDesktop(new UserProfileWebInterfaceBridge());
                    break;
                default:
                    hudElement = base.CreateHUD(hudElementId);
                    break;
            }

            return hudElement;
        }
    }
}