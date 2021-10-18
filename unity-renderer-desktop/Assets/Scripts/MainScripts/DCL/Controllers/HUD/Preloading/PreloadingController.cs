using DCL;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.Preloading
{
    public class PreloadingController
    {
        private GameObject view;
        private BaseVariable<string> loadingMessage => DataStore.i.HUDs.loadingHUD.message;

        public void Initialize()
        {
            view = Object.Instantiate(GetView());
            loadingMessage.OnChange += OnMessageChange;
        }

        private GameObject GetView()
        {
            return Resources.Load<GameObject>("Preloading");
        }

        private void Dispose()
        {
            loadingMessage.OnChange -= OnMessageChange;
            Object.Destroy(view.gameObject);
        }

        private void OnMessageChange(string current, string previous)
        {
            if (current.Contains("%"))
            {
                Dispose();
            }
        }
    }
}