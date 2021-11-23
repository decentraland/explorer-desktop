using System;
using MainScripts.DCL.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainScripts.DCL.Controllers.LoadingFlow
{
    public class LoadingFlowView : MonoBehaviour, ILoadingFlowView
    {
        [SerializeField] private Button exitButton;
        [SerializeField] private Button retryButton;

        private void Awake()
        {
            exitButton.onClick.AddListener(OnExit);
            retryButton.onClick.AddListener(OnRetry);
        }

        private void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
            retryButton.onClick.RemoveListener(OnRetry);
        }

        private void OnExit()
        {
            DesktopUtils.Quit();
        }

        private void OnRetry()
        {
            SceneManager.LoadScene(0);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }

    public interface ILoadingFlowView
    {
        void SetVisible(bool visible);
    }
}