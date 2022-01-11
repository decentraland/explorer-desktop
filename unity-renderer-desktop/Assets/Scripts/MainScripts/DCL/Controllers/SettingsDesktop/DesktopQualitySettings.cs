using System;
using UnityEngine;

namespace DCL
{
    [Serializable]
    public struct DesktopQualitySettings
    {
        [Tooltip("Enable depth of field post process (blurry objects on distance)")]
        public bool depthOfField;
    }
}