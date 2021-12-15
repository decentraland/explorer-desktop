using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainScripts.DCL.Controllers.LoadingFlow
{
    public class LoadingFlowController : IDisposable
    {
        private const float GENERAL_TIMEOUT_IN_SECONDS = 100;

        private readonly BaseVariable<Exception> fatalErrorMessage;
        private readonly ILoadingFlowView view;
        private float timerStart;
        private bool isDisposed;

        public LoadingFlowController(Action reloadAction,
            BaseVariable<Exception> fatalErrorMessage,
            BaseVariable<bool> loadingHudVisible)
        {
            this.fatalErrorMessage = fatalErrorMessage;

            view = CreateView();
            view.Setup(reloadAction);
            view.Hide();

            loadingHudVisible.OnChange += OnLoadingHudVisibleChanged;
            CommonScriptableObjects.rendererState.OnChange += OnRendererStateChange;
        }

        private ILoadingFlowView CreateView()
        {
            return Object.Instantiate(Resources.Load<LoadingFlowView>("LoadingFlow"));
        }

        private void OnLoadingHudVisibleChanged(bool current, bool previous)
        {
            if (current)
            {
                StartWatching();
            }
            else
            {
                view.Hide();
                Dispose();
            }
        }

        private void StartWatching()
        {
            isDisposed = false;
            timerStart = Time.unscaledTime;
            fatalErrorMessage.OnChange += HandleFatalError;
        }

        private void HandleFatalError(Exception current, Exception previous)
        {
            if (current == null) return;
            view.ShowForError();
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            fatalErrorMessage.OnChange -= HandleFatalError;
        }

        public void Update()
        {
            if (isDisposed) return;

            if (IsTimeToShowTimeout())
            {
                view.ShowForTimeout();
                Dispose();
            }
        }

        private bool IsTimeToShowTimeout()
        {
            return Time.unscaledTime - timerStart > GENERAL_TIMEOUT_IN_SECONDS;
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