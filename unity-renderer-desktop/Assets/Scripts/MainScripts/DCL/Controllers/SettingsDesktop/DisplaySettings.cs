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

        public FullScreenMode GetFullScreenMode()
        {
            return windowMode switch
            {
                WindowMode.Windowed => FullScreenMode.Windowed,
                WindowMode.Borderless => FullScreenMode.FullScreenWindow,
                WindowMode.FullScreen => FullScreenMode.ExclusiveFullScreen,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        // Resolution list goes from the smallest to the biggest, our index is inverted for usage reasons so 0 is the biggest resolution available
        public Resolution GetResolution()
        {
            return Screen.resolutions[Screen.resolutions.Length - 1 - resolutionSizeIndex];
        }
    }
}