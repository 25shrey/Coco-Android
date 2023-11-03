using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class Weapon : MonoBehaviour
{
    #region PUBLIC_VARS
    
    public Fireball fireballPrefab;
    private float fireRate;

    #endregion

    #region PRIVATE_VARS

    public bool isFired;
    
    #endregion

    #region UNITY_CALLBACKS

    private void Start()
    {
        fireRate = 2;
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    public async void Fire(Transform forwardPoint)
    {
        if (!isFired)
        {
            isFired = true;
            await Task.Delay(300);
            Fireball fireball = Instantiate(fireballPrefab);
            fireball.transform.position = transform.position;
            fireball.direction = (forwardPoint.position - GameManager.instance.Player.transform.position).normalized;
            fireball.Initialized();
            int time = 1000;
            if (fireRate > 0)
            {
                time = (int)(1000 / fireRate);
            }
            await Task.Delay(time);
            isFired = false;
       }
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