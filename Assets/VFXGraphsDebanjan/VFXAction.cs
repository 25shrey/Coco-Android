using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.VFX;

public class VFXAction : MonoBehaviour
{
    public virtual void OnVFXPlay(float delay)
    {
        StartCoroutine(Utility.CheckForVFXCompletion(delay,
             () => OnVFXCompleted()));
    }

    public virtual void OnVFXCompleted()
    {
        Debug.Log("VFX Completed!");
    }
}
