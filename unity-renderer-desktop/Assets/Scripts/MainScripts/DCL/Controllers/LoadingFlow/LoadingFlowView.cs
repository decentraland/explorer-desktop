using System;
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
    }
}