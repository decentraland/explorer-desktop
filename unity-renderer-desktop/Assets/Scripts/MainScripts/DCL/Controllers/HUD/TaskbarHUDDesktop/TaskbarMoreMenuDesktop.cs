using System.Collections;
using System.Collections.Generic;
using MainScripts.DCL.Utils;
using UnityEngine;

public class TaskbarMoreMenuDesktop : TaskbarMoreMenu
{
    [SerializeField] internal TaskbarMoreMenuButton exitButton;

    public override void Initialize(TaskbarHUDView view)
    {
        base.Initialize(view);
        
        sortedButtonsAnimations.Insert(0, exitButton);
        
        exitButton.gameObject.SetActive(true);
        
        exitButton.mainButton.onClick.AddListener(() =>
        {
            DesktopUtils.Quit();
        });
    }
}
