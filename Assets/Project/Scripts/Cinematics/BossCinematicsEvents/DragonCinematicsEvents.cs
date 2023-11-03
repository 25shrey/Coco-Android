using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class DragonCinematicsEvents : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GameObject cinematicsContainer;
    [SerializeField] private DragonEnemy dragonEnemy; 
    [SerializeField] private VirtualCameraShake entryVirtualCameraShake;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Player player;

    [Header("VFX")] 
    [SerializeField] private VisualEffect angryVFX;

    [Header("Colliders")] [SerializeField] private GameObject endCollider;

    [Header("Player References")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;





    public void CameraShake(float time)
    {
        entryVirtualCameraShake.ShakeCamera(1f, time);
    }

    public void AngryCameraShake(float time)
    {
        entryVirtualCameraShake.ShakeCamera(2f, time);
    }

    public void PlayAngryVFX()
    {
        //GameManager.instance.PlayVFX(angryVFX);
    }

    public void OnEntryCinematicsStart()
    {
        dragonEnemy.gameObject.SetActive(false);
        cinematicsContainer.SetActive(true);
    }
    
    public void OnEntryCinematicsCompleted()
    {
        dragonEnemy.gameObject.SetActive(true);
        cinematicsContainer.SetActive(false);
        //GameManager.instance.StopVFXImediatly(angryVFX);
    }


    public void OnExitCinematicsStart()
    {
        dragonEnemy.gameObject.SetActive(false);
        cinematicsContainer.SetActive(true);
        player.gameObject.SetActive(false);
        GameManager.instance.input.DisableInput();
        MenuInput.DisableInput();
    }
    
    public async void OnExitCinematicsComepleted()
    {
        endCollider.SetActive(true);
        player.transform.position = spawnPoint.position;
        player.gameObject.SetActive(true);
        cinematicsContainer.SetActive(false);
        GameManager.instance.Player.playerPowerUps.DeactivateAllPower();
        var game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        game.RemoveBossHealthBar();
        await Task.Delay(5000);
        GameManager.instance.OnCinematicsShowCloudEnd();
        GameManager.instance.input.EnableInput();
        MenuInput.EnableUiInput();
    }

    public void ShowCloudScreen()
    {
        GameManager.instance.OnCinematicsShowCloud();
    }

    public void MovePlayerTowardsEnd()
    {
        player.transform.position = startPoint.position;
        player.gameObject.SetActive(true);
        StartCoroutine(GameManager.instance.MovePlayer(player.transform, endPoint.position, movementSpeed, rotationSpeed,() =>
        {
            player.gameObject.SetActive(false);
            ShowCloudScreen();
        }));
    }


    /*private IEnumerator Move(Transform objectToMove, Vector3 endPoint, Action Callback)
    {
        Vector3 direction = (endPoint - objectToMove.position).normalized;
        Vector3 lookDir = endPoint - objectToMove.position;
        float actualDistance = Vector3.Distance(objectToMove.position, endPoint);
        float time = actualDistance / movementSpeed;
        Debug.Log($"<color=red>Time : {time}</color>");
        while(true)
        {
            player.characterController.Move(direction * Time.deltaTime * movementSpeed);
            lookDir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookDir);
            objectToMove.rotation = Quaternion.Slerp(objectToMove.rotation, rotation, Time.deltaTime * rotationSpeed);
            player.StartCustomRunAnimation();
            float distance = Vector3.Distance(objectToMove.position, endPoint);
            Debug.Log($"Distance : {distance}");
            if (distance < 1.7f)
            {
                Callback?.Invoke();
                break;
            }
            yield return null;
        }
    }*/
}

