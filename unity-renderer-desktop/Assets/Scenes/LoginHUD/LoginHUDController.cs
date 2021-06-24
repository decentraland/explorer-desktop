using System.Collections;
using DCL;
using DCL.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Signin
{
    public class LoginHUDController
    {
        internal ILoginHUDView view;
        internal virtual ILoginHUDView CreateView() => LoginHUDView.CreateView();
        public void Initialize()
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
            SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
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