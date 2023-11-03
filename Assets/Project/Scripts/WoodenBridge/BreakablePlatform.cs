using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    //[SerializeField] ParticleSystem explosionEffect;
    [SerializeField] 
    internal float breakTime;
    [SerializeField]
    bool isBreakable;
    internal bool isBroken = false;


    private void Start()
    {
        if(breakTime < 5f)
        {
            isBreakable = true;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(LayerMask.LayerToName(collision.gameObject.layer).Equals("Coco"))
        {
            if (!isBroken && isBreakable)
            {
                Invoke("BreakPlatform", breakTime);
            }
        }
    }

    private void BreakPlatform()
    {
        //Instantiate(explosionEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        isBroken = true;
    }
}
