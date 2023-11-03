using Game.BaseFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BullCinematicsEvents : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform spawnPoint;

    [Header("Player")] 
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Player player;

    [Header("Colliders")]
    [SerializeField] private Collider endCollider;
    

    public void OnEndCinematicsStart()
    {
        player.gameObject.SetActive(false);
        GameManager.instance.input.DisableInput();
        MenuInput.DisableInput();
    }
    
    public async void OnEndCinematicsCompleted()
    {
        player.gameObject.transform.position = spawnPoint.position;
        player.gameObject.SetActive(true);
        await Task.Delay(5000);
        var game = UIController.Instance.GetScreen<GameplayUI>(ScreenType.Gameplay);
        game.RemoveBossHealthBar();
        GameManager.instance.OnCinematicsShowCloudEnd();
        GameManager.instance.Player.playerPowerUps.DeactivateAllPower();
        endCollider.enabled = true;
        GameManager.instance.input.EnableInput();
        MenuInput.EnableUiInput();
    }

    public void MovePlayer()
    {
        player.transform.position = startPoint.position;
        player.gameObject.SetActive(true);
        StartCoroutine(GameManager.instance.MovePlayerToParticularPoint(player.transform, endPoint.position,
            movementSpeed, rotationSpeed,
            () =>
            {
                player.gameObject.SetActive(false);
                GameManager.instance.OnCinematicsShowCloud();
            }));
    }
}
