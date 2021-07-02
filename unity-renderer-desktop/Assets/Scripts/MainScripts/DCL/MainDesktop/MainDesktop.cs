namespace DCL
{
    /// <summary>
    /// This is the MainDesktop entry point.
    /// Most of the application subsystems should be initialized from this class Awake() event.
    /// </summary>
    public class MainDesktop : Main
    {
        protected override HUDContext HUDContextBuilder()
        {
            return HUDDesktopContextFactory.CreateDefault();
        }
    }
}