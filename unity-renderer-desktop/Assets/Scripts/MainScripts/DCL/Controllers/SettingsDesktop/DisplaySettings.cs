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
    public class DisplaySettings : ICloneable
    {
        public WindowMode windowMode;
        public int resolutionSizeIndex;
        public bool vSync;
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}