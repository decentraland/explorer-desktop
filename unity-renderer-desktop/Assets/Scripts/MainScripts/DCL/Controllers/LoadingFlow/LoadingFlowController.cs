using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainScripts.DCL.Controllers.LoadingFlow
{
    public class LoadingFlowController : IDisposable
    {
        private const float GENERAL_TIMEOUT_IN_SECONDS = 100;

        private readonly BaseVariable<Exception> fatalErrorMessage;
        private readonly float timerStart;
        private readonly ILoadingFlowView view;
        private bool isDisposed;

        public LoadingFlowController(Action reloadAction,
            BaseVariable<Exception> fatalErrorMessage)
        {
            this.fatalErrorMessage = fatalErrorMessage;
            fatalErrorMessage.OnChange += HandleFatalError;
            CommonScriptableObjects.rendererState.OnChange += OnRendererStateChange;
            
            view = CreateView();
            view.Setup(reloadAction);
            view.Hide();
            timerStart = Time.unscaledTime;
        }

        private void HandleFatalError(Exception current, Exception previous)
        {
            if (current == null) return;
            view.ShowForError();
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
            fatalErrorMessage.OnChange -= HandleFatalError;
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