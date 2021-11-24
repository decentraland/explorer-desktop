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
        [SerializeField] private TextMeshProUGUI errorText;
        private Action reloadAction;

        public void Setup(Action reloadAction)
        {
            this.reloadAction = reloadAction;
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

        public void ShowWithMessage(string message)
        {
            gameObject.SetActive(true);
            errorText.text = message;
        }
    }

    public interface ILoadingFlowView
    {
        void Hide();
        void ShowWithMessage(string message);
        void Setup(Action reloadAction);
    }
}