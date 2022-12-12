using DCL;
using DCL.Components.Video.Plugin;
using UnityEngine;

public class VideoProviderFactory 
{

    public static IVideoPluginWrapper CreateVideoProvider()
    {
#if AV_PRO_PRESENT
        if (Application.platform != RuntimePlatform.LinuxPlayer)
        {
            return new VideoPluginWrapper_AVPro();
        }
#endif
        return new VideoPluginWrapper_Native();
    }
    
}
