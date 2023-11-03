using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HoneybeeDieVFXTrigger : VFXAction
{
    [SerializeField] private GameObject Content;    
    [Range(1, 10)][SerializeField]private float resetTime = 1;


    public override void OnVFXPlay(float delay)
    {
        base.OnVFXPlay(delay);
        Content.SetActive(false);
    }

    public override void OnVFXCompleted()
    {
        base.OnVFXCompleted();
        Debug.Log("VFX completed!");
        Content.SetActive(true);
    }
}
