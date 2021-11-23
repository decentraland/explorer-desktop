using UnityEngine;
using MainScripts.DCL.Controllers.HUD.Preloading;
using MainScripts.DCL.Controllers.LoadingFlow;
using MainScripts.DCL.Utils;

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
                DesktopUtils.Quit();
            }
            
            base.Update();
        }

        protected override void Start()
        {
            LoadingFlowController.Initialize();
            base.Start();
        }

        protected override void InitializeSceneDependencies()
        {
            base.InitializeSceneDependencies();

            PreloadingController.Initialize();
        }
    }
}
