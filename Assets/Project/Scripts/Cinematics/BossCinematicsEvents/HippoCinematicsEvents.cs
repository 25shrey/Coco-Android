using System;
using GameCoreFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Game.BaseFramework;

public class HippoCinematicsEvents : MonoBehaviour
{
    [FormerlySerializedAs("cinematicsContainer")]
    [Header("References")]
    [SerializeField] private GameObject cinematicsContainerBoss;
    [SerializeField] private GameObject bossObj;
    [SerializeField] private Player player;
    [SerializeField] private Transform spawnPoint;

    [Header("VFX")]
    [SerializeField] private VisualEffect jumpAttackVFX;
    [SerializeField] private VisualEffect angryVFX;
    [SerializeField] private VisualEffect smokePuffVFX;

    [Header("Boss Reference")]
    [SerializeField] private HippoEnemy hippoEnemy;

    [FormerlySerializedAs("virtualCameraShake")]
    [Header("Cinemachine Camera")]
    [SerializeField] private VirtualCameraShake virtualCameraShakeStartCinematics;
    [SerializeField] private VirtualCameraShake virtualCameraShakeEndCinematics;

    [Header("Collider")] [SerializeField] private GameObject endCollider;


    [Header("InputController")] 
    private InputController inputController;

    [Header("Player Movement Parameters")] 
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;


    [Header("Temp")] [SerializeField] private bool smokePuff;
    

    public void BeforeHippoEntryCinematics()
    {
        cinematicsContainerBoss.SetActive(true);
        bossObj.SetActive(false);
        BossArenaManage.Instance.PlayerBlockSetup();
        BossArenaManage.Instance.BossPath.SetActive(true);
    }

    public void OnHippoEntryCinematicsComplete()
    {
        hippoEnemy.gameObject.SetActive(true);
        GameManager.instance.StopVFX(jumpAttackVFX);
        GameManager.instance.StopVFX(angryVFX);
        cinematicsContainerBoss.SetActive(false);
    }

    public void CameraShake(float time)
    {
        virtualCameraShakeStartCinematics.ShakeCamera(0.8f, time);
    }

    public void CameraShake2(float time)
    {
        virtualCameraShakeStartCinematics.ShakeCamera(1.8f, time);
    }

    public void PlayJumpAttackVFX()
    {
        if (smokePuff)
        {
            GameManager.instance.PlayVFX(smokePuffVFX);
        }
        else
        {
            GameManager.instance.PlayVFX(jumpAttackVFX);
        }
    }

    public void PlayAngryVFXAndCameraShake(float cameraShakeTime)
    {
        GameManager.instance.PlayVFX(angryVFX);
        virtualCameraShakeStartCinematics.ShakeCamera(1f, cameraShakeTime);
    }

    public void OnEndCinematicsStart()
    {
        hippoEnemy.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        cinematicsContainerBoss.SetActive(true);
        GameManager.instance.input.DisableInput();
        MenuInput.DisableInput();
    }

    public async void OnEndCinematicsEnd()
    {
        endCollider.SetActive(true);
        player.transform.position = spawnPoint.position;
        player.gameObject.SetActive(true);
        var game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        GameManager.instance.Player.playerPowerUps.DeactivateAllPower();
        game.RemoveBossHealthBar();
        await Task.Delay(5000);
        GameManager.instance.OnCinematicsShowCloudEnd();
        cinematicsContainerBoss.SetActive(false);
        GameManager.instance.input.EnableInput();
        MenuInput.EnableUiInput();
    }

    public void MovePlayerTowardsEnd()
    {
        player.transform.position = startPos.position;
        player.gameObject.SetActive(true);
        StartCoroutine(GameManager.instance.MovePlayerToParticularPoint(player.transform, endPos.position,
            movementSpeed, rotationSpeed,
            () =>
            {
                player.gameObject.SetActive(false);
                ShowCloudScreen();
            }, 3.5f));
    }

    public void ShowCloudScreen()
    {
        GameManager.instance.OnCinematicsShowCloud();
    }

    public void ShowCameraShake(float cameraShakeTime)
    {
        virtualCameraShakeEndCinematics.ShakeCamera(1f, cameraShakeTime);
    }
}
