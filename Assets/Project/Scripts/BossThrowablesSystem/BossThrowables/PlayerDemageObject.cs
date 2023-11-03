using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDemageObject : MonoBehaviour
{
    #region PUBLIC_VARS

    public float damagePower;
    public float damageDistance;
    
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(">>>Cube demage trigger");
        Player player = other.GetComponent<Player>();
        if(PlayerCanDamage(player))
        {
            Debug.Log(">>> player Cube demage trigger");
            Vector3 dir = (player.transform.position-transform.position).normalized;
            player.Damage(dir*damageDistance,DamageAnimType.Damage,damagePower);
            player.PlayImpactVFX(10000);
        }
    }
    
    #endregion

        #region PUBLIC_FUNCTIONS
    
    public bool PlayerCanDamage(Player plr)
    {
        return plr && !plr.isDamageing && !plr.isDead;
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
