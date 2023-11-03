using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.BaseFramework;
using UnityEngine;

public class BossEnemyManager : Singleton<BossEnemyManager>
{
    #region PUBLIC_VARS

    public BaseEnemy currentBoss;

    public BossHealthBar healthBar;

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    #endregion

    #region PUBLIC_FUNCTIONS

    public void StartBossFight(BaseEnemy boss)
    {
        currentBoss = boss;
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
