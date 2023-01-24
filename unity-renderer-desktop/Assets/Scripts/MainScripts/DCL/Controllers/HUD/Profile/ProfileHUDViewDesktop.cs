using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainScripts.DCL.Controllers.HUD.Profile
{
    public class ProfileHUDViewDesktop : ProfileHUDViewV2
    {
        [SerializeField]
        protected internal List<Button> exitButtons;
        
        protected internal Button getButtonSignUp => buttonSignUp;
    }
}