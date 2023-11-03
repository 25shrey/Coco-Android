using Game.CheckPoints;
using GameCoreFramework;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class EndPlatformCheckPoint : CheckPointTrigger
{
    [SerializeField] private Collider endPlatformColliders;
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            endPlatformColliders.enabled = true;
        }
    }
}
