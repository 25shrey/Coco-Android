using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalEmitterManager : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetChild(13).GetComponent<Animator>(); 
    }

    public void DisableCocoAnimator()
    {
        if(!System.Object.ReferenceEquals(anim, null)) 
        {
            anim.enabled = false;
        }
    }
}
