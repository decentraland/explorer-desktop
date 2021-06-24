using System;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Signin
{
    public interface ILoginHUDView : IDisposable
    {
        event Action OnPlay;
        event Action OnPlayAsGuest;

        void SetVisibility(bool visible);

        void SetLoading(bool loading);
    }

    public class LoginHUDView : MonoBehaviour, ILoginHUDView
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
        
        public static LoginHUDView CreateView()
        {
            LoginHUDView view = Instantiate(Resources.Load<GameObject>("SigninHUD")).GetComponent<LoginHUDView>();
            view.gameObject.name = "_Signin";
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