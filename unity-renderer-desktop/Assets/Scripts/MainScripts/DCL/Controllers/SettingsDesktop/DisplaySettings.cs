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
    public class DisplaySettings : IEquatable<DisplaySettings>, ICloneable
    {
        public WindowMode windowMode;
        public int resolutionSizeIndex;
        public bool vSync;

        public bool Equals(DisplaySettings other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return windowMode == other.windowMode && resolutionSizeIndex == other.resolutionSizeIndex && vSync == other.vSync;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DisplaySettings) obj);
        }

        public object Clone() => MemberwiseClone();
    }
}