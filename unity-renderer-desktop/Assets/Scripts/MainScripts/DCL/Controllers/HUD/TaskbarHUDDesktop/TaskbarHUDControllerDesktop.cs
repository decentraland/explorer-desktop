using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskbarHUDControllerDesktop : TaskbarHUDController
{
    protected override TaskbarHUDView CreateView()
    {
        return TaskbarHUDViewDesktop.Create(this, chatController, friendsController);
    }
}