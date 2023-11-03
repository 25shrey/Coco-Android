using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    #region PUBLIC_VARS

    public float damagePower;
    public Player player;
    
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS
    
    private void OnCollisionEnter(Collision collision)
    {
        if (player==null)
        {
            player = collision.gameObject.GetComponent<Player>(); 
        }
        //Player plr = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            //  GameManager.instance.Player._sounds.SoundToBeUsed(3, SoundManager.Soundtype.player, 0.5f);

            SoundManager._soundManager._otherSounds.SoundToBeUsed(3, SoundManager.Soundtype.player, false, true);

            player.DamageByFire(damagePower);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (player==null)
        {
            player = other.gameObject.GetComponent<Player>(); 
        }
        //Player plr = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            //  GameManager.instance.Player._sounds.SoundToBeUsed(3, SoundManager.Soundtype.player, 0.5f);

            //SoundManager._soundManager._otherSounds.SoundToBeUsed(3, SoundManager.Soundtype.player, false, true);

            player.DamageByFire(damagePower);
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
