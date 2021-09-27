using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    public class ResolutionLabelProvider : MonoBehaviour, ISpinBoxLabelProvider
    {
        private string[] resolutionCache;

        public string[] GetLabels()
        {
            if (resolutionCache == null)
            {
                var resolutions = Screen.resolutions;
                resolutionCache = new string[resolutions.Length];
                for (var i = 0; i < resolutions.Length; i++)
                {
                    Resolution resolution = resolutions[i];
                    resolutionCache[i] = $"{resolution.width}x{resolution.height}";
                }
            }

            return resolutionCache;
        }
    }
}