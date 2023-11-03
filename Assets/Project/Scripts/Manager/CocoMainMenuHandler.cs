using System.Collections;
using System.Collections.Generic;
using Game.BaseFramework;
using UnityEngine;

public class CocoMainMenuHandler : Singleton<CocoMainMenuHandler>
{
    public CocoMainMenu mainMenuCoco;
    public float defaultCocoRotation;

    public void ResetCocoRotation()
    {
        mainMenuCoco.transform.eulerAngles = new Vector3(0,defaultCocoRotation,0);
    }
}
