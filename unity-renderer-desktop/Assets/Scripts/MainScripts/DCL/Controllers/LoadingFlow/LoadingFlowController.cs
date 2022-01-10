using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainScripts.DCL.Controllers.LoadingFlow
{
    public class LoadingFlowController : IDisposable
    {
        private const float GENERAL_TIMEOUT_IN_SECONDS = 100;
        private const int WEB_SOCKET_TIMEOUT = 15;

        private readonly BaseVariable<Exception> fatalErrorMessage;
        private readonly ILoadingFlowView view;
        private readonly BaseVariable<bool> loadingHudVisible;
        private readonly RendererState rendererState;
        private readonly BaseVariable<bool> websocketCommunicationEstablished;
        private float timerStart;
        private bool isWatching;

        public LoadingFlowController(BaseVariable<Exception> fatalErrorMessage,
            BaseVariable<bool> loadingHudVisible, 
            RendererState rendererState,
            BaseVariable<bool> websocketCommunicationEstablished)
        {
            this.fatalErrorMessage = fatalErrorMessage;
            this.loadingHudVisible = loadingHudVisible;
            this.rendererState = rendererState;
            this.websocketCommunicationEstablished = websocketCommunicationEstablished;

            view = CreateView();
            view.Setup();
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

        private void StopWatching()
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
            var elapsedLoadingTime = Time.unscaledTime - timerStart;
            return elapsedLoadingTime > GENERAL_TIMEOUT_IN_SECONDS
                   || !websocketCommunicationEstablished.Get() && elapsedLoadingTime > WEB_SOCKET_TIMEOUT;
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