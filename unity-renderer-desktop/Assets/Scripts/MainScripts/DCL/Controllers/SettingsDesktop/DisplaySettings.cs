using System;

namespace MainScripts.DCL.Controllers.SettingsDesktop
{
    [Serializable]
    public enum WindowMode
    {
        Windowed,
        FullScreen,
    }

    [Serializable]
    public struct DisplaySettings
    {
        public WindowMode windowMode;
        public int resolutionSizeIndex;
    }
}