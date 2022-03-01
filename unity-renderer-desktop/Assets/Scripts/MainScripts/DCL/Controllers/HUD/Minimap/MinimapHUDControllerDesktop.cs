using UnityEngine;

public class MinimapHUDControllerDesktop : MinimapHUDController
{
    protected override MinimapHUDView CreateView()
    {
        return MinimapHUDViewDesktop.Create(this);
    }
}