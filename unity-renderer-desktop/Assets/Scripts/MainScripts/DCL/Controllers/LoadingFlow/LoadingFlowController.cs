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
        private const float FAIL_TIMEOUT_IN_SECONDS = 20;
        
        private Dictionary<string, IParcelScene> loadingScenes = new Dictionary<string, IParcelScene>();
        private List<string> failedUrls = new List<string>();
        private static ILoadingFlowView view;
        private float? shortTimerStart;
        private float longTimerStart;
        private bool isDisposed = false;

        public LoadingFlowController(Action reloadAction)
        {
            Environment.i.world.sceneController.OnNewSceneAdded += OnNewSceneAdded;
            Environment.i.world.sceneController.OnReadyScene += OnReadyScene;
            WebRequestController.OnWebRequestFailed += OnWebRequestFail;
            CommonScriptableObjects.rendererState.OnChange += OnRendererStateChange;
            
            view = CreateView();
            view.Setup(reloadAction);
            view.Hide();
            longTimerStart = Time.unscaledTime;
        }

        private ILoadingFlowView CreateView()
        {
            return Object.Instantiate(Resources.Load<LoadingFlowView>("LoadingFlow"));
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            Environment.i.world.sceneController.OnNewSceneAdded -= OnNewSceneAdded;
            Environment.i.world.sceneController.OnReadyScene -= OnReadyScene;
            WebRequestController.OnWebRequestFailed -= OnWebRequestFail;
            CommonScriptableObjects.rendererState.OnChange -= OnRendererStateChange;
        }

        public void Update()
        {
            if (isDisposed) return;

            if (Time.unscaledTime - longTimerStart > GENERAL_TIMEOUT_IN_SECONDS)
            {
                view.ShowForTimeout();
                Dispose();
                return;
            }
            if (shortTimerStart.HasValue)
            {
                if (Time.unscaledTime - shortTimerStart.Value > FAIL_TIMEOUT_IN_SECONDS)
                {
                    view.ShowForError();
                    Dispose();
                }
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

        private void OnWebRequestFail(string url)
        {
            shortTimerStart = Time.unscaledTime;
            failedUrls.Add(url);
        }

        private void OnReadyScene(string sceneId)
        {
            if (loadingScenes.ContainsKey(sceneId))
                loadingScenes.Remove(sceneId);
        }

        private void OnNewSceneAdded(IParcelScene parcelScene)
        {
            loadingScenes[parcelScene.sceneData.id] = parcelScene;
        }
    }
}