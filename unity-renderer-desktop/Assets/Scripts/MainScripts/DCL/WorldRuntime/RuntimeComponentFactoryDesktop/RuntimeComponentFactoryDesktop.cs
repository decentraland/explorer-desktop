using DCL;
using DCL.Components;
using DCL.Models;

namespace DCL
{
    public class RuntimeComponentFactoryDesktop : RuntimeComponentFactory
    {
        public RuntimeComponentFactoryDesktop(IPoolableComponentFactory poolableComponentFactory = null) : base(poolableComponentFactory)
        {
            //builders[(int)CLASS_ID.VIDEO_TEXTURE] = BuildComponent<DCLVideoTextureDesktop>;
        }
    }
}