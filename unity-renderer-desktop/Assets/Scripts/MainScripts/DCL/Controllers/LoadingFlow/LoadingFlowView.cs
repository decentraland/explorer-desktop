using System;
using MainScripts.DCL.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace MainScripts.DCL.Controllers.LoadingFlow
{
    public class LoadingFlowView : MonoBehaviour, ILoadingFlowView
    {
        [SerializeField] private Button exitButton;
        [SerializeField] private GameObject timeoutContainer;
        [SerializeField] private GameObject errorContainer;

        public void Setup()
        {
            timeoutContainer.gameObject.SetActive(false);
            errorContainer.gameObject.SetActive(false);
        }

        private void Awake()
        {
            exitButton.onClick.AddListener(OnExit);
        }

        private void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
        }

        private void OnExit()
        {
            DesktopUtils.Quit();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowForError()
        {
            gameObject.SetActive(true);
            errorContainer.SetActive(true);
        }
        public void ShowForTimeout()
        {
            gameObject.SetActive(true);
            timeoutContainer.SetActive(true);
        }
    }

    public interface ILoadingFlowView
    {
        void Hide();
        void ShowForError();
        void ShowForTimeout();
        void Setup();
    }
}