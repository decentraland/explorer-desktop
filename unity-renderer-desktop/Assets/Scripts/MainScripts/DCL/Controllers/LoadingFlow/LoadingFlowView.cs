using System;
using DCL;
using MainScripts.DCL.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainScripts.DCL.Controllers.LoadingFlow
{
    public class LoadingFlowView : MonoBehaviour, ILoadingFlowView
    {
        [SerializeField] private Button exitButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private GameObject timeoutContainer;
        [SerializeField] private GameObject errorContainer;
        private Action reloadAction;

        public void Setup(Action reloadAction)
        {
            this.reloadAction = reloadAction;
            timeoutContainer.gameObject.SetActive(false);
            errorContainer.gameObject.SetActive(false);
        }

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
            reloadAction?.Invoke();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowForError()
        {
            errorContainer.SetActive(true);
        }
        public void ShowForTimeout()
        {
            timeoutContainer.SetActive(true);
        }
    }

    public interface ILoadingFlowView
    {
        void Hide();
        void ShowForError();
        void ShowForTimeout();
        void Setup(Action reloadAction);
    }
}