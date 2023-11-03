using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureScreenShot : MonoBehaviour {


    int i = 0;

    private void OnEnable()
    {
	    InputController.screenshot += OnScreenShotButtonPressed;
    }
    // Update is called once per frame
	/*void Update () {
        if(Input.GetKey(KeyCode.C)){
            ScreenCapture.CaptureScreenshot("Demo" + i+".jpeg");
                       i++;         
        }
	}*/

	private void OnScreenShotButtonPressed()
	{
		Debug.Log("<color=green>ScreenShot Captured</color>");
		ScreenCapture.CaptureScreenshot("Demo" + i+".jpeg");
		i++;
	}

	private void OnDisable()
	{
		InputController.screenshot -= OnScreenShotButtonPressed;
	}
}
