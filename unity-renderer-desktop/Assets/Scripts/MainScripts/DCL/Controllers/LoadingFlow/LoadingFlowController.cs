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
        private bool isWatching;
        private BaseVariable<bool> loadingHudVisible;
        private RendererState rendererState;

        public LoadingFlowController(Action reloadAction,
            BaseVariable<Exception> fatalErrorMessage,
            BaseVariable<bool> loadingHudVisible, 
            RendererState rendererState)
        {
            this.fatalErrorMessage = fatalErrorMessage;
            this.loadingHudVisible = loadingHudVisible;
            this.rendererState = rendererState;

            view = CreateView();
            view.Setup(reloadAction);
            view.Hide();

            this.loadingHudVisible.OnChange += OnLoadingHudVisibleChanged;
            this.rendererState.OnChange += OnRendererStateChange;
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
                StopWatching();
            }
        }

        private void StartWatching()
        {
            isWatching = false;
            timerStart = Time.unscaledTime;
            fatalErrorMessage.OnChange += HandleFatalError;
        }

        private void HandleFatalError(Exception current, Exception previous)
        {
            if (current == null) return;
            view.ShowForError();
            StopWatching();
        }

        public void StopWatching()
        {
            if (isWatching) return;
            isWatching = true;
            fatalErrorMessage.OnChange -= HandleFatalError;
        }

        public void Update()
        {
            if (isWatching) return;

            if (IsTimeToShowTimeout())
            {
                view.ShowForTimeout();
                StopWatching();
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
                StopWatching();
            }
        }

        public void Dispose()
        {
            fatalErrorMessage.OnChange -= HandleFatalError;
            loadingHudVisible.OnChange -= OnLoadingHudVisibleChanged;
            rendererState.OnChange -= OnRendererStateChange;
        }
    }
}