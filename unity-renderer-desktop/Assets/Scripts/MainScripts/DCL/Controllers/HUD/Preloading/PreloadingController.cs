using System;
using DCL;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainScripts.DCL.Controllers.HUD.Preloading
{
    public class PreloadingController : IDisposable
    {
        private GameObject view;
        private BaseVariable<string> loadingMessage => DataStore.i.HUDs.loadingHUD.message;
        private BaseVariable<bool> isSignUpFlow => DataStore.i.isSignUpFlow;
        private bool isDisposed = false;
        
        public PreloadingController()
        {
            view = Object.Instantiate(GetView());
            loadingMessage.OnChange += OnMessageChange;
            isSignUpFlow.OnChange += SignUpFlowChanged;
        }

        private GameObject GetView()
        {
            return Resources.Load<GameObject>("Preloading");
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            loadingMessage.OnChange -= OnMessageChange;
            isSignUpFlow.OnChange -= SignUpFlowChanged;
            Object.Destroy(view.gameObject);
        }

        private void OnMessageChange(string current, string previous)
        {
            if (current.Contains("%"))
            {
                Dispose();
            }
        }

        private void SignUpFlowChanged(bool current, bool previous)
        {
            if (current)
            {
                Dispose();
            }
        }
    }
}