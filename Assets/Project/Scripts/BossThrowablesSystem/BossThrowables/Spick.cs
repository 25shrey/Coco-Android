using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Spick : MonoBehaviour
{
    #region PUBLIC_VARS

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    private void OnCollisionEnter(Collision collision)
    {
        Player plr = collision.gameObject.GetComponent<Player>();
        Debug.LogError("Colltion..."+collision.gameObject.name);
        if (PlayerCanDamage(plr))
        {
            Vector3 dir = (plr.transform.position - transform.position).normalized;
            plr.Damage(dir*1.8f, DamageAnimType.BeeDamage, 25);
            plr.PlayPushVFX();
            //transform.DOMove(transform.position+new Vector3(0,-4,0), 1.5f);
        }
    }

    #endregion

    #region PUBLIC_FUNCTIONS
    
    public bool PlayerCanDamage(Player plr)
    {
        return plr  && !plr.isDamageing && !plr.isDead;
    }

    public IEnumerator SetSpick()
    {
        yield return new WaitForSeconds(0.75f);
        transform.DOMove(transform.position+new Vector3(0,4,0), 1.5f);
        yield return new WaitForSeconds(5.25f);
        transform.DOMove(transform.position+new Vector3(0,-4,0), 1.5f);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
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
