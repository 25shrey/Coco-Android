using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.BaseFramework;
using UnityEngine;

public class PowerHandler : Singleton<PowerHandler>
{
    #region PUBLIC_VARS
    
    public Fireball fireballPrefab;
    private float fireRate;
    public MagnetPower magnetPowerPrefab;
    public ShieldPower shieldPowerPrefab;
    public int magnetPowerTime;
    public int shieldPowerTime;
    //public int fireballCount;
    //public int magnetCount;
    //public int shieldCount;
    
    #endregion

    #region PRIVATE_VARS

    public bool isFired;
    
    public bool magnetPowerUpInUse;
    
    public bool shieldPowerUpInUse;

    /*public bool IsFireBallAvailable()
    {
        return !isFired;
    }*/

    #endregion

    #region UNITY_CALLBACKS
    
    private void Start()
    {
        //LoadData();
        fireRate = 2;
    }
    
    #endregion

    #region PUBLIC_FUNCTIONS
    
    public async void Fire(Transform forwardPoint)
    {
        /*if (fireballCount <= 0)
        {
            return;
        }*/
        if (!isFired)
        {
            SavedDataHandler.Instance.PowerUpFireBallCount--;
            //SaveData();
            isFired = true;
            await Task.Delay(300);
            Fireball fireball = Instantiate(fireballPrefab);
            fireball.transform.position = GameManager.instance.Player.weapon.transform.position;
            fireball.direction = (forwardPoint.position - GameManager.instance.Player.transform.position).normalized;
            fireball.Initialized();
            int time = 1000;
            if (fireRate > 0)
            {
                time = (int)(1000 / fireRate);
            }
            await Task.Delay(time);
            isFired = false;
            if (SavedDataHandler.Instance.PowerUpFireBallCount == 0 )
            {
                //
            }
        }
    }

    public async void UseMagnetPower()
    {
        /*if (magnetCount < 0)
        {
            return;
        }*/
        magnetPowerUpInUse = true;
        SavedDataHandler.Instance.PowerUpMagnetCount--;
        //SaveData();
        MagnetPower magnetPower =  Instantiate(magnetPowerPrefab);
        magnetPower.seconds = magnetPowerTime;
        await Task.Delay(20);
        magnetPower.Collect();
    }

    public async void UseShieldPower()
    {
        /*if (shieldCount < 0)
        {
            return;
        }*/
        shieldPowerUpInUse = true;
        SavedDataHandler.Instance.PowerUpShieldCount--;
        //SaveData();
        ShieldPower shieldPower =  Instantiate(shieldPowerPrefab);
        shieldPower.seconds = shieldPowerTime;
        await Task.Delay(20);
        shieldPower.Collect();
    }

    /*public void LoadData()
    {
        shieldCount = 2;
        magnetCount = 2;
        fireballCount = 20;
    }*/
    
    
    /*public void SaveData()
    {
        
    }*/
    
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
