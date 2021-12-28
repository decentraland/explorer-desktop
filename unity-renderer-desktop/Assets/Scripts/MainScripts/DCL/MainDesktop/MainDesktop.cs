using System;
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
        private bool isRestarting;

        protected override void Awake()
        {
            isRestarting = false;
            isConnectionLost = false;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            FFMPEGDecoderWrapper.nativeCleanAll();
            DCLVideoTexture.videoPluginWrapperBuilder = () => new VideoPluginWrapper_FFMPEG();
#endif

            InitializeSettings();

            base.Awake();
            CommandLineParserUtils.ParseArguments();
            DataStore.i.wsCommunication.communicationEstablished.OnChange += OnCommunicationEstablished;
        }

        private void InitializeSettings()
        {
            Settings.CreateSharedInstance(new DefaultSettingsFactory()
                .WithGraphicsQualitySettingsPresetPath("DesktopGraphicsQualityPresets"));
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
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            FFMPEGDecoderWrapper.nativeCleanAll();
#endif
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

            if (isConnectionLost && !isRestarting)
            {
                DesktopUtils.Quit();
            }
        }

        protected override void Start()
        {
            loadingFlowController = new LoadingFlowController(
                Reload,
                DataStore.i.HUDs.loadingHUD.fatalError,
                DataStore.i.HUDs.loadingHUD.visible,
                CommonScriptableObjects.rendererState);
            base.Start();
        }

        private void Reload()
        {
            isRestarting = true;
            kernelCommunication.Dispose();
            SceneManager.LoadScene(0);
        }

        protected override void InitializeSceneDependencies()
        {
            base.InitializeSceneDependencies();
            preloadingController = new PreloadingController();
        }
    }
}