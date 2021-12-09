using MainScripts.DCL.Utils;
using UnityEngine;

namespace MainScripts.DCL.Controllers.HUD.Profile
{
    public class ProfileHUDControllerDesktop : ProfileHUDController, IHUD
    {
        private ProfileHUDViewDesktop viewDesktop;
        public ProfileHUDControllerDesktop(IUserProfileBridge userProfileBridge) : base(userProfileBridge)
        {
            viewDesktop = view as ProfileHUDViewDesktop;
            viewDesktop.buttonExit.onClick.AddListener(DesktopUtils.Quit);
        }

        protected override GameObject GetViewPrefab()
        {
            return Resources.Load<GameObject>("ProfileHUDDesktop");
        }
    }
}