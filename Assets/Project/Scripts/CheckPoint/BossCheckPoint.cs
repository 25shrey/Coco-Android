using System.Collections;
using System.Collections.Generic;
using Game.CheckPoints;
using GameCoreFramework;
using UnityEngine;
using UnityEngine.Serialization;

public class BossCheckPoint : CheckPointTrigger
{
    #region PUBLIC_VARS

    public GameObject bossObj;
    public Transform startPoint;
    public Transform EndPoint;

    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (((1 << other.gameObject.layer) & _layer) != 0)
        {
            CameraController.Instance.gameCamera.nearClipPlane = 0.3f;
            bossObj.SetActive(true);
            GameManager.instance.Player.SetBossFight(startPoint.position,EndPoint.position);
            GameManager.instance.Player._powerCanBeUsed = false;
            BossArenaManage.Instance.BGMusicSwitcher(true);
            GameManager.instance.input.DisableInput();
            MenuInput.DisableInput();
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
