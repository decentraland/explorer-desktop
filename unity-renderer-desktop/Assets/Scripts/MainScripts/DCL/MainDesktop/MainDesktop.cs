using UnityEngine;
using MainScripts.DCL.Controllers.HUD.Preloading;
using MainScripts.DCL.Controllers.LoadingFlow;
using MainScripts.DCL.Utils;
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

        protected override void Awake()
        {
            base.Awake();
            CommandLineParserUtils.ParseArguments();
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
            loadingFlowController.Dispose();
            preloadingController.Dispose();
        }

        protected override void Update()
        {
            base.Update();
            loadingFlowController.Update();
        }

        protected override void Start()
        {
            loadingFlowController = new LoadingFlowController(Reload);
            base.Start();
        }

        private void Reload()
        {
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
