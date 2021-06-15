using System.Collections;
using System.Collections.Generic;
using Launcher;
using UnityEngine;

public class LauncherSceneController : MonoBehaviour
{
    private LauncherHUDController launcherHUDController;
    // Start is called before the first frame update
    void Start()
    {
        launcherHUDController = new LauncherHUDController();
        launcherHUDController.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
