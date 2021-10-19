using DCL;
using DCL.Models;
using MainScripts.DCL.Components.Video;

namespace MainScripts.DCL.WorldRuntime.RuntimeComponentFactoryDesktop
{
    public class RuntimeComponentFactoryDesktop : RuntimeComponentFactory
    {
        public RuntimeComponentFactoryDesktop(IPoolableComponentFactory poolableComponentFactory = null) : base(poolableComponentFactory)
        {
            builders[(int)CLASS_ID.VIDEO_TEXTURE] = BuildComponent<DCLVideoTextureDesktop>;
        }
    }
}