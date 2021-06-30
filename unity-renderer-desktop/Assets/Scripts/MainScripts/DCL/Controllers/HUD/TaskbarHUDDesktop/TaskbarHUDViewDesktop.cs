using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskbarHUDViewDesktop
{
    //const string VIEW_PATH = "TaskbarDesktop";

    internal static TaskbarHUDView Create(TaskbarHUDController controller, IChatController chatController,
        IFriendsController friendsController)
    {
        var view = Object.Instantiate(Resources.Load<GameObject>("TaskbarDesktop")).GetComponent<TaskbarHUDView>();
        view.Initialize(controller, chatController, friendsController);
        return view;
    }
}
