using UnityEngine;
using UnityEngine.UI;

namespace MainScripts.DCL.Controllers.HUD.Profile
{
    public class ProfileHUDViewDesktop : ProfileHUDView
    {
        [SerializeField]
        protected internal Button buttonExit;
        
        protected internal Button getButtonSignUp => buttonSignUp;
    }
}