using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StonemanAnimationTrigger : MonoBehaviour
{
    #region PUBLIC_VARS

    public Action PickPlayer;
    public Action ThrowPlayer;

    public Action leftHandVFX;
    public Action rightHandVFX;
    
    public Action playerPickVFXEvent;
    public Action punchAttackVFXEvent;
    public BoxCollider punchCollider;

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    #endregion

    #region PUBLIC_FUNCTIONS

    public void OnAnimationTriggerEvent1()
    {
        PickPlayer?.Invoke();
    }

    public void OnAnimationTriggerEvent2()
    {
        ThrowPlayer?.Invoke();
    }

    public void OnAngryVFXLeftHandEventTrigger()
    {
        leftHandVFX?.Invoke();
    }
    
    public void OnAngryVFXRightHandEventTrigger()
    {
        rightHandVFX?.Invoke();
    }

    public void OnPlayerPickVFXEventTrigger()
    {
        playerPickVFXEvent?.Invoke();
    }

    public void OnPunchAttackVFXEventTrigger()
    {
        punchAttackVFXEvent?.Invoke();
    }

    public void OnPunchAttackActiveTrigger()
    {
        punchCollider.enabled = true;
    }
    
    public void OnPunchAttackDeActiveTrigger()
    {
        punchCollider.enabled = false;
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
