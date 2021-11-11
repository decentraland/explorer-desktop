using UnityEngine;
using MainScripts.DCL.Controllers.HUD.Preloading;

namespace DCL
{
    /// <summary>
    /// This is the MainDesktop entry point.
    /// Most of the application subsystems should be initialized from this class Awake() event.
    /// </summary>
    public class MainDesktop : Main
    {
        private bool closeApp = false;
        protected override void Awake()
        {
            var preloading = new PreloadingController();
            preloading.Initialize();
            
            base.Awake();
            CommandLineParserUtils.ParseArguments();
            DataStore.i.wsCommunication.communicationEstablished.OnChange += OnCommunicationEstablished;
        }
        protected override HUDContext HUDContextBuilder()
        {
            return HUDDesktopContextFactory.CreateDefault();
        }

        protected override PlatformContext PlatformContextBuilder()
        {
            return PlatformDesktopContextFactory.CreateDefault();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DataStore.i.wsCommunication.communicationEstablished.OnChange -= OnCommunicationEstablished;
        }

        void OnCommunicationEstablished(bool current, bool previous)
        {
            if (current == false && previous)
            {
                closeApp = true;
            }
        }

        protected override void Update()
        {
            if (closeApp)
            {
                closeApp = false;
#if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
            
            base.Update();
        }
    }
}
