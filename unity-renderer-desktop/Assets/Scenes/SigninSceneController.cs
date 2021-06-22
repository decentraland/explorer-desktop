using System.Collections;
using System.Collections.Generic;
using Signin;
using UnityEngine;

public class SigninSceneController : MonoBehaviour
{
    private SigninHUDController _signinHUDController;
    // Start is called before the first frame update
    void Start()
    {
        _signinHUDController = new SigninHUDController();
        _signinHUDController.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
