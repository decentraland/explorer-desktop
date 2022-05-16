using System;
using System.Collections.Generic;
using System.Linq;
using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using MainScripts.DCL.ScriptableObjectsDesktop;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/Resolution Size",
        fileName = "ResolutionSizeControlController")]
    public class ResolutionSizeControlController : SpinBoxSettingsControlControllerDesktop
    {
        //private Resolution[] availableFilteredResolutions;
        private List<ValueTuple<int,int>> possibleResolutions = new List<ValueTuple<int, int>>();
        private int maxFramerate = 60;

        public override void Initialize()
        {
            SetupAvailableResolutions();
            base.Initialize();
            SetupLabels();
        }

        // Filter the smallest resolutions as no one will ever use them
        private void SetupAvailableResolutions()
        {
            for (int i = 0; i < Screen.resolutions.Count(); i++)
            {
                if (Screen.resolutions[i].width > 1024)
                {
                    if (Screen.resolutions[i].refreshRate > maxFramerate)
                        maxFramerate = Screen.resolutions[i].refreshRate;

                    if (!possibleResolutions.Contains((Screen.resolutions[i].width, Screen.resolutions[i].height)))
                        possibleResolutions.Add(new ValueTuple<int, int>(Screen.resolutions[i].width, Screen.resolutions[i].height));
                }

            }
        }

        private void SetupLabels()
        {
            var length = possibleResolutions.Count;
            var resolutionLabels = new string[length];
            for (var i = 0; i < length; i++)
            {
                (int, int) resolution = possibleResolutions[i];
                
                // by design we want the list to be inverted so the biggest resolutions stay on top
                // our resolutionSizeIndex is based on this decision
                resolutionLabels[length - 1 - i] = GetLabel(resolution);
            }

            RaiseOnOverrideIndicatorLabel(resolutionLabels);
        }

        private static string GetLabel(ValueTuple<int,int> resolution)
        {
            return $"{resolution.Item1}x{resolution.Item2} {GetAspectRatio(resolution.Item1, resolution.Item2)}";
        }

        public override object GetStoredValue()
        {
            return currentDisplaySettings.resolutionSizeIndex;
        }

        private static string GetAspectRatio(int width, int height)
        {
            int rest;
            int tempWidth = width;
            int tempHeight = height;
            while (height != 0)
            {
                rest = width % height;
                width = height;
                height = rest;
            }
            return (tempWidth / width).ToString() + ":" + (tempHeight / width).ToString();
        }

        public override void UpdateSetting(object newValue)
        {
            var value = (int)newValue;
            currentDisplaySettings.resolutionSizeIndex = value;
            var currentResolution = possibleResolutions[possibleResolutions.Count - 1 - value];
            Screen.SetResolution(currentResolution.Item1, currentResolution.Item2, Screen.fullScreenMode, maxFramerate);
        }
    }
}