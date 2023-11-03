using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    #region PUBLIC_VARS

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer).Equals("Coco"))
        {
            GameManager.instance.Player.DeathZoneHit();
            GameManager.instance.isDeadZoneTrigger = true;
        }
    }

    #endregion

    #region PUBLIC_FUNCTIONS

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
