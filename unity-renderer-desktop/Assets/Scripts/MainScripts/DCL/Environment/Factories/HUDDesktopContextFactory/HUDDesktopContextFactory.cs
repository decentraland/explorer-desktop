namespace DCL
{
    public static class HUDDesktopContextFactory
    {
        public static HUDContext CreateDefault() { return new HUDContext(new HUDDesktopFactory(), new HUDController()); }
    }
}