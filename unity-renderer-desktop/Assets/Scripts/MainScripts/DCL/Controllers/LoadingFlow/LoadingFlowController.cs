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
        private Dictionary<string, IParcelScene> loadingScenes = new Dictionary<string, IParcelScene>();
        private List<string> failedUrls = new List<string>();
        private static ILoadingFlowView view;

        public static LoadingFlowController Initialize()
        {
            return new LoadingFlowController();
        }

        private static ILoadingFlowView CreateView()
        {
            return Object.Instantiate(Resources.Load<LoadingFlowView>("LoadingFlow"));
        }

        private LoadingFlowController()
        {
            DataStore.i.isPlayerRendererLoaded.OnChange += OnRendererLoaded;
            Environment.i.world.sceneController.OnNewSceneAdded += OnNewSceneAdded;
            Environment.i.world.sceneController.OnReadyScene += OnReadyScene;
            WebRequestController.OnWebRequestFailed += OnWebRequestFail;
            
            view = CreateView();
            view.SetVisible(false);
        }

        public void Dispose()
        {
            DataStore.i.isPlayerRendererLoaded.OnChange -= OnRendererLoaded;
            Environment.i.world.sceneController.OnNewSceneAdded -= OnNewSceneAdded;
            Environment.i.world.sceneController.OnReadyScene -= OnReadyScene;
            WebRequestController.OnWebRequestFailed -= OnWebRequestFail;
        }

        private void OnWebRequestFail(string url)
        {
            failedUrls.Add(url);
        }

        private void OnReadyScene(string sceneId)
        {
            if (loadingScenes.ContainsKey(sceneId))
                loadingScenes.Remove(sceneId);
        }

        private void OnNewSceneAdded(IParcelScene parcelScene)
        {
            PrettyLog("Loading parcel scene: " + parcelScene.sceneData.id);
            loadingScenes[parcelScene.sceneData.id] = parcelScene;
        }

        private void OnRendererLoaded(bool current, bool previous)
        {
            PrettyLog("PLAYER RENDERER LOADED: " + current);
            if (loadingScenes.Count > 0)
            {
                PrettyLog($"{loadingScenes.Count} scenes are still loading");

                foreach (var parcelSceneKvp in loadingScenes)
                {
                    var parcelScene = parcelSceneKvp.Value;
                    PrettyLog($"id:{parcelScene.sceneData.id} progress: {parcelScene.loadingProgress}");
                }
                PrettyLog(WebRequestControllerDesktop.GetOngoingWebRequestCount() + " webrequests remain");
                if (WebRequestControllerDesktop.GetOngoingWebRequestCount() == 0)
                {
                    view.SetVisible(true);
                    Debug.LogError("There is no ongoing web request, show popup");
                } else 
                if (failedUrls.Count > 0)
                {
                    view.SetVisible(true);
                    PrettyLogRed("URLS FAILED WHILE LOADING: ");
                    foreach (string failedUrl in failedUrls)
                    {
                        PrettyLogRed(failedUrl);
                    }
                }
            }
            
            Dispose();
        }

        private void PrettyLog(string message)
        {
            Debug.Log($"<b><color=Green>{message}</color></b>");
        }
        
        private void PrettyLogRed(string message)
        {
            Debug.LogError($"<b><color=Red>{message}</color></b>");
        }
    }
}