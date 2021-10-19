using MainScripts.DCL.Controllers.HUD.Preloading;

namespace DCL
{
    /// <summary>
    /// This is the MainDesktop entry point.
    /// Most of the application subsystems should be initialized from this class Awake() event.
    /// </summary>
    public class MainDesktop : Main
    {
        protected override void Awake()
        {
            var preloading = new PreloadingController();
            preloading.Initialize();
            
            base.Awake();
            CommandLineParserUtils.ParseArguments();
        }
        protected override HUDContext HUDContextBuilder()
        {
            return HUDDesktopContextFactory.CreateDefault();
        }
    }
}