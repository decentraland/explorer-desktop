using System;
using System.Collections.Generic;
using DCL;
using DCL.Controllers;
using MainScripts.DCL.Controllers.WebRequest;
using UnityEngine;
using Environment = DCL.Environment;
using Object = UnityEngine.Object;

namespace MainScripts.DCL.Controllers.LoadingFlow
{
    public class LoadingFlowController : IDisposable
    {
        private const float GENERAL_TIMEOUT_IN_SECONDS = 100;
        
        private static ILoadingFlowView view;
        private float timerStart;
        private bool isDisposed = false;

        public LoadingFlowController(Action reloadAction)
        {
            CommonScriptableObjects.rendererState.OnChange += OnRendererStateChange;
            
            view = CreateView();
            view.Setup(reloadAction);
            view.Hide();
            timerStart = Time.unscaledTime;
        }

        private ILoadingFlowView CreateView()
        {
            return Object.Instantiate(Resources.Load<LoadingFlowView>("LoadingFlow"));
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            CommonScriptableObjects.rendererState.OnChange -= OnRendererStateChange;
        }

        public void Update()
        {
            if (isDisposed) return;

            if (Time.unscaledTime - timerStart > GENERAL_TIMEOUT_IN_SECONDS)
            {
                view.ShowForTimeout();
                Dispose();
            }
        }

        private void OnRendererStateChange(bool current, bool previous)
        {
            if (current)
            {
                view.Hide();
                Dispose();
            }
        }
    }
}