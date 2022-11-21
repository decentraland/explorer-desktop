
namespace DCL.Controllers.HUD
{
    public class MinimapHUDControllerDesktop : MinimapHUDController
    {
        protected override MinimapHUDView CreateView()
        {
            return MinimapHUDViewDesktop.Create(this);
        }

    public MinimapHUDControllerDesktop(MinimapMetadataController minimapMetadataController, IHomeLocationController locationController) : base(minimapMetadataController, locationController)
        {
        }
    public MinimapHUDControllerDesktop(MinimapHUDModel model, MinimapMetadataController minimapMetadataController, IHomeLocationController locationController) : base(model, minimapMetadataController, locationController)
        {
        }
    }
}