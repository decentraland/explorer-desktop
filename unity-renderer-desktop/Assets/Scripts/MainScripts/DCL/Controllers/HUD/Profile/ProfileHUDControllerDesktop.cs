using MainScripts.DCL.Utils;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.Profile
{
    public class ProfileHUDControllerDesktop : ProfileHUDController
    {
        private ProfileHUDViewDesktop viewDesktop;
        public ProfileHUDControllerDesktop(IUserProfileBridge userProfileBridge) : base(userProfileBridge)
        {
            viewDesktop = (ProfileHUDViewDesktop)view;
            viewDesktop.getButtonSignUp.onClick.RemoveAllListeners();
            viewDesktop.getButtonSignUp.onClick.AddListener(OnExitButtonClick); // When you exit the renderer, you will see the launcher where you can sign up
            viewDesktop.buttonExit.onClick.AddListener(OnExitButtonClick);
        }

        private void OnExitButtonClick()
        {
            DesktopUtils.Quit();
        }

        protected override GameObject GetViewPrefab()
        {
            return Resources.Load<GameObject>("ProfileHUDDesktop");
        }
    }
}