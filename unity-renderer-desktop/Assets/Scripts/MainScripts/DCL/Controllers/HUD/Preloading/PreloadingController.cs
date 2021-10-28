using DCL;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.Preloading
{
    public class PreloadingController
    {
        private GameObject view;
        private BaseVariable<string> loadingMessage => DataStore.i.HUDs.loadingHUD.message;
        private BaseVariable<bool> isSignUpFlow => DataStore.i.isSignUpFlow;

        public void Initialize()
        {
            view = Object.Instantiate(GetView());
            loadingMessage.OnChange += OnMessageChange;
            isSignUpFlow.OnChange += SignUpFlowChanged;
        }

        private GameObject GetView()
        {
            return Resources.Load<GameObject>("Preloading");
        }

        private void Dispose()
        {
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