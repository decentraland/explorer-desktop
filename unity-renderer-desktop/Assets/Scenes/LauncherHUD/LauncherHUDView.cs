using System;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Launcher
{
    public interface ILauncherHUDView : IDisposable
    {
        event Action OnPlay;
        event Action OnPlayAsGuest;

        void SetVisibility(bool visible);

        void SetLoading(bool loading);
    }

    public class LauncherHUDView : MonoBehaviour, ILauncherHUDView
    {
        public event Action OnPlay;
        public event Action OnPlayAsGuest;
        
        [SerializeField] internal Button playButton;
        [SerializeField] internal Button playAsGuestButton;
        [SerializeField] internal GameObject signinPanel;
        [SerializeField] internal GameObject loadingPanel;
        private void Awake()
        {
            playButton.onClick.AddListener(() => OnPlay?.Invoke());
            playAsGuestButton.onClick.AddListener(() => OnPlayAsGuest?.Invoke());
        }
        
        public static LauncherHUDView CreateView()
        {
            LauncherHUDView view = Instantiate(Resources.Load<GameObject>("LauncherHUD")).GetComponent<LauncherHUDView>();
            view.gameObject.name = "_Launcher";
            return view;
        }

        public void SetVisibility(bool visible) { gameObject.SetActive(visible); }

        public void SetLoading(bool loading)
        {
            signinPanel.SetActive(!loading);
            loadingPanel.SetActive(loading);
        }

        public void Dispose()
        {
            if (this != null)
                Destroy(gameObject);
        }
    }
}