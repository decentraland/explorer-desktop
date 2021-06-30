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
            default:
                hudElement = base.CreateHUD(hudElementId);
                break;
        }

        return hudElement;
    }
}