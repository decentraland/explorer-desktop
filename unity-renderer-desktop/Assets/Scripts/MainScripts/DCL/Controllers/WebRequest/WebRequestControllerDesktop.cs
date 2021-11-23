using DCL;

namespace MainScripts.DCL.Controllers.WebRequest
{
    public class WebRequestControllerDesktop : WebRequestController
    {
        public static int GetOngoingWebRequestCount()
        {
            return ongoingWebRequests.Count;
        }
    }
}