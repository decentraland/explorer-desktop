using System;
using System.Linq;
using DCL.SettingsCommon;
using DCL.Components;
using HTC.UnityPlugin.Multimedia;
using MainScripts.DCL.Controllers.HUD.Preloading;
using MainScripts.DCL.Controllers.LoadingFlow;
using MainScripts.DCL.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DCL
{
    /// <summary>
    /// This is the MainDesktop entry point.
    /// Most of the application subsystems should be initialized from this class Awake() event.
    /// </summary>
    public class MainDesktop : Main
    {
        private LoadingFlowController loadingFlowController;
        private PreloadingController preloadingController;
        private bool isConnectionLost;

        protected override void Awake()
        {
            CommandLineParserUtils.ParseArguments();
            isConnectionLost = false;
/*
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            FFMPEGDecoderWrapper.nativeCleanAll();
            DCLVideoTexture.videoPluginWrapperBuilder = () => new VideoPluginWrapper_FFMPEG();
#endif
*/
            InitializeSettings();

            base.Awake();
            DataStore.i.wsCommunication.communicationEstablished.OnChange += OnCommunicationEstablished;
            
            DataStore.i.multithreading.enabled.Set(true);

            CheckForIncorrectScreenSize();
        }
        

        protected override void InitializeCommunication()
        {
            // TODO(Brian): Remove this branching once we finish migrating all tests out of the
            //              IntegrationTestSuite_Legacy base class.
            if (!Configuration.EnvironmentSettings.RUNNING_TESTS)
            {
                var withSSL = true;
                int startPort = CommandLineParserUtils.startPort;
                
                #if UNITY_EDITOR
                withSSL = DebugConfigComponent.i.webSocketSSL;
                startPort = 5000;
                #endif
                
                int endPort = startPort + 100;
                kernelCommunication = new WebSocketCommunication(withSSL, startPort, endPort);
            }
            
        }

        private void CheckForIncorrectScreenSize()
        {
            var width = Screen.currentResolution.width;
            var height = Screen.currentResolution.height;
            var availableFilteredResolutions = Screen.resolutions.Where(r => r.width >= 1024 && r.refreshRate > 0).ToArray();
            var minRes = availableFilteredResolutions[0];

            if (width < 800 || height < 600)
            {
                Screen.SetResolution(minRes.width, minRes.height, Screen.fullScreenMode);
            }
            //DataStore.i.multithreading.enabled.Set(true);
        }

        private void InitializeSettings()
        {
            Settings.CreateSharedInstance(new DefaultSettingsFactory()
                .WithGraphicsQualitySettingsPresetPath("DesktopGraphicsQualityPresets"));
        }

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
                DesktopDestroy();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void DesktopDestroy()
        {
            loadingFlowController.Dispose();
            preloadingController.Dispose();
            DataStore.i.wsCommunication.communicationEstablished.OnChange -= OnCommunicationEstablished;
            /*
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            FFMPEGDecoderWrapper.nativeCleanAll();
#endif*/
        }

        void OnCommunicationEstablished(bool current, bool previous)
        {
            if (current == false && previous)
            {
                isConnectionLost = true;
            }
        }

        protected override void Update()
        {
            base.Update();
            loadingFlowController.Update();

            if (isConnectionLost)
            {
                DesktopUtils.Quit();
            }
        }

        protected override void Start()
        {
            loadingFlowController = new LoadingFlowController(
                DataStore.i.HUDs.loadingHUD.fatalError,
                DataStore.i.HUDs.loadingHUD.visible,
                CommonScriptableObjects.rendererState,
                DataStore.i.wsCommunication.communicationEstablished);
            base.Start();
        }

        protected override void InitializeSceneDependencies()
        {
            base.InitializeSceneDependencies();
            preloadingController = new PreloadingController();
        }

        protected override void SetupServices()
        {
            Environment.Setup(ServiceLocatorDesktopFactory.CreateDefault());
        }
    }
}