using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    public Action OnAnimationEvenTrigger1;
    public Action OnAnimationEvenTrigger2;
    public Action hippoReBounceVFXEventTrigger;
    public Action hippoRingAttackVFXEventTrigger;
    public Action hippoShockWaveVFXEventTrigger;
    public Action OnHippoArmadilloThrow;

    public void OnAnimationTriggerEvent1()
    {
        OnAnimationEvenTrigger1?.Invoke();
    }

    public void OnAnimationTriggerEvent2()
    {
        OnAnimationEvenTrigger2?.Invoke();
    }
    
    public void HippoReBounceVFX()
    {
        Debug.Log("----HippoReBounceVFX");
        hippoReBounceVFXEventTrigger?.Invoke();
    }
    
    public void HippoRingAttackVFX()
    {
        Debug.Log("----HippoRingAttackVFX");
        hippoRingAttackVFXEventTrigger?.Invoke();
    }
    
    public void HippoShockWaveVFX()
    {
        Debug.Log("----HippoShockWaveVFX");
        hippoShockWaveVFXEventTrigger?.Invoke();
    }

    public void OnHippoArmadilloBallThrow()
    {
        OnHippoArmadilloThrow?.Invoke();
    }
}
