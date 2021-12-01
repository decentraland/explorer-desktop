using System.Collections;
using bosqmode.libvlc;
using DCL.Components.Video.Plugin;
using UnityEngine;

namespace DCL.Components
{
    public class DCLVideoTextureDesktop : DCLVideoTexture
    {
        public override IEnumerator ApplyChanges(BaseModel newModel)
        {
            Debug.Log("Load DCLVideoTextureDesktop");
            return base.ApplyChanges(newModel);
        }
    }
}