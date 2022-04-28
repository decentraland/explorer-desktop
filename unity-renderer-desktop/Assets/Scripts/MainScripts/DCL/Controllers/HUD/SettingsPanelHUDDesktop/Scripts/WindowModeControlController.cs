using System;
using Cysharp.Threading.Tasks;
using MainScripts.DCL.Controllers.SettingsDesktop;
using MainScripts.DCL.Controllers.SettingsDesktop.SettingsControllers;
using MainScripts.DCL.ScriptableObjectsDesktop;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.SettingsPanelHUDDesktop.Scripts
{
    [CreateAssetMenu(menuName = "Settings/Controllers/Controls/Window Mode",
        fileName = "WindowModeControlController")]
    public class WindowModeControlController : SpinBoxSettingsControlControllerDesktop
    {
        public override object GetStoredValue()
        {
            return (int)currentDisplaySettings.windowMode;
        }

        public override void UpdateSetting(object newValue)
        {
            currentDisplaySettings.windowMode = (WindowMode)(int)newValue;
            switch (currentDisplaySettings.windowMode)
            {
                case WindowMode.Windowed:
                    SetupWindowed().Forget();
                    break;
                case WindowMode.Borderless:
                    SetupBorderless().Forget();
                    break;
                case WindowMode.FullScreen:
                    SetupFullScreen().Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CommonScriptableObjectsDesktop.disableVSync.Set(currentDisplaySettings.windowMode == WindowMode.Windowed);
            CommonScriptableObjectsDesktop.disableScreenResolution.Set(currentDisplaySettings.windowMode == WindowMode.Borderless);
        }

        //NOTE(Kinerius): We have to wait a single frame between changing screen mode and resolution because one of them fails if done at the same frame for some reason
        private async UniTaskVoid SetupFullScreen()
        {
            Screen.fullScreen = true;
            await UniTask.WaitForEndOfFrame();
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            await UpdateResolution();
        }

        private async UniTaskVoid SetupWindowed()
        {
            Screen.fullScreen = false;
            await UniTask.WaitForEndOfFrame();
            Screen.fullScreenMode = FullScreenMode.Windowed;
            await UpdateResolution();
        }

        private async UniTaskVoid SetupBorderless()
        {
            currentDisplaySettings.resolutionSizeIndex = 0;
            ApplySettings();
            await UniTask.WaitForEndOfFrame();
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            await UpdateResolution();
        }

        //NOTE(Kinerius) this fixes a race condition when starting the application
        private async UniTask UpdateResolution()
        {
            await UniTask.WaitForEndOfFrame();
            Resolution resolution = currentDisplaySettings.GetResolution();
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }
    }
}