using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimationTrigger : MonoBehaviour
{
    #region PUBLIC_VARS

    public Action OnAnimationEvenTrigger1;
    public Action OnAnimationEvenTrigger2;
    public Action earthShatterTrigger;
    public Action trailAttackTrigger;
    public Action groundAttackTrigger;
    public Action groundAttackTriggerStop;

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    #endregion

    #region PUBLIC_FUNCTIONS

    public void OnAnimationTriggerEvent1()
    {
        OnAnimationEvenTrigger1?.Invoke();
    }

    public void OnAnimationTriggerEvent2()
    {
        OnAnimationEvenTrigger2?.Invoke();
    }
    
    public void EarthShatterEffect()
    {
        earthShatterTrigger?.Invoke();
    }
    
    public void TrailAttackEffect()
    {
        trailAttackTrigger?.Invoke();
    }

    public void FireBreathVFX()
    {
        groundAttackTrigger?.Invoke();
    }

    public void FireBreathVFXStop()
    {
        groundAttackTriggerStop?.Invoke();
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
