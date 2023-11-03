using System.Collections;
using System.Collections.Generic;
using Game.BaseFramework;
using UnityEngine;
using UnityEngine.InputSystem;

public class GS : Singleton<GS>
{
    #region PUBLIC_VARS

    public int StartingLife;
    public int StartingHelth;
    public float CameraFollowSpeed;  // 3
    public float CamaraRotetionSpeed; // 8
    public bool isFireBallAttack;

    public float xFollowSpeed;
    public float yFollowSpeed;
    public float zFollowSpeed;
    
    public float xRotetionSpeed;
    public float yRotetionSpeed;
    public float zRotetionSpeed;
    
    public float YOfset;
    public float ZOfset;
    
    public float XRotetionOfset;
    public float YRotetionOfset;

    public float RunMul;
    public float StayMul;


    public float cocoHeight;
    public InputController input;
    public PlayerInput playerInput;
    public InputActionReference fastForwardAnimationInput;
    public InputActionReference levelSkipInput;
    public bool isPaused = false;

    //[HideInInspector] 
    public float backgroundSoundVolume = 0.5f;
    //[HideInInspector] 
    public float sfxSoundVolume = 0.5f;
    
    
    public delegate void OnControllerSwitchDelegate(int index);
    public static event OnControllerSwitchDelegate onControllerSwitch; 

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    #endregion

    #region PUBLIC_FUNCTIONS

    public void OnGamePause()
    {
        if (!isPaused)
        {
            isPaused = true;
        }
    }


    public void OnGameUnPause()
    {
        if (isPaused)
        {
            isPaused = false;
        }
    }

    public bool IsGamePaused()
    {
        return isPaused;
    }
    public void OnGameControllerChanged(int index)
    {
        onControllerSwitch?.Invoke(index);
    }
    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINES

    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}
