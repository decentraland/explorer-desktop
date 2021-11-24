using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainScripts.DCL.Controllers.LoadingFlow
{
    public class LoadingFlowController : IDisposable
    {
        private const float GENERAL_TIMEOUT_IN_SECONDS = 100;

        private readonly float timerStart;
        private readonly ILoadingFlowView view;
        private bool isDisposed;

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