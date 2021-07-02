using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskbarMoreMenuDesktop : TaskbarMoreMenu
{
    [SerializeField] internal TaskbarMoreMenuButton exitButton;

    public override void Initialize(TaskbarHUDView view)
    {
        base.Initialize(view);
        
        sortedButtonsAnimations.Add(exitButton);
        
        exitButton.gameObject.SetActive(true);
        
        exitButton.mainButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
