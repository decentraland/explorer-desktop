using System;
using UnityEngine;

namespace MainScripts.DCL.Controllers.SettingsDesktop
{
    [Serializable]
    public enum WindowMode
    {
        Windowed,
        Borderless,
        FullScreen,
    }

    [Serializable]
    public struct DisplaySettings
    {
        public WindowMode windowMode;
        public int resolutionSizeIndex;
        public bool vSync;
    }
}