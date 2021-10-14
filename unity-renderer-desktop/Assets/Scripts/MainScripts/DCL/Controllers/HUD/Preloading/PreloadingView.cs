using System;
using DCL;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.Preloading
{
    public class PreloadingView : MonoBehaviour
    {
        //TODO: dont let this hack go to production, its just a test
        internal BaseVariable<string> message => DataStore.i.HUDs.loadingHUD.message;

        private void Start()
        {
            gameObject.SetActive(true);
            message.OnChange += OnMessageChange;
        }

        private void OnDestroy()
        {
            message.OnChange -= OnMessageChange;
        }

        private void OnMessageChange(string current, string previous)
        {
            if (current.Contains("%"))
            {
                Destroy(gameObject);
            }
        }
    }
}
