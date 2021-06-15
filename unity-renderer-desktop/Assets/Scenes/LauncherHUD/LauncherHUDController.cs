using System.Collections;
using DCL;
using DCL.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Launcher
{
    public class LauncherHUDController
    {
        internal ILauncherHUDView view;
        internal virtual ILauncherHUDView CreateView() => LauncherHUDView.CreateView();

        private AsyncOperation progress;
        public void Initialize() // TODO: Start -> Initialize
        {
            view = CreateView();
            if (view == null)
                return;
            
            view.OnPlay += OnPlay;
            view.OnPlayAsGuest += OnPlayAsGuest;
        }

        private void OnPlay()
        {
            DCLWebSocketService.enterAsAGuest = true;
            ChangeMainScene();
        }
        private void OnPlayAsGuest()
        {
            DCLWebSocketService.enterAsAGuest = true;
            ChangeMainScene();
        }

        private void ChangeMainScene()
        {
            view.SetLoading(true);
            progress = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
        }

        public void OnDestroy()
        {
            if (view == null)
                return;
            view.OnPlay -= OnPlay;
            view.OnPlayAsGuest -= OnPlayAsGuest;
            view.Dispose();
        }
    }
}